

using Empresa.Inv.Application.Shared.Dtos;
using Empresa.Inv.Core.Entities;

namespace Empresa.Inv.EntityFramework.Core
{
    public interface IProductCustomRepository : IRepository<Product>
    {

        // Métodos adicionales si es necesario
        Task<IEnumerable<ProductDTO>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize);


    }
}
