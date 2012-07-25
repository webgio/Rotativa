using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Rotativa.Tests
{
    public class PdfTester
    {
        private byte[] pdfContent;
        private string pdfText;
        private PdfReader pdfReader;

        public bool PdfIsValid { get; set; }

        //public void LoadPdf(RequestResult pdfresult)
        //{
        //    try
        //    {
        //        byte[] pdfcontent = System.Text.Encoding.UTF8.GetBytes(pdfresult.ResponseText);
        //        this.pdfReader = new iTextSharp.text.pdf.PdfReader(pdfcontent);
        //        var parser = new PDFParser();
        //        var parsed = parser.ExtractTextFromPDFBytes(pdfcontent);
        //        this.PdfIsValid = true;
        //    }
        //    catch (iTextSharp.text.exceptions.InvalidPdfException)
        //    {
        //        this.PdfIsValid = false;
        //    }
        //}

        public void LoadPdf(byte[] pdfcontent)
        {
            try
            {
                this.pdfReader = new PdfReader(pdfcontent);
                var parser = new PDFParser();
                var parsed = parser.ExtractTextFromPDFBytes(pdfcontent);
                this.PdfIsValid = true;
            }
            catch (InvalidPdfException)
            {
                this.PdfIsValid = false;
            }
        }

        /// <summary>
        /// Not really useful since wkhtmltopdf build an image pdf...
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool PdfContains(string text)
        {
            //var textbuilder = new StringBuilder();
            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                //textbuilder.Append(currentText);
                if (currentText.Contains(text))
                    return true;
                pdfReader.Close();
            }
            return false;
        }
    }
}
