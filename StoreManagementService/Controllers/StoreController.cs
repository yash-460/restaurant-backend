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
    public class StoreController : ControllerBase
    {
        private readonly restaurantDBContext _context;

        public StoreController(restaurantDBContext context)
        {
            _context = context;
        }

        // GET: api/Store
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            return await _context.Stores.Include(s => s.Products).ToListAsync();
        }

        // GET: api/Store/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(short id)
        {
            var store = await _context.Stores.Include(s => s.Products).Where(s => s.StoreId == id).FirstAsync();

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }

        // PUT: api/Store/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutStore(short id, StoreDTO storeDTO)
        {
            if (id != storeDTO.StoreId)
            {
                return BadRequest();
            }

            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                return BadRequest();

            DTOToStore(storeDTO, ref store);

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

        // POST: api/Store
        // TODO: handle creating second store under same userName
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Store>> PostStore(StoreDTO storeDTO)
        {
            User user = await _context.Users.FindAsync(User.Identity.Name);
            if(user == null)
                BadRequest("User not found");

            Store store = new Store();
            DTOToStore(storeDTO, ref store);
            
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            user.StoreId = store.StoreId;
            UserRole userRole = new UserRole{ 
                UserName = user.UserName,
                Role = Constants.STORE_OWNER_ROLE
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetStore", new { id = store.StoreId }, store);
        }

        // DELETE: api/Store/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(short id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void DTOToStore(StoreDTO storeDTO, ref Store store)
        {
            store.Name = storeDTO.Name;
            store.RegistrationNumber = storeDTO.RegistrationNumber;
            store.StreetName = storeDTO.StreetName;
            store.City = storeDTO.City;
            store.Province = storeDTO.Province;
            store.Zip = storeDTO.Zip;
            store.TaxRate = storeDTO.TaxRate;
            store.ImgLoc = storeDTO.ImgLoc;
        }

        private bool StoreExists(short id)
        {
            return _context.Stores.Any(e => e.StoreId == id);
        }
    }
}
