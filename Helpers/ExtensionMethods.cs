using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tutioncloud.Helpers
{
    public static class ExtensionMethods
    {
        //Extension method to substring text
        public static string Chop(this string text, int chopLength, string postfix = "...")
        {
            if (text == null || text.Length < chopLength)
                return text;
            else
                return text.Substring(0, chopLength - postfix.Length) + postfix;
        }
    }
}