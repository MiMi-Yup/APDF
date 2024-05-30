using APDF.Core.Implements;
using APDF.DTOs.Requests.PDF;
using Microsoft.AspNetCore.Mvc;

namespace APDF.Controllers
{
    [ApiController]
    public class PDFsController : ControllerBase
    {
        private readonly ILogger _logger;

        public PDFsController(ILogger<PDFsController> logger)
        {
            _logger = logger;
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult AddText(PDF_AddTextRequest obj)
        {
            using (var pdfHandler = new PDFHandler(obj.InputFile, obj.OutputFile))
            {
                foreach (var item in obj.Details)
                {
                    pdfHandler.AddText(new Models.PDFHandler.PDFHandler_AddText
                    {
                        Page = item.Page,
                        Text = item.Text,
                        XPosition = item.XPosition,
                        YPosition = item.YPosition,
                        PaperSize = obj.PaperSize,
                        RadAngle = item.RadAngle,
                        FontSize = item.FontSize
                    });
                }
            }

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult ExtractInfo(PDF_ExtractInfoRequest obj)
        {
            using (var pdfHandler = new PDFHandler(obj.FilePath, readOnly: true))
            {
                var result = pdfHandler.ExtractInfo();
                return string.IsNullOrEmpty(result.Format) ? NotFound() : Ok(result.Format);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult ExtractInfoFromPaperSize(PDF_ExtractInfoRequest obj)
        {
            using (var pdfHandler = new PDFHandler(obj.FilePath, readOnly: true))
            {
                var result = pdfHandler.ExtractInfoFromPaperSize();
                return result == -1 ? NotFound() : Ok($"A{result}");
            }
        }
    }
}
