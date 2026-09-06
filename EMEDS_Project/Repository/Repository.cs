using EMEDS_Project.DAL;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly EmedDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(EmedDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList();
        public T? GetById(int id) => _dbSet.Find(id);
        public void Add(T entity) => _dbSet.Add(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
        public void Save() => _context.SaveChanges();
    }
}
