using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace JukeWeb.Foundry.Utilities.Utilities.Images
{
    public class SvgUtils
    {
        private static SvgUtils _instance;
        public static SvgUtils Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SvgUtils();
                return _instance;
            }
        }

        private Semaphore _locker;

        private SvgUtils()
        {
            _locker = new Semaphore(10, 10);
        }

        public byte[] ConvertToPng(string cmdPath, string dirPath, byte[] fileBytes, out string error)
        {
            string svgPath = null, pngPath = null;
            byte[] result = null;
            error = string.Empty;
            try
            {
                if (!File.Exists(cmdPath))
                    throw new Exception("The inkscape runtime .exe doesn't exist.");
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                svgPath = Path.Combine(dirPath, Guid.NewGuid() + ".svg");
                pngPath = Path.Combine(dirPath, Guid.NewGuid() + ".png");
                File.WriteAllBytes(svgPath, fileBytes);

                Process inkscape = new Process();
                inkscape.StartInfo.FileName = cmdPath;
                inkscape.StartInfo.Arguments = string.Format("-f \"{0}\" -e \"{1}\"", svgPath, pngPath);
                inkscape.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                _locker.WaitOne();
                Exception exception = null;
                try
                {
                    inkscape.Start();
                    inkscape.WaitForExit();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                _locker.Release();
                if (exception != null)
                    throw exception;
                if (File.Exists(pngPath))
                    result = File.ReadAllBytes(pngPath);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                if (svgPath != null && File.Exists(svgPath))
                    File.Delete(svgPath);
                if (pngPath != null && File.Exists(pngPath))
                    File.Delete(pngPath);
            }

            return result;
        }
    }
}
