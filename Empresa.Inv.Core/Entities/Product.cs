﻿namespace Empresa.Inv.Core.Entities
{
    //1 ENTIDAD 
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        // Nueva propiedad para la relación con ProductBalance
        public ICollection<ProductBalance> ProductBalances { get; set; }
        public ICollection<ProductKardex> ProductKardexs { get; set; }

    }    

}
