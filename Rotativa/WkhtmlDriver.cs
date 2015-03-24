using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Rotativa
{
    public abstract class WkhtmlDriver
    {
        /// <summary>
        /// Converts given URL or HTML string to PDF.
        /// </summary>
        /// <param name="wkhtmlPath">Path to wkthmltopdf\wkthmltoimage.</param>
        /// <param name="switches">Switches that will be passed to wkhtmltopdf binary.</param>
        /// <param name="html">String containing HTML code that should be converted to PDF.</param>
        /// <param name="wkhtmlExe"></param>
        /// <returns>PDF as byte array.</returns>
        protected static byte[] Convert(string wkhtmlPath, string switches, string html, string wkhtmlExe)
        {
            // switches:
            //     "-q"  - silent output, only errors - no progress messages
            //     " -"  - switch output to stdout
            //     "- -" - switch input to stdin and output to stdout
            switches = "-q " + switches + " -";

            // generate PDF from given HTML string, not from URL
            if (!string.IsNullOrEmpty(html))
            {
                switches += " -";
                html = SpecialCharsEncode(html);
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(wkhtmlPath, wkhtmlExe),
                    Arguments = switches,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    WorkingDirectory = wkhtmlPath,
                    CreateNoWindow = true
                }
            };
            proc.Start();

            // generate PDF from given HTML string, not from URL
            if (!string.IsNullOrEmpty(html))
            {
                using (var sIn = proc.StandardInput)
                {
                    sIn.WriteLine(html);
                }
            }

            using (var ms = new MemoryStream())
            {
                using (var sOut = proc.StandardOutput.BaseStream)
                {
                    byte[] buffer = new byte[4096];
                    int read;

                    while ((read = sOut.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                string error = proc.StandardError.ReadToEnd();

                if (ms.Length == 0)
                {
                    throw new Exception(error);
                }

                proc.WaitForExit();

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Encode all special chars
        /// </summary>
        /// <param name="text">Html text</param>
        /// <returns>Html with special chars encoded</returns>
        private static string SpecialCharsEncode(string text)
        {
            var chars = text.ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (var c in chars)
            {
                var value = System.Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}