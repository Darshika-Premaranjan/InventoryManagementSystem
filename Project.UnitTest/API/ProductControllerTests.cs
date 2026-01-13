using Project.API.Controllers.V1;
using Project.Core.Entities.Business;
using Project.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;

namespace Project.UnitTest.API
{
    public class ProductControllerTests
    {
        private Mock<IProductService> _productServiceMock;
        private Mock<ILogger<ProductController>> _loggerMock;
        private ProductController _productController;
        private Mock<IMemoryCache> _memoryCacheMock;

        [SetUp]
        public void Setup()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _productController = new ProductController(_loggerMock.Object, _productServiceMock.Object,_memoryCacheMock.Object);
        }

        [Test]
        public async Task Get_ReturnsViewWithListOfProducts()
        {
            // Arrange
            var products = new List<ProductViewModel>
            {
                new ProductViewModel { Id = 1, Code = "P001", Name = "Product A", Price = 9.99f, IsActive = true },
                new ProductViewModel { Id = 2, Code = "P002", Name = "Product B", Price = 19.99f, IsActive = true }
            };

            _productServiceMock.Setup(service => service.GetAll(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(products);

            // Act
            var result = await _productController.Get(It.IsAny<CancellationToken>());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okObjectResult = (OkObjectResult)result;
            Assert.NotNull(okObjectResult);

            var model = (IEnumerable<ProductViewModel>)okObjectResult.Value;
            Assert.NotNull(model);
            Assert.That(model.Count(), Is.EqualTo(products.Count));

        }

        // Add more test methods for other controller actions, such as Create, Update, Delete, etc.

    }
}
