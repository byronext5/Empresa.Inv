using AutoMapper;
using Empresa.Inv.Application.Shared.Dtos;
using Empresa.Inv.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;


namespace Empresa.Inv.EntityFramework.Core.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ProductRepository(ApplicationDbContext context, IUnitOfWork unitOfWork, IMapper mapper) : base(context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;   
        }


        // Método específico para ejecutar un procedimiento almacenado en el contexto de Product
        public async Task<IEnumerable<Product>> GetProductsPagedAsyncSp(
      string searchTerm, int pageNumber, int pageSize
      )
        {


            var parameters = new[]
            {
          new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 128) { Value = searchTerm ?? (object)DBNull.Value },
          new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
          new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }
      };


            // Ejecutar un procedimiento almacenado que devuelve resultados
            var products = await ExecuteStoredProcedureWithResultsAsync("EXEC GetProductsPaged @SearchTerm,@PageNumber,@PageSize", parameters);

            return products;
            // Trabaja con los resultados
        }




        public async Task<IEnumerable<Product>> GetProductsPagedAsyncEf(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            // Aplicar filtrado
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Aplicar paginación
            var products = await query
                .OrderBy(p => p.Name) // Ordenar por algún criterio
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products;
        }


        public async Task<ProductDTO> GetProductsDetailsByIdAsync(int id)
        {
            // Obtener el producto y los datos relacionados en una sola consulta
            var productDto = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                    SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                })
                .FirstOrDefaultAsync();

            return productDto;
        }


        public async Task<ProductDTO> GetProductsDetailsByIdMapperAsync(int id)
        {
            // Obtener el producto y los datos relacionados en una sola consulta
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado                
                .FirstOrDefaultAsync();
            if (product == null) {
                return new ProductDTO();
            }

            // Mapear el producto a ProductDTO usando AutoMapper
            var productDto = _mapper.Map<ProductDTO>(product);

            return productDto;
        }

        public async Task<Boolean> UpdateInventAsync(int productId, decimal Amount, int userId, int typeId) 
        {
            bool result = false;
            // validaciones generales 

            if(Amount <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor a cero");
            }
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // primero insertar el kardex
                var kardexEntry = new ProductKardex
                {
                    ProductId = productId,
                    Amount = Amount,
                    UserId = userId,
                    Created = DateTime.UtcNow,
                    TipoId = typeId
                };
                // agregar el kardex insertar el contexto
                await _context.ProductKardexs.AddAsync(kardexEntry);

                // actualizar el balance
                // busca el registro que totaliza ese producto
                // busca el balance actual del producto
                var productBalance = await _context.ProductBalances
                    .Where(pb=> pb.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (productBalance != null)
                {                    
                    switch (typeId)
                    {
                        case 1:
                            productBalance.Amount += Amount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;
                        case 2:
                            productBalance.Amount -= Amount;
                            productBalance.UserId = userId;
                            productBalance.Created = DateTime.UtcNow;
                            break;
                        default:
                            break;
                    }
                    _context.ProductBalances.Update(productBalance); // marca la entidad para actualizar 

                }
                else
                { 
                    productBalance = new ProductBalance 
                    { 
                        ProductId = productId,
                        Amount = Amount,
                        UserId = userId,
                        Created = DateTime.UtcNow,
                    };
                    await _context.ProductBalances.AddAsync(productBalance);
                }

                await _unitOfWork.SaveAsync(); 
                // guarda los camvios en productkardex y productbalance 
                await _unitOfWork.CommitTransactionAsync();
                result = true;
            }
            catch (Exception exx) 
            {
                await _unitOfWork.RollbacktransactionAsync();
                result = false;
                throw;
            }

            // insercion al kardex

            // actualizacion al balance 
            // actualizacion del product
            return result;
        }

        // NUEVO METODOD 

        public async Task<List<UserKardexSummaryDto>> getKardexSummaryByUserAsync(DateTime startDate, DateTime endDate)
        {

            var result = await _context.ProductKardexs
                .Where(k => k.Created >= startDate && k.Created <= endDate)
                .GroupBy(k => k.UserId)
                .Select(g => new UserKardexSummaryDto
                {
                    UserId = g.Key,
                    CantidadMovimientos = g.Count(),
                    TotalIngresos = g.Sum(k => k.TipoId == 1 ? k.Amount : 0),
                    TotalEgresos = g.Sum(k => k.TipoId == 2 ? k.Amount : 0)
                })
                .ToListAsync();

            
            return result;
        }

    }
}
