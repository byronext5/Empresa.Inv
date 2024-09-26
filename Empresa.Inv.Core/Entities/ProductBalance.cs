﻿namespace Empresa.Inv.Core.Entities
{
    public class ProductBalance
    {
        public int Id { get; set; }
        public int UserId { get; set; }  
        public User User { get; set; } 

        public DateTime Created { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Amount { get; set; }

    }
}
