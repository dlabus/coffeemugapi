using System;
using CoffeeMugApi.DA.Commons.Attributes;
using MongoDB.Bson.Serialization.Attributes;
namespace CoffeeMugApi.DA.DtoModels
{

    [MongoCollectionName("Products")]
    public class Product
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }
    }
}