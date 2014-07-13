using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPost.Models.HtmlGeneration.Embedded;
using System.IO;
using System.Xml;

namespace PushPost.Models.Temp
{
    public class References
    {     
        public static void Save(IResource resource, string filename)
        {
            System.Xml.Serialization.XmlSerializer w =
                new System.Xml.Serialization.XmlSerializer(resource.GetType());

            using (StreamWriter file = File.AppendText(Path.Combine(
                Directory.GetCurrentDirectory(),
                filename)))
            {

                w.Serialize(file, resource);

                file.WriteLine("\n----");                
            }
        }

        public static List<IResource> Load(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
