using EMEDS_Project.Controllers;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EMEDS.Tests
{
    [TestFixture]
    public class CategoryTests
    {
        private Mock<ICategoryRepo> _categoryRepoMock;
        private CategoryController _controller;

        [SetUp]
        public void Setup()
        {
            _categoryRepoMock = new Mock<ICategoryRepo>();
            _controller = new CategoryController(_categoryRepoMock.Object);
        }

        [Test]
        public void Index_ReturnsViewWithAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category(),
                new Category()
            };

            _categoryRepoMock
                .Setup(repo => repo.GetAllCategories())
                .Returns(categories);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(categories));
        }

        [Test]
        public void Details_WhenCategoryExists_ReturnsViewWithCategory()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category();

            _categoryRepoMock
                .Setup(repo => repo.GetCategoryById(categoryId))
                .Returns(category);

            // Act
            var result = _controller.Details(categoryId);

            // Assert
            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(category));
        }

        [Test]
        public void Details_WhenCategoryDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int categoryId = 999;

            _categoryRepoMock
                .Setup(repo => repo.GetCategoryById(categoryId))
                .Returns((Category?)null);

            // Act
            var result = _controller.Details(categoryId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void Create_WhenModelIsValid_AddsCategoryAndRedirectsToIndex()
        {
            // Arrange
            var category = new Category();

            // Act
            var result = _controller.Create(category);

            // Assert
            _categoryRepoMock.Verify(
                repo => repo.AddCategory(category),
                Times.Once
            );

            var redirectResult = result as RedirectToActionResult;

            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(
                redirectResult.ActionName,
                Is.EqualTo(nameof(CategoryController.Index))
            );
        }
    }
}