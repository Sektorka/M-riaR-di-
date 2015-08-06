using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Maria_Radio
{
    static public class ProgramInfo
    {
        static public string AssemblyGuid
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((GuidAttribute)attributes[0]).Value;
            }
        }
        static public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }
    }
}
