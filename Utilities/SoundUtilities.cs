using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Speech.Synthesis;
using System.Text;
using WaveLib;
using Yeti.MMedia.Mp3;

namespace JukeWeb.Foundry.Utilities.Utilities
{
    public class SoundUtilities
    {
        public static byte[] TextToSound(string text, string filePath, string c, string g, string a)
        {
            try
            {
                byte[] byteArr = null;

                var t = new System.Threading.Thread(() =>
                {
                    SpeechSynthesizer ss = new SpeechSynthesizer();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        ss.SetOutputToWaveStream(memoryStream);
                        ss.Volume = 100;

                        var gender = (VoiceGender) Enum.Parse(typeof(VoiceGender), g, true);
                        var age = (VoiceAge) Enum.Parse(typeof(VoiceAge), a, true);
                        var culture = new System.Globalization.CultureInfo(c);    
                        ss.SelectVoiceByHints(gender, age, 0, culture);

                        ss.Speak(text);
                        if (!String.IsNullOrEmpty(filePath))
                        {
                            var fileName = Path.GetFileName(filePath);
                            string fullPath = Path.GetFullPath(filePath).Replace(fileName, "");
                            bool isExists = System.IO.Directory.Exists(fullPath);

                            if (!isExists)
                                System.IO.Directory.CreateDirectory(fullPath);
                            System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
                        }
                        byteArr = memoryStream.ToArray();
                    }
                });
                t.Start();
                t.Join();
                return byteArr;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static void ConvertWavToMp3(string wavFilePath, bool saveFile = false)
        {
            WaveStream wavStream = new WaveStream(wavFilePath);
            Mp3WriterConfig m_Config = new Mp3WriterConfig(wavStream.Format);
            string mp3FilePath = wavFilePath.Replace(".wav", ".mp3");

            try
            {
                Mp3Writer writer = new Mp3Writer(new FileStream(mp3FilePath, FileMode.Create), m_Config);
                try
                {
                    byte[] buff = new byte[writer.OptimalBufferSize];
                    int read = 0;
                    int actual = 0;
                    long total = wavStream.Length;
                    try
                    {
                        while ((read = wavStream.Read(buff, 0, buff.Length)) > 0)
                        {
                            writer.Write(buff, 0, read);
                        }

                        if (saveFile)
                        {
                            var fileName = Path.GetFileName(mp3FilePath);
                            string fullPath = Path.GetFullPath(mp3FilePath).Replace(fileName, "");
                            bool isExists = System.IO.Directory.Exists(fullPath);

                            if (!isExists)
                                System.IO.Directory.CreateDirectory(fullPath);
                            System.IO.File.WriteAllBytes(mp3FilePath, buff.ToArray());
                        }
                    }
                    finally
                    {
                    }
                }
                finally
                {
                    writer.Close();
                }
            }
            finally
            {
                wavStream.Close();
            }
        }
    }
}
