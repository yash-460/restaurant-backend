using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using restaurantUtility.Data;
using restaurantUtility.Models;
using StoreManagementService.Models;

namespace StoreManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly restaurantDBContext _context;

        public CartController(restaurantDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCart()
        {
            var user = User.Identity.Name;
            var items = await _context.Carts.Where(cart => cart.UserName == user).Include(cart => cart.Product).ToListAsync();
            return items;
        }

        [HttpPost]
        public async Task<IActionResult> PostCart(Cart cart)
        {
            string username = User.Identity.Name;

            if(username == null || username.Length == 0)
            {
                return BadRequest();
            }
            cart.UserName = username;
            List<short> stores = await _context.Carts.Where(c => c.UserName == cart.UserName).Include(c => c.Product).Select(c => c.Product.StoreId).Distinct().ToListAsync();

            if(stores.Count != 0)
            {
                var product = await _context.Products.FindAsync(cart.ProductId);
                if (product == null)
                    return BadRequest(new {errorMessage="Product not found"});

                if(stores.FirstOrDefault() != product.StoreId)
                {
                    return Conflict("Cannot add Product of different store to Cart");
                }                   
            }

            bool exists = await _context.Carts.AnyAsync(c => c.UserName == cart.UserName && c.ProductId == cart.ProductId);
            if(exists)
                _context.Entry(cart).State = EntityState.Modified;
            else
                _context.Entry(cart).State = EntityState.Added;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id) {
            string userName = User.Identity.Name;

            var cart = await _context.Carts.FindAsync(userName, id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
