using EMEDS_Project.DAL;
using EMEDS_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly EmedDbContext _context;

        public CategoryRepo(EmedDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories
                .AsNoTracking()
                .ToList();
        }

        public Category? GetCategoryById(int categoryId)
        {
            return _context.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoryId == categoryId);
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            Category? category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }
    }
}