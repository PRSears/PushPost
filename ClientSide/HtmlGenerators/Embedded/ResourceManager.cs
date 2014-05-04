using System;
using System.Collections.Generic;

namespace PushPost.ClientSide.HtmlGenerators.Embedded
{
    public static class ResourceManager
    {
        public static int NextIndex(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if(text[i].Equals('+') && i < text.Length - 2)
                {
                    if (text[++i].Equals('@') && text[++i].Equals('('))
                        return i;
                }
            }
            
            return -1;
        }

        public static string NextResourceName(string text)
        {
            string resource = string.Empty;

            try
            {
                for (int i = NextIndex(text) + 1; !text[i].Equals(')'); i++)
                    resource += text[i];
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }

            return resource;
        }

        public static IResource NextResource(string text, List<IResource> resources)
        {
            return GetResourceByName(NextResourceName(text), resources);
        }

        public static List<string> GetResourceNames(string text)
        {
            string remainingText = text;
            List<string> resourceNames = new List<string>();
            
            int n;
            while((n = NextIndex(remainingText)) > 0)
            {
                resourceNames.Add(NextResourceName(remainingText));
                remainingText = remainingText.Substring(n);
            }

            return resourceNames;
        }

        public static string ExpandReferences(string text, List<IResource> resources)
        {
            return ReplaceReferences(text, resources, true);
        }

        public static string RemoveReferences(string text)
        {
            return ReplaceReferences(text, null, false);
        }

        private static string ReplaceReferences(string text, List<IResource> resources, bool expand)
        {
            string parsed = string.Empty;

            int n;
            string reference;
            string remainingText = text;
            while ((n = NextIndex(remainingText)) > 0) // skip to next reference
            {                
                reference = string.Empty;
                int i = n;
                while(!remainingText[++i].Equals(')')) // walk through each character after n until we reach the close bracket
                    reference += remainingText[i];

                parsed += remainingText.Substring(0, n - 2);
                if (expand)
                    parsed += GetResourceByName(reference, resources).CreateHTML();

                remainingText = remainingText.Substring(i++);
            }

            return parsed + remainingText.Substring(1); // append any leftover text, starting after the last ')'
        }

        public static IResource GetResourceByName(string resourceName, List<IResource> resources)
        {
            foreach(IResource r in resources)
            {
                if (r.Name.Equals(resourceName))
                    return r;
            }

            return null;
        }
    }
}
