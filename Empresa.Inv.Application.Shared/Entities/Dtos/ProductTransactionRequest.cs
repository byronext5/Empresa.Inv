namespace Empresa.Inv.Application.Shared.Dtos
{
    public class ProductTransactionRequest
    {
        public int ProductoId { get; set; }
        public decimal Amonunt { get; set; }
        public  int UserId { get; set; }
        public int TypeId { get; set; }

    }
}
