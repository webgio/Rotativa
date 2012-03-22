using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Rotativa
{
    public class WkhtmltopdfDriver
    {
        private string cookieName;
        private string cookieValue;

        public byte[] ConvertHtml(string wkhtmltopdfPath, string html)
        {
            string switches = " ";
            switches += "- -";

            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
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
            using (var stream = proc.StandardInput)
            {
                stream.WriteLine(html);
            }
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            if (String.IsNullOrEmpty(output))
            {
                throw new Exception(error);
            }
            proc.WaitForExit();
            byte[] buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
            return buffer;
        }

        public static byte[] Convert(string wkhtmltopdfPath, string switches)
        {
            // adding the switch to ouput on stdout
            switches += " -";
            var proc = new Process()
                           {
                               StartInfo = new ProcessStartInfo()
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
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            if (String.IsNullOrEmpty(output))
            {
                throw new Exception(error);
            }
            proc.WaitForExit();
            byte[] buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
            return buffer;
        }
    }
}
