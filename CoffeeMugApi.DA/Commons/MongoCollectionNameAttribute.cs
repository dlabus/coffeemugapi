using System;

namespace CoffeeMugApi.DA.Commons.Attributes
{
    public class MongoCollectionNameAttribute : Attribute 
    {
        public string CollectionName { get; set; }   
              
        public MongoCollectionNameAttribute(string collectionName)
        {
            this.CollectionName = collectionName;
        }

    }
}