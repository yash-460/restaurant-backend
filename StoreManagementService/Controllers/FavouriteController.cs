using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurantUtility.Data;
using restaurantUtility.Models;
using restaurantUtility.Util;

/**
 * I Yash Chaudhary, 000820480 certify that this material is my original work.
 * No other person's work has been used without due acknowledgement. 
 * I have not made my work available to anyone else.
 */
namespace StoreManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavouriteController : ControllerBase
    {
        private readonly restaurantDBContext _context;

        public FavouriteController(restaurantDBContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<List<int>>> GetFavourite(short id)
        {
            string userName = User.Identity.Name;

            if (id == 0)
                return BadRequest();

            var favouriteProducts = await _context.Favourites.Include(f => f.Product).Where(f => f.UserName == userName && f.Product.StoreId == id).Select(f => f.ProductId).ToListAsync();

            return favouriteProducts;
        }

        [HttpGet("Product")]
        [Authorize]
        public async Task<ActionResult<PaginatedList<Favourite>>> GetFavouriteProduct(int pageIndex = 1, int pageSize = 10)
        {
            var favourites = _context.Favourites.Include(fav => fav.Product).Where(fav => fav.UserName == User.Identity.Name).AsNoTracking();

            return await PaginatedList<Favourite>.CreateAsync(favourites,pageIndex,pageSize);
        }



        [HttpPost("Add/{id}")]
        public async Task<ActionResult> AddToFavourite(int id)
        {
            var alreadyFav = await _context.Favourites.AnyAsync(fav => fav.UserName == User.Identity.Name && fav.ProductId == id);

            if (alreadyFav)
                return BadRequest("Already Favourite");

            await _context.Favourites.AddAsync(new Favourite { 
                UserName = User.Identity.Name,
                ProductId = id
            });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("Remove/{id}")]
        public async Task<ActionResult<List<int>>> RemoveFromFavourite(int id)
        {
            var fav = await _context.Favourites.FindAsync(User.Identity.Name,id);

            if (fav == null)
                return BadRequest("Not favourite");

            _context.Favourites.Remove(fav);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
