using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Extender.Strings;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    //
    // TODO_  (ResourceSerializer re-write)
    //
    //      I should really just save each new reference's xml to a separate file, creating
    //      filenames at runtime by checking for collisions and incrementing --
    //      OR better yet, just check to see how many files are in the Subfolder and then 
    //      check / inc as neccessary.
    //
    //      This could make loading easier, since I could put IResource.Name in the filename.
    //      That way instead of loading (potentially) hundreds of XML files, I'd just need to 
    //      iterate through a List<string> of filenames in the Subfolder.
    //
    //      OR None of this, because I'm a fucking idiot. Just pass the Post object around
    //      and add to the Resources list directly.

    /// <remarks>
    /// Class containing functions for serialization and deserialization of 
    /// IResource objects.
    /// </remarks>
    public class ResourceSerializer
    {
        /// <summary>
        /// Filename of the aggregated xml file containing IResource object data.
        /// </summary>
        public string AggregateFilename
        {
            get;
            set;
        }
        /// <summary>
        /// String to insert between IResource's xml in the aggregated xml file.
        /// </summary>
        public string Delimiter
        {
            get;
            set;
        }
        /// <summary>
        /// Path to the subfolder which holds the split temporary xml files.
        /// </summary>
        public string Subfolder
        {
            get;
            set;
        }
        /// <summary>
        /// Switch to control using a regular path, or treating all paths as 
        /// relative to the current working directory.
        /// </summary>
        public bool UseProgramFolder
        {
            get;
            set;
        }

        protected string AbsoluteSubfolderPath
        {
            get
            {
                if (UseProgramFolder)
                    return Path.Combine(Directory.GetCurrentDirectory(), Subfolder);
                else
                    return Path.GetFullPath(Subfolder);
            }
        }
        protected bool SubfolderEmpty
        {
            get
            {
                return (Directory.GetFiles(AbsoluteSubfolderPath).Length < 1);
            }
        }
        protected string AbsoluteFilename
        {
            get
            {
                if (UseProgramFolder)
                    return Path.Combine(
                        Directory.GetCurrentDirectory(),
                        AggregateFilename);
                else
                    return Path.GetFullPath(AggregateFilename);
            }
        }

        /// <summary>
        /// Initializes a new ResourceSerializer.
        /// </summary>
        public ResourceSerializer()
        {
            this.AggregateFilename  = Properties.Settings.Default.TempReferenceFilename;
            this.Delimiter          = @"----";
            this.Subfolder          = @"refs_temp";
            this.UseProgramFolder   = true;
        }

        protected string GetSplitFilename(Int16 fileNum)
        {
            return AggregateFilename.InsertBeforeExtension("_" + fileNum.ToString("D3"));
        }

        protected void DeleteTempFiles()
        {
            foreach (string file in Directory.GetFiles(AbsoluteSubfolderPath))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Uses XmlSerializer to serialize the IResource object into xml, and stores
        /// that xml in an aggregated xml file containing all current temporary references.
        /// </summary>
        /// <param name="resource">IResource object to add to the aggregated temporary file.</param>
        public void Save(IResource resource)
        {
            Save(new IResource[] { resource });
        }

        /// <summary>
        /// Uses XmlSerializer to serialize the IResource objects into xml, and stores
        /// them in an aggregated xml file containing all current temporary references.
        /// </summary>
        /// <param name="resource">List of all resources to add to the aggregated temporary file.</param>
        public void Save(IResource[] resources)
        {

            using (StreamWriter file = File.AppendText(AbsoluteFilename))
            {
                foreach (IResource resource in resources)
                {
                    XmlSerializer w = new XmlSerializer(resource.GetType());
                    w.Serialize(file, resource);
                    file.WriteLine("\n" + Delimiter);
                }
            }
        }

        /// <summary>
        /// Creates a list of IResource objects deserialized from the aggregated tempory xml file.
        /// </summary>
        /// <returns>List of IResource objects loaded from disc.</returns>
        public List<IResource> Load()
        {
            SplitTempXML(AbsoluteFilename);

            List<IResource> resources = new List<IResource>();

            foreach (string file in Directory.GetFiles(AbsoluteSubfolderPath))
            {
                if (Path.GetExtension(file).ToLower() != ".xml")
                    continue;

                XmlSerializer s = new XmlSerializer(GetRefType(file));

                using (FileStream streamedResource = new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    resources.Add((IResource)s.Deserialize(streamedResource));
                }
            }

            DeleteTempFiles();
            return resources;
        }

        protected void SplitTempXML(string filename)
        {
            if (!Directory.Exists(AbsoluteSubfolderPath))
                Directory.CreateDirectory(AbsoluteSubfolderPath);

            if (!SubfolderEmpty) DeleteTempFiles();


            using (StreamReader buffer = new StreamReader(AbsoluteFilename))
            {
                Int16 fileNum = 1;
                string cl;

                if (buffer.Peek() != -1) cl = buffer.ReadLine();
                else return;

                while (!String.IsNullOrEmpty(cl))
                {
                    StringBuilder curObject = new StringBuilder();
                    while (cl != Delimiter && buffer.Peek() != -1)
                    {
                        if (!string.IsNullOrEmpty(cl))
                            curObject.AppendLine(cl);

                        cl = buffer.ReadLine();
                    }

                    using (StreamWriter stream = File.CreateText(Path.Combine(
                            Directory.GetCurrentDirectory(),
                            Subfolder,
                            GetSplitFilename(fileNum++))))
                        stream.Write(curObject.ToString());

                    if (buffer.Peek() != -1) cl = buffer.ReadLine();
                    else return;
                }
            }
        }

        protected Type GetRefType(string file)
        {
            using (FileStream streamedResource = new FileStream(
                       file,
                       FileMode.Open,
                       FileAccess.Read,
                       FileShare.ReadWrite))
            {
                XmlReader r = XmlReader.Create(streamedResource);
                r.MoveToContent();
                return Type.GetType(GetResourceFullTypeName(r.Name));
            }
        }

        protected string GetResourceFullTypeName(string typeName)
        {
            return string.Format("{0}.{1}",
                typeof(IResource).Namespace,
                typeName.Trim(new char[] { ' ', '.' }));
        }


        /*
         * 
         * debug members
         * 
         */

        public static void TestHarness()
        {
            ResourceSerializer s = new ResourceSerializer();

            foreach (IResource resource in s.Load())
            {
                Extender.Debugging.Debug.WriteMessage(
                    string.Format(" N:{0} V:{1}", resource.Name, resource.Value));
            }
        }
    }
}
