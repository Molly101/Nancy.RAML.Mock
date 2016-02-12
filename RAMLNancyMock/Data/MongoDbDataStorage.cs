using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMLNancyMock.Data
{
    public class MongoDbDataStorage : IDataStorage
    {
        private readonly IMongoCollection<BsonDocument> collection;
        private readonly IMongoDatabase database;
        private readonly string pathElement = "__path";

        private BsonDocument FullDocument(DataModel model) => BsonDocument.Parse(model.jsonModel).Add(pathElement, model.Path);
        private BsonDocument FullQuery(DataModel model) => BsonDocument.Parse(model.jsonQuery).Add(pathElement, model.Path);

        public MongoDbDataStorage(IMongoDatabase database)
        {
            this.database = database;
            collection = database.GetCollection<BsonDocument>("JSONwithPath");
        }

        public void Insert(DataModel model)
        {
            collection.InsertOne(FullDocument(model));
        }

        public long Update(DataModel model)
        {
            FilterDefinition<BsonDocument> filter = FullQuery(model);
            var result = collection.UpdateMany(filter, FullDocument(model));
            return result.ModifiedCount;
        }

        public void Drop(DataModel model)
        {
            throw new NotImplementedException();
        }

        public long  Delete(DataModel model)
        {
            FilterDefinition<BsonDocument> filter = FullQuery(model);

            var result = collection.DeleteMany(filter);

            return result.DeletedCount;
        }

        public IList<DataModel> Get(DataModel model)
        {
            FilterDefinition<BsonDocument> filter = FullQuery(model);

            var recordList = collection.Find(filter).ToList();
            var modelList = new List<DataModel>();

            foreach (var record in recordList)
            {
                var newModel = new DataModel();
                newModel.Path = record.GetValue(pathElement).ToString();
                record.Remove(pathElement);
                newModel.jsonModel = record.ToJson();

                modelList.Add(new DataModel());
            }

            return modelList.AsReadOnly();
        }

        
    }
}
