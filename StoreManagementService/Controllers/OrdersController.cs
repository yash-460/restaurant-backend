using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using restaurantUtility.Data;
using restaurantUtility.Models;
using restaurantUtility.Util;
using StoreManagementService.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
    public class OrdersController : ControllerBase
    {
        private readonly restaurantDBContext _context;

        public OrdersController(restaurantDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int pageIndex = 1, int pageSize = 10,DateTime? startDate = null, DateTime? endDate = null)
        {
            var orders = _context.Orders.Where(o => o.UserName == User.Identity.Name).Include(o => o.OrderDetails).OrderByDescending(o => o.OrderedTime).AsNoTracking();
            if (startDate != null && endDate != null)
                if (startDate > endDate)
                    return BadRequest("startdate and endDate filter not valid");
            if (startDate != null)
                orders = orders.Where(o => o.OrderedTime.Date >= startDate.Value.Date);
            if (endDate != null)
                orders = orders.Where(o => o.OrderedTime.Date <= endDate.Value.Date);

            var paginatedList = await PaginatedList<Order>.CreateAsync(orders,pageIndex,pageSize);

            // populating with related entities without fetching all columns in them (Mainly to avoid large Image)
            HashSet<short> storeIds = new HashSet<short>();
            paginatedList.Items.ForEach(o => storeIds.Add(o.StoreId));
            var stores = await _context.Stores.Where(s => storeIds.Contains(s.StoreId)).Select(s => new { storeId = s.StoreId, name = s.Name}).ToListAsync();

            HashSet<int> productIds = new HashSet<int>();
            paginatedList.Items.ForEach(o => { 
                o.OrderDetails.ToList().ForEach(od => productIds.Add(od.ProductId));
            });
            var products = await _context.Products.Where(p => productIds.Contains(p.ProductId)).Select(p => new {productIds = p.ProductId, productName = p.ProductName}).ToListAsync();

            paginatedList.Items.ForEach(o => {
                o.Store = new Store
                {
                    StoreId = o.StoreId,
                    Name = stores.Find(s => s.storeId == o.StoreId).name
                };
                o.OrderDetails.ToList().ForEach(od => {
                    od.Product = new Product
                    {
                        ProductId = od.ProductId,
                        ProductName = products.Find(p => p.productIds == od.ProductId).productName
                    };
                });
            });

            return Ok(paginatedList);
        }

        [HttpGet("Store/{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetStoreOrders(short id)
        {
            List<Order> orders =  await _context.Orders.Include(o => o.OrderDetails).Where(o => o.StoreId == id && o.Status == Constants.ORDER_IN_PROGRESS).OrderBy(o => o.OrderedTime).ToListAsync();
            foreach(Order order in orders)
            {
                foreach (OrderDetail orderDetail in order.OrderDetails)
                {
                    orderDetail.Product = new Product { 
                        ProductName = await _context.Products.Where(p => p.ProductId == orderDetail.ProductId).Select(p => p.ProductName).FirstOrDefaultAsync() };
                }
            }
            return orders;
        }

        public class transactionDTO
        {
            public string transactionId { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult> PostCreateOrder(transactionDTO transaction)
        {
            var user = User.Identity.Name;
            var items = await _context.Carts.Where(cart => cart.UserName == user).Include(cart => cart.Product).ToListAsync();
            if(items.Count == 0)
            {
                BadRequest();
            }

            var storeTaxRate = await _context.Stores.Where(s => s.StoreId == items[0].Product.StoreId).Select(s => s.TaxRate).FirstOrDefaultAsync();

            Order order = new Order
            {
                UserName = user,
                StoreId = items[0].Product.StoreId, 
                Status = Constants.ORDER_IN_PROGRESS,
                Tax = storeTaxRate, 
                OrderedTime = DateTime.Now,
                transactionId = transaction.transactionId
            };
            _context.Orders.Add(order);
            foreach(var item in items)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price,
                    Instruction = item.Instruction
                };
                order.OrderDetails.Add(orderDetail);
                _context.Carts.Remove(item);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("markDone/{id}")]
        public async Task<IActionResult> PutOrderComplete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Status = Constants.ORDER_COMPLETED;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("Rate")]
        public async Task<IActionResult> PostRate(RateDTO ratings)
        {
            var orderDetails = await _context.OrderDetails.Where(o => o.OrderId == ratings.OrderId).ToListAsync();
            foreach (OrderDetail orderDetail in orderDetails)
            {
                orderDetail.Rating = ratings.ratings.Find(r => r.ProductId == orderDetail.ProductId).Rating;
            }
            await _context.SaveChangesAsync();

            UpdateRatings(ratings); // This updates rating average
            return NoContent();
        }

        private async Task<int> UpdateRatings(RateDTO ratings)
        {        

            var storeid = await _context.Orders.Where(o => o.OrderId == ratings.OrderId).Select(o => o.StoreId).FirstOrDefaultAsync();
            ratings.ratings.ForEach(rating =>
            {
                if (rating.Rating != null)
                {
                    var t =  _context.Database.ExecuteSqlRaw($"Update product set rating= (Select Sum(rating)/count(*) from order_details where product_id = {rating.ProductId} and rating IS NOT null) where product_id = {rating.ProductId}");
                }
            });
            await _context.Database.ExecuteSqlRawAsync($"Update store set rating = (select SUM(rating)/Count(*) from product where store_id = {storeid} and rating IS NOT NULL ) where store_id = {storeid}");
            return 1;
        }
    }
}
