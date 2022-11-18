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

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(int id, ProductDTO productDTO)
        {
            if (id != productDTO.ProductId)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return BadRequest();

            DTOToProduct(productDTO,product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        private void DTOToProduct(ProductDTO productDTO, Product product)
        {
            product.ProductName = productDTO.ProductName;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.ImgLoc = productDTO.ImgLoc;
            product.Active = productDTO.Active;
        }

    }
}
