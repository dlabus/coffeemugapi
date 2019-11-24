using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeMugApi.DA.DtoModels;
using CoffeeMugApi.DA.Repositories;
using CoffeeMugApi.DL.Services.ProductService;
using Moq;
using NUnit.Framework;

namespace CoffeeMugApi.Tests
{
    public class ProductServiceTests
    {
        [Test]
        public async Task List_Products_Returns_The_Same_Collection_Test()
        {
            // Arrange
            var random = new Random();
            var productCollection = new List<Product>();

            for (int i = 0; i < 5; i++)
            {
                productCollection.Add(new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() });
            }

            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.GetAll()).ReturnsAsync(productCollection);

            //Act
            var productService = new ProductService(mockProductRepository.Object);

            var listedProducts = (await productService.List()).ToList();

            //Assert

            CollectionAssert.AreEqual(productCollection, listedProducts, "Procedure returns different collection");

        }

        [Test]
        public async Task List_Products_Returns_Empy_Collection_When_There_Is_No_Data_Test()
        {
            // Arrange
            var random = new Random();
            var productCollection = new List<Product>();


            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.GetAll()).ReturnsAsync(productCollection);

            var productService = new ProductService(mockProductRepository.Object);

            //Act
            var listedProducts = (await productService.List()).ToList();

            //Assert
            CollectionAssert.IsEmpty(productCollection, "Collection should be empty");
        }


        [Test]
        public async Task Get_Product_By_Id_Returns_Correct_Product_Test()
        {
            //Arrange
            var random = new Random();
            var productCollection = new List<Product>(); ;

            for (int i = 0; i < 5; i++)
            {
                productCollection.Add(new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 });
            }

            var randomItemToTest = productCollection[random.Next(0, 10)];


            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync((Guid id) => productCollection.FirstOrDefault(item => item.Id == id));

            var productService = new ProductService(mockProductRepository.Object);

            //Act
            var selectedItem = await productService.GetProductById(randomItemToTest.Id);

            //Assert
            Assert.AreEqual(randomItemToTest, selectedItem, "Item should exist and be the same as template");
        }

        [Test]
        public async Task Get_Product_By_Id_Returns_Null_When_There_Is_No_Product_Test()
        {
            //Arrange
            var random = new Random();
            var productCollection = new List<Product>(); ;

            for (int i = 0; i < 5; i++)
            {
                productCollection.Add(new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 });
            }

            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync((Guid id) => productCollection.FirstOrDefault(item => item.Id == id));

            var productService = new ProductService(mockProductRepository.Object);
            //Act
            var selectedItem = await productService.GetProductById(Guid.NewGuid());

            //Assert
            Assert.IsNull(selectedItem, "Selected item should be null");
        }

        [Test]
        public async Task Add_Product_Returns_Guid_When_Product_Is_Inserted()
        {
            //Arrange
            var random = new Random();

            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Insert(It.IsAny<Product>())).ReturnsAsync((Product product) => Guid.NewGuid());

            var productService = new ProductService(mockProductRepository.Object);

            //Act
            var idOfInsertedItem = await productService.AddProduct(new Product { Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 });

            //Assert
            Assert.IsNotNull(idOfInsertedItem, "return value should not be null");
        }

        [Test]
        public async Task Add_Product_Product_Exists_In_Collection_After_Insert()
        {
            //Arange
            var random = new Random();
            var productCollection = new List<Product>();

            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Insert(It.IsAny<Product>())).ReturnsAsync(
                (Product product) =>
                {
                    productCollection.Add(product);
                    return Guid.NewGuid();
                });

            var productService = new ProductService(mockProductRepository.Object);
            var productForInsert = new Product { Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            //Act
            var idOfInsertedProduct = await productService.AddProduct(productForInsert);

            //Assert
            Assert.Contains(productForInsert, productCollection, "Product should exist in collection");
        }

        [Test]
        public async Task Add_Product_When_Product_Exist_In_Collection_Procedure_Returns_Empty_Guid()
        {
            //Arange
            var random = new Random();
            var productCollection = new List<Product>();

            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Insert(It.IsAny<Product>())).ReturnsAsync(
                (Product product) => Guid.Empty);

            var productService = new ProductService(mockProductRepository.Object);
            var productForInsert = new Product { Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            //Act
            var idOfInsertedProduct = await productService.AddProduct(productForInsert);

            //Assert
            Assert.AreEqual(idOfInsertedProduct, Guid.Empty, "Procedure should return empty guid");
        }

        [Test]
        public async Task Update_Product_Should_Change_Product_In_Collection()
        {
            //Arange
            var random = new Random();
            var productCollection = new List<Product>();
            var productForTest = new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            productCollection.Add(productForTest);


            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Update(It.IsAny<Guid>(), It.IsAny<Product>()))
            .Returns((Guid id, Product product) =>
            {
               var t = new Task(() =>
               {
                   var prod = productCollection.FirstOrDefault(p => p.Id == id);
                   prod.Name = product.Name;
                   prod.Price = product.Price;
               });
               t.Start();
               return t;
            });

            mockProductRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync((Guid id) => productCollection.FirstOrDefault(item => item.Id == id));

            var productService = new ProductService(mockProductRepository.Object);
            var productForUpdate = new Product { Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            //Act
            var result = await productService.UpdateProduct(productForTest.Id, productForUpdate);

            //Assert
            Assert.IsTrue(productForUpdate.Name == productForTest.Name && productForUpdate.Price == productForTest.Price, "Procedure does not update the item in the collection");
        }
        [Test]
        public async Task Update_Product_Returns_False_When_Product_Not_Exist()
        {

            //Arange
            var random = new Random();
            var productCollection = new List<Product>();
            var productForTest = new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            productCollection.Add(productForTest);


            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Update(It.IsAny<Guid>(), It.IsAny<Product>()))
            .Returns((Guid id, Product product) =>
            {
                return new Task(() =>
                {
                    var prod = productCollection.FirstOrDefault(p => p.Id == id);
                    prod.Name = product.Name;
                    prod.Price = product.Price;
                });
            });

            mockProductRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync((Guid id) => productCollection.FirstOrDefault(item => item.Id == id));

            var productService = new ProductService(mockProductRepository.Object);
            var productForUpdate = new Product { Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };

            //Act
            var result = await productService.UpdateProduct(Guid.NewGuid(), productForUpdate);

            //Assert
            Assert.IsFalse(result, "Procedure shoudl return false when updated product does not exist");
        }

        public async Task Update_Product_Returns_True_When_Product_Exist()
        {

            //Arange
            var random = new Random();
            var productCollection = new List<Product>();
            var productForTest = new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            productCollection.Add(productForTest);


            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Update(It.IsAny<Guid>(), It.IsAny<Product>()))
            .Returns((Guid id, Product product) =>
            {
                return new Task(() =>
                {
                    var prod = productCollection.FirstOrDefault(p => p.Id == id);
                    prod.Name = product.Name;
                    prod.Price = product.Price;
                });
            });

            mockProductRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync((Guid id) => productCollection.FirstOrDefault(item => item.Id == id));

            var productService = new ProductService(mockProductRepository.Object);
            var productForUpdate = new Product { Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };

            //Act
            var result = await productService.UpdateProduct(productForTest.Id, productForUpdate);

            //Assert
            Assert.IsTrue(result, "Procedure should return true when updated product exists");
        }

        public async Task Remove_Product_Remove_product_From_Collection()
        {
            //Arange
            var random = new Random();
            var productCollection = new List<Product>();
            var productForTest = new Product { Id = Guid.NewGuid(), Name = nameGenerator(), Price = (decimal)random.NextDouble() * 100 };
            productCollection.Add(productForTest);

            var mockProductRepository = new Mock<IRepository<Product>>();
            mockProductRepository.Setup(m => m.Remove(It.IsAny<Guid>()))
            .Returns((Guid id) =>
            {
                return new Task(() =>
                {
                    var prod = productCollection.FirstOrDefault(p => p.Id == id);
                    productCollection.Remove(prod);
                });
            });

            var productService = new ProductService(mockProductRepository.Object);
            //Act
            var result = await productService.RemoveProduct(productForTest.Id);

            //Assert
            CollectionAssert.IsEmpty(productCollection, "Collection should be empty");
        }

        private string nameGenerator()
        {
            string generatedString = string.Empty;
            var random = new Random();
            int length = random.Next(10, 20);

            for (int i = 0; i < length; i++)
            {
                generatedString += (char)(random.Next(97, 122));
            }

            return generatedString;
        }
    }
}