using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock.Data
{
    public class MongoDbDataStorage : IDataStorage
    {
        private readonly IMongoDatabase database;
        private IMongoCollection<BsonDocument> getMongoCollection(DataModel model) => database.GetCollection<BsonDocument>(model.getCollectionName());

        public MongoDbDataStorage(IMongoDatabase database)
        {
            this.database = database;
        }

        public void Insert(DataModel model)
        {
            getMongoCollection(model).InsertOne(model.getBsonDoc());
        }

        public bool Update(DataModel model)
        {
            var result = getMongoCollection(model).UpdateOne(model.getFilter(), model.getBsonDoc());
            return result.ModifiedCount == 1;
        }

        public void Drop(DataModel model)
        {
            database.DropCollection(model.Path.Replace('\\', '_'));
        }

        public bool  Delete(DataModel model)
        {
            var result = getMongoCollection(model).DeleteOne(model.getFilter());

            return result.DeletedCount == 1;
        }

        public DataModel Get(DataModel model)
        {
            var record = getMongoCollection(model).Find(model.getFilter()).ToList().FirstOrDefault();
            DataModel result = null;

            if (record != null)
            {
                result = new DataModel(){
                    Path = model.Path,
                    jsonModel = record.ToJson()
                };
                
            }

            return result;
        }

        public IList<DataModel> GetMany(DataModel model)
        {
            var records = getMongoCollection(model).Find(model.getFilter()).ToList();
            List<DataModel> result = new List<DataModel>();

            foreach (var record in records)
            {
                result.Add(new DataModel()
                    {
                        Path = model.Path,
                        jsonModel = record.ToJson()
                    });
            }

            return result.AsReadOnly();
        }
    }
}
