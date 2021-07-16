using BookCart.Models;

namespace BookCart.Dto
{
    public class CartItemDTO
    {
        public Product Book { get; set; }
        public int Quantity { get; set; }
    }
}
