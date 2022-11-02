using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurantUtility.Data;
using restaurantUtility.Models;
using StoreManagementService.Models;

namespace StoreManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly restaurantDBContext _context;

        public ProductController(restaurantDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(ProductDTO productDTO)
        {
            Product product = new Product
            {
                ProductName = productDTO.ProductName,
                StoreId = productDTO.StoreId,
                Description = productDTO.Description,
                Price = productDTO.Price,
                ImgLoc = productDTO.ImgLoc,
                Active = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return  product;
        }


    }
}
