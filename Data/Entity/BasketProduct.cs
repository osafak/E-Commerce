using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class BasketProduct
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public Basket Basket { get; set; }
    }
}
