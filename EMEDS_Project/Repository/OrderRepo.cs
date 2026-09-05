using EMEDS_Project.DAL;
using EMEDS_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.Repository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly EmedDbContext _context;

        public OrderRepo(EmedDbContext context)
        {
            _context = context;
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(order => order.OrderItems)
                .OrderByDescending(order => order.OrderDate)
                .ToList();
        }

        public Order? GetOrderById(int orderId)
        {
            return _context.Orders
                .FirstOrDefault(order => order.OrderId == orderId);
        }

        public List<Order> GetOrdersByUserId(string userId)
        {
            return _context.Orders
                .Include(order => order.OrderItems)
                .Where(order => order.UserId == userId)
                .OrderByDescending(order => order.OrderDate)
                .ToList();
        }

        public Order? GetOrderWithItems(int orderId)
        {
            return _context.Orders
                .Include(order => order.OrderItems)
                .FirstOrDefault(order => order.OrderId == orderId);
        }

        public int AddOrder(Order order)
        {
            _context.Orders.Add(order);

            return _context.SaveChanges();
        }

        public int UpdateOrder(Order order)
        {
            _context.Orders.Update(order);

            return _context.SaveChanges();
        }

        public int UpdateOrderStatus(
            int orderId,
            string orderStatus)
        {
            var order = _context.Orders
                .FirstOrDefault(order => order.OrderId == orderId);

            if (order == null)
            {
                return 0;
            }

            order.OrderStatus = orderStatus;

            return _context.SaveChanges();
        }
    }
}