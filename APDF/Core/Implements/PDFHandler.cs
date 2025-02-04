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
using System.Collections.Frozen;
using System.Globalization;
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
        private static readonly Regex regex = new Regex(@"\bA\d{1}\b");

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

            var result = RatioScalePaperSize.ConvertFormat(obj.PaperSize,
                obj.ManualPaperSize == null 
                    ? _readerDocument.GetPage(obj.Page).GetPageSize() 
                    : new Rectangle((float)PaperSizeHelper.GetWidth(obj.ManualPaperSize.Value), (float)PaperSizeHelper.GetHeight(obj.ManualPaperSize.Value)),
                new Point(UnitHelper.mm2uu(obj.XPosition), UnitHelper.mm2uu(obj.YPosition)));

            var paragraph = new Paragraph(text);
            _workingDocument?.ShowTextAligned(paragraph,
                (float)result.addPoint.GetX(),
                (float)result.addPoint.GetY(),
                obj.Page,
                obj.TextAlignment ?? TextAlignment.LEFT,
                obj.VerticalAlignment ?? VerticalAlignment.TOP,
                obj.RadAngle + result.offsetRadAngle);
        }

        public string GetProducer { get => _readerDocument.GetDocumentInfo().GetProducer(); }

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

        public PDF_ReadPOResponse ReadPO(int[] nos)
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
            List<(int startIndex, int endIndex)> skipLines = new List<(int startIndex, int endIndex)>();
            int startIndex = -1, endIndex = -1;
            for (int index = 0; index < extractedContent.Length; index++)
            {
                if (extractedContent[index].Contains("<Send_mail>", StringComparison.InvariantCultureIgnoreCase))
                    startIndex = index;
                else if (extractedContent[index].Equals(".", StringComparison.InvariantCultureIgnoreCase))
                    endIndex = index + 1;

                if (startIndex != -1 && endIndex != -1)
                {
                    skipLines.Add((startIndex > endIndex ? endIndex : startIndex, endIndex > startIndex ? endIndex : startIndex));
                    startIndex = -1;
                    endIndex = -1;
                }
            }

            List<string> contentLines = new List<string>();

            foreach (var segment in skipLines)
            {
                contentLines.AddRange(extractedContent.Skip(segment.startIndex).Take(segment.endIndex - segment.startIndex));
            }

            string[] contentArray = contentLines.ToArray();
            List<(int startIndex, int endIndex)> segments = new List<(int startIndex, int endIndex)>();
            bool hasJobCC = contentArray.Any(item => item.Contains("JOB/CC", StringComparison.InvariantCultureIgnoreCase));
            for (int index = 0; index < contentArray.Length; index++)
            {
                if (contentArray[index].Contains(hasJobCC ? "JOB/CC" : "TRACK LABEL", StringComparison.InvariantCultureIgnoreCase) && startIndex == -1)
                    startIndex = index;
                else if (contentArray[index].Contains("UNIT WEIGHT", StringComparison.InvariantCultureIgnoreCase))
                {
                    endIndex = index - 1;
                }

                if (startIndex != -1 && endIndex != -1)
                {
                    segments.Add((startIndex > endIndex ? endIndex : startIndex, endIndex > startIndex ? endIndex : startIndex));
                    startIndex = -1;
                    endIndex = -1;
                }
            }
            if (startIndex > -1 || endIndex > -1)
                segments.Add((startIndex > endIndex ? startIndex : endIndex, contentArray.Length - 1));

            var result = new List<PDF_ReadPO>();
            CultureInfo enUSCulture = CultureInfo.GetCultureInfo("en-US");
            foreach (var segment in segments)
            {
                var info = contentArray[segment.startIndex + 1].Split(" ");
                var revCC = contentArray[segment.startIndex + 2].Split(" ");

                var colorCode = string.Empty;
                for (int index = segment.startIndex; index <= segment.endIndex; index++)
                {
                    if (contentArray[index].Contains("Cycle-RAL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        colorCode = contentArray[index].Split(":").Last().Trim();
                        if (!colorCode.EndsWith("\""))
                            colorCode += contentArray[index + 1].Trim();
                        colorCode = colorCode.Replace("\"", string.Empty).Trim();
                        break;
                    }
                }

                result.Add(new PDF_ReadPO
                {
                    Job = hasJobCC ? contentArray[segment.startIndex].Split(":").Last().Trim() : null,
                    NO = int.Parse(info[0]),
                    CodeDWG = info[1],
                    UOM = info[info.Length - 2],
                    Quantity = double.Parse(info[info.Length - 3], enUSCulture),
                    Revise = revCC[0],
                    CC = revCC.Length >= 2 ? revCC.Skip(1).FirstOrDefault(item => Regex.IsMatch(item.Trim(), @"^[a-zA-Z].*\d$"))?.Trim() ?? "INVALID" : null,
                    ColorCode = string.IsNullOrEmpty(colorCode) ? null : colorCode
                });
            }

            IDictionary<int, PDF_ReadPO> results = new Dictionary<int, PDF_ReadPO>();
            var resultMap = result.ToFrozenDictionary(item => item.NO, item => item);
            foreach (var no in nos.ToHashSet())
                if (resultMap.ContainsKey(no))
                    results.Add(no, resultMap[no]);

            return new PDF_ReadPOResponse { Informations = results };
        }

        ~PDFHandler()
        {
            Dispose(false);
        }
        #endregion
    }
}
