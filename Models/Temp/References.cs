using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PushPost.Models.Temp
{
    [Obsolete]
    public class References
    {   
        protected static string Delimiter
        {
            get
            {
                return @"----";
            }
        }

        public static string Subfolder
        {
            get
            {
                return "refs_temp";
            }
        }

        protected static string GetSplitFilename(byte fileNum)
        {
            return string.Format("refs_{0}.tmp.xml", fileNum.ToString("D3"));
        }

        protected static string Absolute(string filename)
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                filename);
        }

        protected static bool SubfolderEmpty(string subfolder)
        {
            int numFiles = Directory.GetFiles(Absolute(subfolder)).Length;

            return (numFiles < 1);
        }

        protected static void DeleteAllIn(string subfolder)
        {
            foreach(string file in Directory.GetFiles(Absolute(subfolder)))
            {
                File.Delete(file);
            }
        }

        public static void Save(NotifyingResource resource, string filename)
        {
            Save(new NotifyingResource[] { resource }, filename);
        }

        public static void Save(NotifyingResource[] resources, string filename)
        {

            using (StreamWriter file = File.AppendText(Absolute(filename)))
            {
                foreach (NotifyingResource resource in resources)
                {
                    XmlSerializer w = new XmlSerializer(resource.GetType());
                    w.Serialize(file, resource);
                    file.WriteLine("\n" + Delimiter);
                }
            }
        }

        public static List<NotifyingResource> Load(string filename)
        {
            SplitTempXML(filename);

            List<NotifyingResource> resources = new List<NotifyingResource>();

            foreach(string file in Directory.GetFiles(Absolute(Subfolder)))
            {
                if (Path.GetExtension(file).ToLower() != ".xml")
                    continue;

                XmlSerializer s = new XmlSerializer(GetRefType(file));

                using(FileStream streamedResource = new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    resources.Add((NotifyingResource)s.Deserialize(streamedResource));
                }
            }

            DeleteAllIn(Subfolder);
            return resources;
        }

        protected static void SplitTempXML(string filename)
        {
            if (!Directory.Exists(Absolute(Subfolder)))
                Directory.CreateDirectory(Absolute(Subfolder));

            if (!SubfolderEmpty(Subfolder)) DeleteAllIn(Subfolder);


            using(StreamReader buffer = new StreamReader(Absolute(filename)))
            {
                byte fileNum = 1;
                string cl;

                if (buffer.Peek() != -1) cl = buffer.ReadLine();
                else return;

                while (!string.IsNullOrEmpty(cl))
                {
                    StringBuilder curObject = new StringBuilder();
                    while (cl != Delimiter && buffer.Peek() != -1)
                    {
                        if(!string.IsNullOrEmpty(cl))
                            curObject.AppendLine(cl);

                        cl = buffer.ReadLine();
                    }

                    using(StreamWriter stream = File.CreateText(Path.Combine(
                            Directory.GetCurrentDirectory(),
                            Subfolder,
                            GetSplitFilename(fileNum++))))
                        stream.Write(curObject.ToString());

                    if (buffer.Peek() != -1) cl = buffer.ReadLine();
                    else return;
                }
            }
        }

        protected static Type GetRefType(string file)
        {
            using (FileStream streamedResource = new FileStream(
                       file,
                       FileMode.Open,
                       FileAccess.Read,
                       FileShare.ReadWrite))
            {
                XmlReader r = XmlReader.Create(streamedResource);
                r.MoveToContent();
                return Type.GetType(GetFullName(r.Name));
            }
        }

        protected static string GetFullName(string typeName)
        {
            return string.Format("{0}.{1}", 
                typeof(NotifyingResource).Namespace, 
                typeName.Trim(new char[] {' ', '.'}));
        }


        /*
         * 
         * Debugging members
         * 
         */

        public static void TestHarness()
        {
            foreach (IResource resource in Load(Properties.Settings.Default.TempReferenceFilename))
            {
                Console.WriteLine(string.Format(" N:{0} V:{1}", resource.Name, resource.Value));
            }
        }
    }
}
