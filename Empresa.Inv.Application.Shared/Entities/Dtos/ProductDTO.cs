
namespace Empresa.Inv.Application.Shared.Dtos
{
    public class ProductDTO
    {
        
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }

            public string CategoryName { get; set; }
            public string SupplierName { get; set; }

        public int CategoryId { get; set; }
        public int SupplierId { get; set; }


    }
}
