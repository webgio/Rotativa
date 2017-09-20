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
        public Exception PdfException { get; set; }

        public void LoadPdf(byte[] pdfcontent)
        {
            try
            {
                this.pdfReader = new PdfReader(pdfcontent);
                var parser = new PDFParser();
                var parsed = parser.ExtractTextFromPDFBytes(pdfcontent);
                this.PdfIsValid = true;
            }
            catch (InvalidPdfException ex)
            {
                this.PdfException = ex;
                this.PdfIsValid = false;
            }
        }

        public bool PdfContains(string text)
        {
            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                if (currentText.Contains(text))
                    return true;
                pdfReader.Close();
            }
            return false;
        }
    }
}
