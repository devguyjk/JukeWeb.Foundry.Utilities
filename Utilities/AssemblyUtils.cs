using System;
using System.IO;
using System.Runtime;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace JukeWeb.Foundry.Utilities
{
    public class AssemblyUtils
    {
        /// <summary>
        /// Gets an assembly path from the GAC given a partial name.
        /// </summary>
        /// <param name="name">An assembly partial name. May not be null.</param>
        /// <returns>
        /// The assembly path if found; otherwise null;
        /// </returns>
        /// 
        private static Dictionary<string,Assembly> _embeddedAssemblies = null;

        public static string GetAssemblyRootDirectory(Assembly asm)
        {            
            return GetAssemblyRootDirectory(asm.GetName().FullName);
        }

        public static string GetAssemblyRootDirectory(string asmName)
        {
            string path = Assembly.Load(new AssemblyName(asmName)).Location;
            return path.Substring(0, path.LastIndexOf("\\"));
        }

        public static string GetAssemblyPath(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            string finalName = name;
            AssemblyInfo aInfo = new AssemblyInfo();
            aInfo.cchBuf = 1024; // should be fine...
            aInfo.currentAssemblyPath = new String('\0', aInfo.cchBuf);

            IAssemblyCache ac;
            int hr = CreateAssemblyCache(out ac, 0);
            if (hr >= 0)
            {
                hr = ac.QueryAssemblyInfo(0, finalName, ref aInfo);
                if (hr < 0)
                    return null;
            }

            return aInfo.currentAssemblyPath;
        }

        public static void LoadEmbeddedAssembly(Assembly executingAssembly, string embeddedResourceName)
        {
            if (_embeddedAssemblies == null)
                _embeddedAssemblies = new Dictionary<string, Assembly>();

            byte[] ba = null;
            Assembly embeddedAssembly = null;

            using (Stream stm = executingAssembly.GetManifestResourceStream(string.Format("{0}.{1}",executingAssembly.GetName(false).Name, embeddedResourceName)))
            {
                // Either the file is not existed or it is not mark as embedded resource
                if (stm == null)
                    throw new Exception(embeddedResourceName + " is not found in Embedded Resources.");

                // Get byte[] from the file from embedded resource
                ba = new byte[(int)stm.Length];
                stm.Read(ba, 0, (int)stm.Length);
                try
                {
                    embeddedAssembly = Assembly.Load(ba);

                    // Add the assembly/dll into dictionary
                    _embeddedAssemblies.Add(embeddedAssembly.FullName, embeddedAssembly);
                    return;
                }
                catch
                {
                    // Purposely do nothing
                    // Unmanaged dll or assembly cannot be loaded directly from byte[]
                    // Let the process fall through for next part
                }
            }

            bool fileOk = false;
            string tempFile = "";

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty); ;

                tempFile = Path.GetTempPath() + embeddedResourceName;

                if (File.Exists(tempFile))
                {
                    byte[] bb = File.ReadAllBytes(tempFile);
                    string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                    if (fileHash == fileHash2)
                    {
                        fileOk = true;
                    }
                    else
                    {
                        fileOk = false;
                    }
                }
                else
                {
                    fileOk = false;
                }
            }

            if (!fileOk)
            {
                System.IO.File.WriteAllBytes(tempFile, ba);
            }

            embeddedAssembly = Assembly.LoadFile(tempFile);
            _embeddedAssemblies.Add(embeddedAssembly.FullName, embeddedAssembly);

        }

        public static Assembly GetEmbeddedAssembly(string assemblyFullName)
        {
            if (_embeddedAssemblies == null || _embeddedAssemblies.Count == 0)
                return null;

            if (_embeddedAssemblies.ContainsKey(assemblyFullName))
                return _embeddedAssemblies[assemblyFullName];

            return null;
        }

        #region DLLImport / Win32 Helpers

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        private interface IAssemblyCache
        {
            void Reserved0();

            [PreserveSig]
            int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AssemblyInfo
        {
            public int cbAssemblyInfo;
            public int assemblyFlags;
            public long assemblySizeInKB;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string currentAssemblyPath;
            public int cchBuf; // size of path buf.
        }

        [DllImport("fusion.dll")]
        private static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

        #endregion

    }
}
