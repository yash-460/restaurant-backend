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
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly restaurantDBContext _context;

        public OrdersController(restaurantDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Where(o => o.UserName == User.Identity.Name).Include(o => o.OrderDetails).Include(o => o.Store).ToListAsync();
        }

        [HttpGet("Store/{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetStoreOrders(short id)
        {
            List<Order> orders =  await _context.Orders.Include(o => o.OrderDetails).Where(o => o.StoreId == id && o.Status == Constants.ORDER_WAITING_APPROVAL).ToListAsync();
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

        [HttpPost]
        public async Task<ActionResult> PostCreateOrder()
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
                StoreId = items[0].Product.StoreId, // Get This
                Status = Constants.ORDER_WAITING_APPROVAL,
                Tax = storeTaxRate, // Get This
                OrderedTime = DateTimeOffset.Now
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

    }
}
