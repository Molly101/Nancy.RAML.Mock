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

        public DataModel Insert(DataModel model)
        {
            getMongoCollection(model).InsertOne(model.getBsonModel());

            return new DataModel() { operationSuccesfull = true , jsonModel = model.jsonModel };
        }
   

        public DataModel Update(DataModel model)
        {

            var originalDoc = GetBsonDoc(model);
            var replacementDoc = originalDoc.Merge(model.getBsonModel(), true);

            var result = getMongoCollection(model).ReplaceOne(model.getOrFilter(), replacementDoc);

            replacementDoc.Remove("_id");
            return new DataModel() { jsonModel = replacementDoc.ToJson(), operationSuccesfull = (result.ModifiedCount == 1) };
        }

        public DataModel Drop(DataModel model)   //TO DO
        {
            database.DropCollection(model.getCollectionName());

            return new DataModel() { operationSuccesfull = true };
        }

        public DataModel Delete(DataModel model)
        {
            var result = getMongoCollection(model).DeleteOne(model.getOrFilter());

            return new DataModel() { operationSuccesfull = (result.DeletedCount == 1) }; 
        }

        public DataModel Get(DataModel model)
        {
            var record = GetBsonDoc(model);

            DataModel result = null;
            if (record != null)
            {
                record.Remove("_id");
                result = new DataModel() { jsonModel = record.ToString() };
            }

            return result;
        }

        public IList<DataModel> GetMany(DataModel model)    //TO DO
        {
            var records = getMongoCollection(model).Find(model.getOrFilter()).ToList();

            List<DataModel> result = new List<DataModel>();
            foreach (var record in records)
            {
                record.Remove("_id");
                result.Add(new DataModel() { jsonModel = record.ToJson(), operationSuccesfull = true });
            }

            return result.AsReadOnly();
        }

        private BsonDocument GetBsonDoc(DataModel model) => getMongoCollection(model).Find(model.getOrFilter()).ToList().FirstOrDefault();
    }
}
