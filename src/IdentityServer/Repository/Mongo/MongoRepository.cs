using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdentityServer.Repository.Mongo
{
    public class MongoRepository : IRepository
    {
        private readonly IMongoDatabase MongoDatabase;

        public MongoRepository(DatabaseSettings databaseSettings)
        {
            var connectionString = databaseSettings.ConnectionString;
            var databaseName = databaseSettings.DatabaseName;
            var mongoClient = new MongoClient(connectionString);
            MongoDatabase = mongoClient.GetDatabase(databaseName);
        }

        public IQueryable<T> All<T>() where T : class, new()
        {
            return MongoDatabase.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var result = MongoDatabase.GetCollection<T>(typeof(T).Name).DeleteMany(predicate);

        }
        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = MongoDatabase.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = collection.CountDocuments(filter);
            return (totalCount > 0) ? true : false;

        }

        public void Add<T>(T item) where T : class, new()
        {
            MongoDatabase.GetCollection<T>(typeof(T).Name).InsertOne(item);
        }

        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            MongoDatabase.GetCollection<T>(typeof(T).Name).InsertMany(items);
        }
    }
}