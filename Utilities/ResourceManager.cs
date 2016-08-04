using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace JukeWeb.Foundry.Utilities
{
	public class ResourceManager
	{
		static ResourceManager() { }

		public static Stream GetEmeddedResource(System.Reflection.Assembly asm, string resName)
		{
			Stream s = asm.GetManifestResourceStream(string.Format("{0}.{1}",asm.GetName().Name,resName));
			return s;
		}

        public static string GetResourceString(System.Reflection.Assembly asm, string resourceName, CultureInfo cult, string title)
        { 
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(resourceName, asm);
            return rm.GetString(title, cult);    
        }


        public static string GetFileASCII(System.Reflection.Assembly asm, string name)
        {
            Stream s = GetEmeddedResource(asm, name);
            byte[] bytes = new byte[s.Length];
            s.Position = 0;
            s.Read(bytes, 0, (int)s.Length);
            s.Flush();
            return Encoding.ASCII.GetString(bytes).Trim();
        }

        public static string GetFileUTF7(System.Reflection.Assembly asm, string name)
        {
            Stream s = GetEmeddedResource(asm, name);
            byte[] bytes = new byte[s.Length];
            s.Position = 0;
            s.Read(bytes, 0, (int)s.Length);
            s.Flush();
            return Encoding.UTF7.GetString(bytes).Trim();
        }

        public static string GetFileUTF8(System.Reflection.Assembly asm, string name)
        {
            Stream s = GetEmeddedResource(asm, name);
            byte[] bytes = new byte[s.Length];
            s.Position = 0;
            s.Read(bytes, 0, (int)s.Length);
            s.Flush();
            return Encoding.UTF8.GetString(bytes).Trim();
        }

	}
}
