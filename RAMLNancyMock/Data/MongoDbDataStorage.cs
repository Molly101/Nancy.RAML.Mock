using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.IO;

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

        /// <summary>
        /// MongoDb -> Find with Filter defined in DataModel and return BsonDocument
        /// </summary>
        private BsonDocument GetBsonDoc(DataModel model) => getMongoCollection(model).Find(model.getOrFilter()).ToList().FirstOrDefault();

        public DataModel Update(DataModel model)
        {
            var resultModel = new DataModel() { operationSuccesfull = false };

            var originalDoc = GetBsonDoc(model);
            if (originalDoc != null)
            {
                var replacementDoc = originalDoc.Merge(model.getBsonModel(), true);
                var replacementResult = getMongoCollection(model).ReplaceOne(model.getOrFilter(), replacementDoc);
                
                if (replacementResult.ModifiedCount == 1)
                {
                    resultModel.operationSuccesfull = true;
                    replacementDoc.Remove("_id");
                    resultModel.jsonModel = replacementDoc.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
                }

            }

            return resultModel;
        }

        public DataModel Delete(DataModel model)
        {
            var result = getMongoCollection(model).DeleteOne(model.getOrFilter());

            return new DataModel() { operationSuccesfull = (result.DeletedCount == 1) }; 
        }

        public DataModel Get(DataModel model)
        {
            var resultModel = new DataModel() { operationSuccesfull = false };

            var record = GetBsonDoc(model);
            if (record != null)
            {
                record.Remove("_id");
                resultModel.jsonModel = record.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict});
                resultModel.operationSuccesfull = true;
            }

            return resultModel;
        }

        public DataModel Drop(DataModel model)   //TO DO
        {
            database.DropCollection(model.getCollectionName());

            return new DataModel() { operationSuccesfull = true };
        }

        public IList<DataModel> GetMany(DataModel model)    //TO DO
        {
            var records = getMongoCollection(model).Find(model.getOrFilter()).ToList();

            List<DataModel> result = new List<DataModel>();
            foreach (var record in records)
            {
                record.Remove("_id");
                result.Add(new DataModel() { jsonModel = record.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict }), operationSuccesfull = true });
            }

            return result.AsReadOnly();
        }
    }
}
