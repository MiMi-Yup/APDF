using APDF.DTOs.Responses.PDF;
using APDF.Models.PDFHandler;

namespace APDF.Core.Interfaces
{
    public interface IPDFHandler : IDisposable
    {
        void AddText(PDFHandler_AddText obj);
        PDF_ExtractInfoResponse ExtractInfo();
    }
}
