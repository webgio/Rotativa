using System;
using System.Diagnostics;
using System.IO;

namespace Rotativa
{
    public class WkhtmltopdfDriver
    {
        /// <summary>
        /// Converts given HTML string to PDF.
        /// </summary>
        /// <param name="wkhtmltopdfPath">Path to wkthmltopdf.</param>
        /// <param name="html">String containing HTML code that should be converted to PDF.</param>
        /// <returns>PDF as byte array.</returns>
        public byte[] ConvertHtml(string wkhtmltopdfPath, string html)
        {
            return Convert(wkhtmltopdfPath, null, html);
        }

        /// <summary>
        /// Converts given URL to PDF.
        /// </summary>
        /// <param name="wkhtmltopdfPath">Path to wkthmltopdf.</param>
        /// <param name="switches">Switches that will be passed to wkhtmltopdf with URL that should be converted to PDF.</param>
        /// <returns>PDF as byte array.</returns>
        public static byte[] Convert(string wkhtmltopdfPath, string switches)
        {
            return Convert(wkhtmltopdfPath, switches, null);
        }

        /// <summary>
        /// Converts given URL or HTML string to PDF.
        /// </summary>
        /// <param name="wkhtmltopdfPath">Path to wkthmltopdf.</param>
        /// <param name="switches">Switches that will be passed to wkhtmltopdf with URL that should be converted to PDF.</param>
        /// <param name="html">String containing HTML code that should be converted to PDF.
        /// If this variable is set then content of switches variable is ignored.</param>
        /// <returns>PDF as byte array.</returns>
        private static byte[] Convert(string wkhtmltopdfPath, string switches, string html)
        {
            // switches:
            //     "-q"  - silent output, only errors - no progress messages
            //     " -"  - switch output to stdout
            //     "- -" - switch input to stdin and output to stdout
            switches = string.IsNullOrEmpty(html) ? "-q " + switches + " -" : "-q - -";

            var proc = new Process
                           {
                               StartInfo = new ProcessStartInfo
                                               {
                                                   FileName = Path.Combine(wkhtmltopdfPath, "wkhtmltopdf.exe"),
                                                   Arguments = switches,
                                                   UseShellExecute = false,
                                                   RedirectStandardOutput = true,
                                                   RedirectStandardError = true,
                                                   RedirectStandardInput = true,
                                                   WorkingDirectory = wkhtmltopdfPath,
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

            var ms = new MemoryStream();
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
}
