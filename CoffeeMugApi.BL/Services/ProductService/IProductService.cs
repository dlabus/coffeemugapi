using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeMugApi.DA.DtoModels;

namespace CoffeeMugApi.DL.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> List();

        Task<Product> GetProductById(Guid _);

        Task<Guid> AddProduct(Product _);

        Task<bool> UpdateProduct(Guid _, Product __);

        Task<bool> RemoveProduct(Guid _);
    }
}