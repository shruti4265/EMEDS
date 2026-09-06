using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public interface IOrderRepo
    {
        List<Order> GetAllOrders();

        Order? GetOrderById(int orderId);

        List<Order> GetOrdersByUserId(string userId);

        Order? GetOrderWithItems(int orderId);

        int AddOrder(Order order);

        int UpdateOrder(Order order);

        int UpdateOrderStatus(int orderId, string orderStatus);
    }
}
