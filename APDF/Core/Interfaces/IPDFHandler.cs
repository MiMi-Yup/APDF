using APDF.Models.PDFHandler;

namespace APDF.Core.Interfaces
{
    public interface IPDFHandler : IDisposable
    {
        void AddText(PDFHandler_AddText obj);
    }
}
