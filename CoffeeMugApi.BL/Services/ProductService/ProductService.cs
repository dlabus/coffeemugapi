using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeMugApi.DA.DtoModels;
using CoffeeMugApi.DA.Repositories;

namespace CoffeeMugApi.DL.Services.ProductService
{
    public class ProductService : IProductService
    {
        public IRepository<Product> Repository { get; }

        public ProductService(IRepository<Product> repository)
        {
            this.Repository = repository;
        }

        public async Task<IEnumerable<Product>> List()
        {
            try
            {
                return await this.Repository.GetAll();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<Product> GetProductById(Guid productId)
        {
            try
            {
                return await this.Repository.Get(productId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<Guid> AddProduct(Product product)
        {
            try
            {
                return await this.Repository.Insert(product);
            }
            catch (System.Exception)
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateProduct(Guid id, Product product)
        {
            try
            {
                if (await this.GetProductById(id) != null)
                {
                    Product productForUpdate;
                    if (product.Id == Guid.Empty)
                    {
                        productForUpdate = new Product { Id = id, Name = product.Name, Price = product.Price };
                    }
                    else
                    {
                        productForUpdate = product;
                    }

                    await this.Repository.Update(id, productForUpdate);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveProduct(Guid productId)
        {
            try
            {
                await this.Repository.Remove(productId);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}