using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeMugApi.DA.DtoModels;
using CoffeMugApi.BD.Commons;
using MongoDB.Driver;

namespace CoffeeMugApi.DA.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        public IMongoDatabase Database { get; }
        private IMongoCollection<Product> _collection
        {
            get
            {
                var collectionNameResolver = new MongoNameResolver<Product>();
                return Database.GetCollection<Product>(collectionNameResolver.GetCollectionName());
            }
        }

        public ProductRepository(IMongoClient client, string database)
        {
            Database = client.GetDatabase(database);
        }

        public async Task<Product> Get(Guid productId)
        {
            var asyncCursor = await this._collection.FindAsync(_ => _.Id == productId);
            return await asyncCursor.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var asyncCursor = await this._collection.FindAsync(_ => true);
            return await asyncCursor.ToListAsync();
        }

        public async Task<Guid> Insert(Product product)
        {
            await this._collection.InsertOneAsync(product);
            return product.Id;
        }

        public async Task Update(Guid id, Product product)
        {
            await this._collection.FindOneAndReplaceAsync(_ => _.Id == id, product);
        }

        public async Task Remove(Guid productId)
        {
            await this._collection.DeleteOneAsync(_ => _.Id == productId);
        }
    }
}