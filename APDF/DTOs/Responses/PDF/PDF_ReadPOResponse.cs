namespace APDF.DTOs.Responses.PDF
{
    public class PDF_ReadPOResponse
    {
        public IDictionary<int, PDF_ReadPO> Informations { get; set; }
    }

    public class PDF_ReadPO
    {
        public int NO { get; set; }

        public string CodeDWG { get; set; }

        public string? Job { get; set; }

        public string UOM { get; set; }

        public double Quantity { get; set; }

        public string Revise { get; set; }

        public string? CC { get; set; }

        public string? ColorCode { get; set; }
    }
}
