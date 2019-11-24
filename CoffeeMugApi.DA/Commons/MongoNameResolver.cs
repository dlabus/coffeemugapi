using System.Linq;
using CoffeeMugApi.DA.Commons.Attributes;

namespace CoffeMugApi.BD.Commons
{
    public class MongoNameResolver<T> where T : class
    {
        public string GetCollectionName()
        {
            return (typeof(T).GetCustomAttributes(typeof(MongoCollectionNameAttribute), true)
                .FirstOrDefault() as MongoCollectionNameAttribute)?.CollectionName;
        }
    }
}