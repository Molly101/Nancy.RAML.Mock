using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock.Data
{
    /// <summary>
    /// Data Storage implementation via MongoDb
    /// </summary>
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
            getMongoCollection(model).InsertOne(model.getBsonModel());
        }

        public DataModel Update(DataModel model)
        {

            var originalDoc = Get(model).getBsonModel();
            var replacementDoc = originalDoc.Merge(model.getBsonModel(), true);

            var result = getMongoCollection(model).ReplaceOne(model.getBsonQuery(), replacementDoc);
            if (result.ModifiedCount == 0)
                result = getMongoCollection(model).ReplaceOne(model.getBsonQueryUnquoted(), replacementDoc);   // {"id":"1"} => {"id":1}

            DataModel resModel = null;
            replacementDoc.Remove("_id");
            if (result.ModifiedCount == 1)
            {
                resModel = new DataModel
                {
                    Path = model.Path,
                    jsonModel = replacementDoc.ToJson()
                };
            }

            return resModel;
        }

        public void Drop(DataModel model)
        {
            database.DropCollection(model.Path.Replace('\\', '_'));
        }

        public bool  Delete(DataModel model)
        {
            var result = getMongoCollection(model).DeleteOne(model.getBsonQuery());
            if(result.DeletedCount!=1)
                result = getMongoCollection(model).DeleteOne(model.getBsonQueryUnquoted());

            return result.DeletedCount == 1;
        }

        public DataModel Get(DataModel model)
        {
            var record = getMongoCollection(model).Find(model.getBsonQuery()).ToList().FirstOrDefault();
            if(record == null)
            {
                record = getMongoCollection(model).Find(model.getBsonQueryUnquoted()).ToList().FirstOrDefault();        // {"id":"1"} => {"id":1}
            }


            DataModel result = null;
            if (record != null)
            {
                record.Remove("_id");

                result = new DataModel() {
                    Path = model.Path,
                    jsonModel = record.ToString()
                };
                
            }

            return result;
        }

        public IList<DataModel> GetMany(DataModel model)
        {
            var records = getMongoCollection(model).Find(model.getBsonQuery()).ToList();

            
            if (records.Count == 0)
            {
                records = getMongoCollection(model).Find(model.getBsonQueryUnquoted()).ToList();                     // {"id":"1"} => {"id":1}
            }

            List<DataModel> result = new List<DataModel>();
            foreach (var record in records)
            {
                record.Remove("_id");
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
