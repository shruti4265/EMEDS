using EMEDS_Project.DAL;
using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public class PaymentRepo : Repository<Payment>, IPaymentRepo
    {
        public PaymentRepo(EmedDbContext context) : base(context)
        {
        }

        public Payment? GetByOrderId(int orderId)
        {
            return _context.Payments.FirstOrDefault(p => p.OrderId == orderId);
        }
    }
}