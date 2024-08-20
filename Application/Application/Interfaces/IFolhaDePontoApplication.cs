using Entity.Entities;

namespace Application.Application.Interfaces
{
    public interface IFolhaDePontoApplication
    {
        public Task<List<FolhaDePonto>> CarregarFolhaDePonto();
    }
}
