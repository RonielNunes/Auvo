using Application.Application.Models;
using Entity.Entities;

namespace Application.Application.Interfaces
{
    public interface IFolhaDePagamentoApplication
	{
        public Task<FolhaDePagamento> Processar(List<FolhaDePonto> folhasDePonto, Arquivo arquivo);
    }
}
