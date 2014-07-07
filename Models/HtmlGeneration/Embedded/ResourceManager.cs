﻿using System;
using System.Collections.Generic;
using Extender.Debugging;

namespace PushPost.Models.HtmlGeneration.Embedded
{
    /// <remarks>
    /// A helper class made up of static methods to handle expanding the markup
    /// from a post into HTML for any IResource implemntation. 
    /// 
    /// A reference to an IResource object should be of the form: 
    /// +@(resource_name).
    /// </remarks>
    public static class ResourceManager
    {
        /// <summary>
        /// Parses a block of text to find the index at which the first
        /// reference (written as '+@()') for an IResource object appears. 
        /// </summary>
        /// <param name="text">The block of text to search in.</param>
        /// <returns>
        /// The 0-based index of the beginning of first reference to 
        /// an IResource object. Refers to the first '(' of the reference.
        /// </returns>
        public static int FirstIndex(string text)
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

        /// <summary>
        /// Parses a block of text to extract the full name of the first
        /// appearing reference to an IResource object.
        /// </summary>
        /// <param name="text">The block of text to search in.</param>
        /// <returns>
        /// The full text of the IResource name contained in the first 
        /// appearance of a reference.
        /// eg for the text "this line contains a reference +@(a_reference)" 
        /// the string "a_reference" is returned. 
        /// </returns>
        public static string FirstResourceName(string text)
        {
            string resource = string.Empty;

            try
            {
                for (int i = FirstIndex(text) + 1; !text[i].Equals(')'); i++)
                    resource += text[i];
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
            
            return resource;
        }

        /// <summary>
        /// Parses a block of text to extract the first reference to an
        /// IResource, and return that IResource object.
        /// </summary>
        /// <param name="text">The block of text to search in.</param>
        /// <param name="resources">The list of possible resources with which to match 
        /// referenced names against.</param>
        /// <returns>The IResource object named in the first reference contained in the 
        /// provided block of text.</returns>
        public static IResource FirstResource(string text, List<IResource> resources)
        {
            return GetResourceByName(FirstResourceName(text), resources);
        }

        /// <summary>
        /// Builds a list of names from all the IResource references in a 
        /// block of text.
        /// </summary>
        /// <param name="text">The block of text to parse.</param>
        /// <returns>The list of strings containing all IResource references
        /// present in provided text.</returns>
        public static List<string> GetResourceNames(string text)
        {
            string remainingText = text;
            List<string> resourceNames = new List<string>();
            
            int n;
            while((n = FirstIndex(remainingText)) > 0)
            {
                resourceNames.Add(FirstResourceName(remainingText));
                remainingText = remainingText.Substring(n);
            }

            return resourceNames;
        }

        /// <summary>
        /// Replaces all IResource references with the full HTML generated
        /// by the corresponding IResource(s).
        /// </summary>
        /// <param name="text">The block of text to parse.</param>
        /// <param name="resources">The list of possible resources to replace
        /// the references with. Only IResources named in the text will be used.</param>
        /// <returns>The parsed text with all IResource references [+@(reference_name)]
        /// replaced with full HTML generated by the IResources.</returns>
        public static string ExpandReferences(string text, List<IResource> resources)
        {
            return ReplaceReferences(text, resources, true);
        }

        /// <summary>
        /// Finds all IResource references (of the form +@(resource_name) ) 
        /// and removes them.
        /// </summary>
        /// <param name="text">The text to remove references from.</param>
        /// <returns>The parsed text will all IResource references removes.
        /// No HTML is added. </returns>
        public static string RemoveReferences(string text)
        {
            return ReplaceReferences(text, null, false);
        }
        
        /// <summary>
        /// Finds all references in a block of text and either removes, or expands them.
        /// </summary>
        private static string ReplaceReferences(string text, List<IResource> resources, bool expand)
        {
            string parsed = string.Empty;

            int n;
            string remainingText = text;
            while ((n = FirstIndex(remainingText)) > 0) // skip to next reference
            {                
                string reference = string.Empty;
                int i = n + 1;
                // walk through each character after n until we reach the close bracket
                while(!remainingText[i].Equals(')')) 
                    reference += remainingText[i++];

                parsed += remainingText.Substring(0, n - 2);
                if (resources == null)
                    Debug.WriteMessage("resources is null, no references can be expanded.", "warn");
                else if (expand && resources.Count > 0)
                    parsed += GetResourceByName(reference, resources).CreateHTML();
                // chop off everything BEFORE the final ')'
                remainingText = remainingText.Substring(i + 1); 
            }

            return parsed + remainingText;
        }

        /// <summary>
        /// Matches a resourceName (reference) to an IResource from a 
        /// provided list.
        /// </summary>
        /// <param name="resourceName">Name (text contained in a reference) of
        /// the IResource being fetched. </param>
        /// <param name="resources">List of all IResources to select from.</param>
        /// <returns>The IResource whose name matches 'resourceName'. Returns
        /// null if no match is found. </returns>
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
