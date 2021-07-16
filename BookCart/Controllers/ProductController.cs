using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookCart.Interfaces;
using BookCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BookCart.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        readonly IWebHostEnvironment _hostingEnvironment;
        readonly IProductService _bookService;
        readonly IConfiguration _config;
        readonly string coverImageFolderPath = string.Empty;

        public ProductController(IConfiguration config, IWebHostEnvironment hostingEnvironment, IProductService productService)
        {
            _config = config;
            _bookService = productService;
            _hostingEnvironment = hostingEnvironment;
            coverImageFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Upload");
            if (!Directory.Exists(coverImageFolderPath))
            {
                Directory.CreateDirectory(coverImageFolderPath);
            }
        }

        /// <summary>
        /// Get the list of available books
        /// </summary>
        /// <returns>List of Book</returns>
        [HttpGet]
        public async Task<List<Product>> Get()
        {
            return await Task.FromResult(_bookService.GetAllBooks()).ConfigureAwait(true) ;
        }

        /// <summary>
        /// Get the specific book data corresponding to the BookId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Product book = _bookService.GetBookData(id);
            if(book!=null)
            {
                return Ok(book);
            }
            return NotFound();
        }

        /// <summary>
        /// Get the list of available categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCategoriesList")]
        public async Task<IEnumerable<Categories>> CategoryDetails()
        {
            return await Task.FromResult(_bookService.GetCategories()).ConfigureAwait(true) ;
        }

        /// <summary>
        /// Get the random five books from the category of book whose BookId is supplied
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSimilarBooks/{bookId}")]
        public async Task<List<Product>> SimilarBooks(int bookId)
        {
            return await Task.FromResult(_bookService.GetSimilarBooks(bookId)).ConfigureAwait(true) ;
        }

        /// <summary>
        /// Add a new book record
        /// </summary>
        /// <returns></returns>
        [HttpPost, DisableRequestSizeLimit]
        [Authorize(Policy = UserRoles.Admin)]
        public int Post()
        {
            Product book = JsonConvert.DeserializeObject<Product>(Request.Form["bookFormData"].ToString());

            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];

                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(coverImageFolderPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    book.CoverFileName = fileName;
                }
            }
            else
            {
                book.CoverFileName = _config["DefaultCoverImageFile"];
            }
            return _bookService.AddBook(book);
        }

        /// <summary>
        /// Update a particular book record
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = UserRoles.Admin)]
        public int Put()
        {
            Product book = JsonConvert.DeserializeObject<Product>(Request.Form["bookFormData"].ToString());
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];

                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(coverImageFolderPath, fileName);
                    bool isFileExists = Directory.Exists(fullPath);

                    if (!isFileExists)
                    {
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        book.CoverFileName = fileName;
                    }
                }
            }
            return _bookService.UpdateBook(book);
        }

        /// <summary>
        /// Delete a particular book record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = UserRoles.Admin)]
        public int Delete(int id)
        {
            string coverFileName = _bookService.DeleteBook(id);
            if (coverFileName != _config["DefaultCoverImageFile"])
            {
                string fullPath = Path.Combine(coverImageFolderPath, coverFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            return 1;
        }
    }
}
