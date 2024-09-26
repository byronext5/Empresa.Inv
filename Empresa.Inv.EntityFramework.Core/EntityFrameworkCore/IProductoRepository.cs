
using Empresa.Inv.Application.Shared.Dtos;
using Empresa.Inv.Core.Entities;

namespace Empresa.Inv.EntityFramework.Core
{
    public interface IProductRepository : IRepository<Product>
    {

        // Métodos adicionales si es necesario
        Task<IEnumerable<Product>> GetProductsPagedAsyncSp(string searchTerm, int pageNumber, int pageSize);

        Task<IEnumerable<Product>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize);

        Task<ProductDTO> GetProductsDetailsByIdAsync(int id);

        Task<ProductDTO> GetProductsDetailsByIdMapperAsync(int id);

        Task<Boolean> UpdateInventAsync(int productId, decimal Amount, int userId, int typeId);

        Task<List<UserKardexSummaryDto>> getKardexSummaryByUserAsync(DateTime startDate, DateTime endDate);
    }
}
