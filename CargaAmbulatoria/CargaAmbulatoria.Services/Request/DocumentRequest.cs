
namespace CargaAmbulatoria.Services.Request
{
    public class DocumentRequest
    {
        public string DniType { get; set; }
        public string Dni { get; set; }
        public string DocumentFile { get; set; }
        public string Regime { get; set; }
        public long CohortId { get; set; }
        public string UserId { get; set; }
    }
}
