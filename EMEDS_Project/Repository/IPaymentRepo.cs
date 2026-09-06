using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public interface IPaymentRepo : IRepository<Payment>
    {
        Payment? GetByOrderId(int orderId);
    }
}
