using EMEDS_Project.DAL;
using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public class SupplierRepo : Repository<Supplier>, ISupplierRepo
    {
        public SupplierRepo(EmedDbContext context)
            : base(context)
        {
        }
    }
}
