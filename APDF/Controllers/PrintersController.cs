using Microsoft.AspNetCore.Mvc;
using System.Printing;

namespace APDF.Controllers
{
    [ApiController]
    public class PrintersController : ControllerBase
    {
        private readonly ILogger _logger;

        public PrintersController(ILogger<PrintersController> logger)
        {
            _logger = logger;
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult QueuePrint(string printerName)
        {
            using (PrintServer server = new PrintServer())
            using (PrintQueue queue = server.GetPrintQueue(printerName))
            {
                return Ok(new
                {
                    QueueStatus = queue.QueueStatus.ToString(),
                    Queues = queue.GetPrintJobInfoCollection().Select(jobInfo => new
                    {
                        jobInfo.JobIdentifier,
                        jobInfo.Name,
                        JobStatus = jobInfo.JobStatus.ToString(),
                        jobInfo.PositionInPrintQueue,
                        jobInfo.NumberOfPages,
                        jobInfo.NumberOfPagesPrinted,
                        jobInfo.TimeJobSubmitted
                    })
                });
            }
        }
    }
}
