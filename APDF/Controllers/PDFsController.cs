using APDF.Core.Implements;
using APDF.DTOs.Requests;
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
        public IActionResult AddText(PDF_AddText obj)
        {
            using (var pdfHandler = new PDFHandler(obj.InputFile, obj.OutputFile))
            {
                pdfHandler.AddText(new Models.PDFHandler.PDFHandler_AddText { Page = obj.Page, Text = obj.Text, XPosition = obj.XPosition, YPosition = obj.YPosition });
            }

            return Ok();
        }
    }
}
