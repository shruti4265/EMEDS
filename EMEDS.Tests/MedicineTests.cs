using EMEDS_Project.Controllers;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace EMEDS.Tests
{
    [TestFixture]
    public class MedicineTests
    {
        private Mock<IMedicineRepo> _medicineRepoMock;
        private MedicineController _controller;

        [SetUp]
        public void Setup()
        {
            _medicineRepoMock = new Mock<IMedicineRepo>();
            _controller = new MedicineController(_medicineRepoMock.Object);
        }

        [Test]
        public void Index_ReturnsViewWithAllMedicines()
        {
            // Arrange
            var medicines = new List<Medicine>
            {
                new Medicine(),
                new Medicine()
            };

            _medicineRepoMock
                .Setup(repo => repo.GetAllMedicines())
                .Returns(medicines);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(medicines));
        }

        [Test]
        public void Details_WhenMedicineExists_ReturnsViewWithMedicine()
        {
            // Arrange
            int medicineId = 1;
            var medicine = new Medicine();

            _medicineRepoMock
                .Setup(repo => repo.GetMedicineById(medicineId))
                .Returns(medicine);

            // Act
            var result = _controller.Details(medicineId);

            // Assert
            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(medicine));
        }

        [Test]
        public void Details_WhenMedicineDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int medicineId = 999;

            _medicineRepoMock
                .Setup(repo => repo.GetMedicineById(medicineId))
                .Returns((Medicine?)null);

            // Act
            var result = _controller.Details(medicineId);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void Create_WhenModelIsValid_AddsMedicineAndRedirectsToIndex()
        {
            // Arrange
            var medicine = new Medicine();

            // Act
            var result = _controller.Create(medicine);

            // Assert
            _medicineRepoMock.Verify(
                repo => repo.AddMedicine(medicine),
                Times.Once
            );

            var redirectResult = result as RedirectToActionResult;

            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(
                redirectResult.ActionName,
                Is.EqualTo(nameof(MedicineController.Index))
            );
        }

        [Test]
        public void Search_ReturnsIndexViewWithSearchResults()
        {
            // Arrange
            string searchTerm = "Paracetamol";

            var medicines = new List<Medicine>
            {
                new Medicine()
            };

            _medicineRepoMock
                .Setup(repo => repo.SearchMedicines(searchTerm))
                .Returns(medicines);

            // Act
            var result = _controller.Search(searchTerm);

            // Assert
            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
            Assert.That(viewResult.Model, Is.EqualTo(medicines));
        }

    }
}