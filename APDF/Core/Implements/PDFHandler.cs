using APDF.Core.Interfaces;
using APDF.Helpers;
using APDF.Models.PDFHandler;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace APDF.Core.Implements
{
    public class PDFHandler : IPDFHandler
    {
        public string InputFile { get; private set; }
        public string OutputFile { get; private set; }

        private readonly PdfReader _reader;
        private readonly PdfDocument _readerDocument;
        private readonly PdfWriter _writer;
        private readonly PdfDocument _writerDocument;
        private readonly Document _workingDocument;

        public PDFHandler(string inputFile, string outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;

            if (!File.Exists(inputFile))
                throw new FileNotFoundException();

            _reader = new PdfReader(inputFile);
            _readerDocument = new PdfDocument(_reader);
            _writer = new PdfWriter(outputFile);
            _writerDocument = new PdfDocument(_writer);
            _workingDocument = new Document(_writerDocument, _readerDocument.GetDefaultPageSize());

            ClonePDF();
        }

        public void AddText(PDFHandler_AddText obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            var numberOfPages = _readerDocument.GetNumberOfPages();
            if (numberOfPages < obj.Page)
                throw new ArgumentException($"Page {obj.Page} exceed pages of file {numberOfPages}");

            Text text = new Text(obj.Text);
            text.SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK);

            var paragraph = new Paragraph(text);
            _workingDocument.ShowTextAligned(paragraph,
                UnitHelper.mm2uu(obj.XPosition),
                UnitHelper.mm2uu(obj.YPosition),
                obj.Page,
                obj.TextAlignment ?? TextAlignment.LEFT,
                obj.VerticalAlignment ?? VerticalAlignment.TOP,
                obj.RadAngle);
        }

        #region Private
        private void ClonePDF()
        {
            int numberOfPages = _readerDocument.GetNumberOfPages();
            for (var i = 1; i <= numberOfPages; i++)
            {
                var page = _readerDocument.GetPage(i);
                var newPage = page.CopyTo(_writerDocument);
                _writerDocument.AddPage(newPage);
            }
        }
        #endregion

        #region Dispose
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    using (_reader)
                    using (_writer)
                    using (_readerDocument)
                    using (_writerDocument)
                    using (_workingDocument) { }
                }
                disposed = true;
            }
        }

        ~PDFHandler()
        {
            Dispose(false);
        }
        #endregion
    }
}
