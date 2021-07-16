using BookCart.Dto;
using BookCart.Models;
using System.Collections.Generic;

namespace BookCart.Interfaces
{
    public interface IProductService
    {
        List<Product> GetAllBooks();
        int AddBook(Product book);
        int UpdateBook(Product book);
        Product GetBookData(int bookId);
        string DeleteBook(int bookId);
        List<Categories> GetCategories();
        List<Product> GetSimilarBooks(int bookId);
        List<CartItemDTO> GetBooksAvailableInCart(string cartId);
        List<Product> GetBooksAvailableInWishlist(string wishlistID);
    }
}
