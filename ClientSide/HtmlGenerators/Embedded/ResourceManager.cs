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

        // TODO_ Implement ResourceManager.NextResource()
        public static IResource NextResource(string text, List<IResource> resources)
        {
            throw new NotImplementedException();
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
                int i = n + 1;
                while(!remainingText[i].Equals(')')) // walk through each character after n until we reach the close bracket
                    reference += remainingText[i++];

                parsed += remainingText.Substring(0, n - 2);
                if (expand && resources.Count > 0)
                    parsed += GetResourceByName(reference, resources).CreateHTML();

                remainingText = remainingText.Substring(i + 1); // chop off everything BEFORE the final ')'
            }

            return parsed + remainingText;
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
