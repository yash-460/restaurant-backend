﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using restaurantUtility.Data;
using restaurantUtility.Models;
using restaurantUtility.Util;
using StoreManagementService.Models;

/**
 * I Yash Chaudhary, 000820480 certify that this material is my original work.
 * No other person's work has been used without due acknowledgement. 
 * I have not made my work available to anyone else.
 */
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
        public async Task<ActionResult<PaginatedList<Store>>> GetStores(string? search, int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<Store> stores = _context.Stores.AsNoTracking();
            if (search != null && search.Trim().Length != 0)
                stores = stores.Where(s => s.Name.ToLower().Contains(search.ToLower())).AsNoTracking();

            return await PaginatedList<Store>.CreateAsync(stores,pageIndex,pageSize);
        }

        [HttpGet("tax/{id}")]
        public async Task<ActionResult<decimal>> GetStoreTax(short id)
        {
            return await _context.Stores.Where(s => s.StoreId == id).Select(store => store.TaxRate).FirstOrDefaultAsync();
        }

        // GET: api/Store/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(short id, bool? active )
        {
            Store store = await _context.Stores.Include(s => s.Products).Where(s => s.StoreId == id).FirstAsync();
            if (active == null || active.Value)
            {
                store.Products = store.Products.Where(p => p.Active).ToList();
            }

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
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                User user = await _context.Users.FindAsync(User.Identity.Name);
                if (user == null)
                    return BadRequest("User not found");

                Store store = new Store();
                DTOToStore(storeDTO, ref store);

                _context.Stores.Add(store);
                await _context.SaveChangesAsync();

                user.StoreId = store.StoreId;
                UserRole userRole = new UserRole
                {
                    UserName = user.UserName,
                    Role = Constants.STORE_OWNER_ROLE
                };
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                dbContextTransaction.Commit();
                return CreatedAtAction("GetStore", new { id = store.StoreId }, store);
            }
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

        [HttpGet("Report/")]
        [Authorize(Roles ="owner")]
        public async Task<ActionResult<List<Report>>> GetReport(short? storeId, DateTime? startDate, DateTime? endDate)
        {
            if (storeId == null && startDate == null && endDate == null)
                return BadRequest();

            var query = "select od.product_id, SUM(od.quantity), SUM(od.quantity * od.price) ,SUM((o.tax * od.price * od.quantity)/100) from Orders as o, order_details as od where od.order_id = o.order_id AND Date(o.ordered_time) >= @startDate and Date(o.ordered_time) <= @endDate and o.store_id = @StoreId Group By od.product_id";
            List<Report> reports = new List<Report>();
            MySqlConnection conn = null;

            try
            {
                conn  = new MySqlConnection(_context.Database.GetConnectionString());
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@StoreId", storeId);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reports.Add(new Report
                            {
                                ProductId = reader.GetInt32(0),
                                Quantity = reader.GetInt32(1),
                                TotalAmount = Math.Round(reader.GetDecimal(2),2),
                                TotalTax = Math.Round(reader.GetDecimal(3),2)
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                if(conn != null)
                    conn.Close();
            }

            var products = await _context.Products.Where(p => p.StoreId == storeId).Select(p => new { productId = p.ProductId, productname = p.ProductName }).ToListAsync();

            foreach (Report report in reports)
                report.ProductName = products.Find(a => a.productId == report.ProductId).productname;

            return reports;
        }

       

        private bool StoreExists(short id)
        {
            return _context.Stores.Any(e => e.StoreId == id);
        }
    }
}
