namespace Empresa.Inv.Core.Entities
{
    public class ProductKardex
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int TipoId { get; set; } // 1 o 2 Ingreso o Egreso - AjusteC - ajustesV - Compras - Ventas

        public DateTime Created { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Amount { get; set; }
    }
}
