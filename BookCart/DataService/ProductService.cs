using BookCart.Dto;
using BookCart.Interfaces;
using BookCart.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookCart.DataAccess
{
    public class ProductService : IProductService
    {
        readonly SravyEcommerceDBContext _dbContext;

        public ProductService(SravyEcommerceDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Product> GetAllBooks()
        {
            try
            {
                return _dbContext.Product.AsNoTracking().ToList();
            }
            catch
            {
                throw;
            }
        }

        public int AddBook(Product book)
        {
            try
            {
                _dbContext.Product.Add(book);
                _dbContext.SaveChanges();

                return 1;
            }
            catch
            {
                throw;
            }
        }

        public int UpdateBook(Product book)
        {
            try
            {
                Product oldBookData = GetBookData(book.ProductId);

                if (oldBookData.CoverFileName != null)
                {
                    if (book.CoverFileName == null)
                    {
                        book.CoverFileName = oldBookData.CoverFileName;
                    }
                }

                _dbContext.Entry(book).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return 1;
            }
            catch
            {
                throw;
            }
        }

        public Product GetBookData(int bookId)
        {
            try
            {
                Product book = _dbContext.Product.FirstOrDefault(x => x.ProductId == bookId);
                if (book != null)
                {
                    _dbContext.Entry(book).State = EntityState.Detached;
                    return book;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public string DeleteBook(int bookId)
        {
            try
            {
                Product book = _dbContext.Product.Find(bookId);
                _dbContext.Product.Remove(book);
                _dbContext.SaveChanges();

                return (book.CoverFileName);
            }
            catch
            {
                throw;
            }
        }

        public List<Categories> GetCategories()
        {
            List<Categories> lstCategories = new List<Categories>();
            lstCategories = (from CategoriesList in _dbContext.Categories select CategoriesList).ToList();

            return lstCategories;
        }

        public List<Product> GetSimilarBooks(int bookId)
        {
            List<Product> lstBook = new List<Product>();
            Product book = GetBookData(bookId);

            lstBook = _dbContext.Product.Where(x => x.Category == book.Category && x.ProductId != book.ProductId)
                .OrderBy(u => Guid.NewGuid())
                .Take(5)
                .ToList();
            return lstBook;
        }

        public List<CartItemDTO> GetBooksAvailableInCart(string cartID)
        {
            try
            {
                List<CartItemDTO> cartItemList = new List<CartItemDTO>();
                List<CartItems> cartItems = _dbContext.CartItems.Where(x => x.CartId == cartID).ToList();

                foreach (CartItems item in cartItems)
                {
                    Product book = GetBookData(item.ProductId);
                    CartItemDTO objCartItem = new CartItemDTO
                    {
                        Book = book,
                        Quantity = item.Quantity
                    };

                    cartItemList.Add(objCartItem);
                }
                return cartItemList;
            }
            catch
            {
                throw;
            }
        }

        public List<Product> GetBooksAvailableInWishlist(string wishlistID)
        {
            try
            {
                List<Product> wishlist = new List<Product>();
                List<WishlistItems> cartItems = _dbContext.WishlistItems.Where(x => x.WishlistId == wishlistID).ToList();

                foreach (WishlistItems item in cartItems)
                {
                    Product book = GetBookData(item.ProductId);
                    wishlist.Add(book);
                }
                return wishlist;
            }
            catch
            {
                throw;
            }
        }
    }
}
