using APDF.Core.Interfaces;
using APDF.DTOs.Responses.PDF;
using APDF.Helpers;
using APDF.Models.PDFHandler;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Text;
using System.Text.RegularExpressions;

namespace APDF.Core.Implements
{
    internal class PDFHandler : IPDFHandler
    {
        public string InputFile { get; private set; }
        public string OutputFile { get; private set; }
        public bool IsReadOnly { get; private set; }

        private readonly PdfReader _reader;
        private readonly PdfDocument _readerDocument;
        private readonly PdfWriter? _writer;
        private readonly PdfDocument? _writerDocument;
        private readonly Document? _workingDocument;
        private static readonly Regex regex = new Regex(@"\bA\d+\b");

        public PDFHandler(string inputFile, string outputFile = "", bool readOnly = false)
        {
            InputFile = inputFile;
            IsReadOnly = readOnly;
            if (!IsReadOnly && string.IsNullOrEmpty(outputFile))
                throw new ArgumentNullException(nameof(outputFile));

            if (!File.Exists(InputFile))
                throw new FileNotFoundException(nameof(InputFile));

            _reader = new PdfReader(InputFile);
            _readerDocument = new PdfDocument(_reader);

            OutputFile = outputFile;
            if (!IsReadOnly)
            {
                _writer = new PdfWriter(OutputFile);
                _writerDocument = new PdfDocument(_writer);
                _workingDocument = new Document(_writerDocument, _readerDocument.GetDefaultPageSize());

                ClonePDF();
            }
        }

        public void AddText(PDFHandler_AddText obj)
        {
            if (IsReadOnly) return;

            if (obj == null)
                throw new ArgumentNullException();

            var numberOfPages = _readerDocument.GetNumberOfPages();
            if (numberOfPages < obj.Page)
                throw new ArgumentException($"Page {obj.Page} exceed pages of file {numberOfPages}");

            Text text = new Text(obj.Text.Replace("(newline)", "\n"));
            text.SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK);
            text.SetFontSize(obj.FontSize);

            var positionAdd = RatioScalePaperSize.ConvertFormat(obj.PaperSize,
                _readerDocument.GetPage(obj.Page).GetPageSize(),
                new Rectangle(UnitHelper.mm2uu(obj.XPosition), UnitHelper.mm2uu(obj.YPosition), 0, 0));

            var paragraph = new Paragraph(text);
            _workingDocument?.ShowTextAligned(paragraph,
                positionAdd.GetX(),
                positionAdd.GetY(),
                obj.Page,
                obj.TextAlignment ?? TextAlignment.LEFT,
                obj.VerticalAlignment ?? VerticalAlignment.TOP,
                obj.RadAngle);
        }

        public PDF_ExtractInfoResponse ExtractInfo()
        {
            int pages = _readerDocument.GetNumberOfPages();
            StringBuilder content = new StringBuilder();
            for (int i = 1; i <= pages; i++)
            {
                var strategy = new LocationTextExtractionStrategyV2();
                strategy.SetUseActualText(true);
                content.Append(PdfTextExtractor.GetTextFromPage(_readerDocument.GetPage(i), strategy));
            }
            string[] extractedContent = content.ToString().Split('\n');

            var result = new PDF_ExtractInfoResponse();
            var found = extractedContent.FirstOrDefault(item => item.Contains(nameof(result.Format)));
            if (!string.IsNullOrEmpty(found))
            {
                int index = Array.IndexOf(extractedContent, found);
                for (; index < extractedContent.Length; index++)
                {
                    Match match = regex.Match(extractedContent[index]);
                    if (match.Success)
                    {
                        result.Format = match.Value;
                        break;
                    }
                }
            }

            return result;
        }

        public int ExtractInfoFromPaperSize()
        {
            var pageSize = _readerDocument.GetPage(1).GetPageSize();
            return PaperSizeHelper.GetSize(pageSize.GetWidth(), pageSize.GetHeight());
        }

        #region Private
        private void ClonePDF()
        {
            if (IsReadOnly) return;
            int numberOfPages = _readerDocument.GetNumberOfPages();
            for (var i = 1; i <= numberOfPages; i++)
            {
                var page = _readerDocument.GetPage(i);
                var newPage = page.CopyTo(_writerDocument);
                _writerDocument?.AddPage(newPage);
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
                    using (_readerDocument)
                        if (!IsReadOnly)
                        {
                            using (_writer)
                            using (_writerDocument)
                            using (_workingDocument) { }
                        }
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
