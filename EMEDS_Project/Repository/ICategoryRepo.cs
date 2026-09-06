using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public interface ICategoryRepo
    {
        IEnumerable<Category> GetAllCategories();

        Category? GetCategoryById(int categoryId);

        void AddCategory(Category category);

        void UpdateCategory(Category category);

        void DeleteCategory(int categoryId);
    }
}