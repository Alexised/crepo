

namespace CargaAmbulatoria.EntityFramework.Interfaces
{
    public interface IDocument
    {
        long DocumentId { get; }
        string Name { get; set; }
        string Path { get; set; }
        DateTime DocumentDate { get; set; }
    }
}
