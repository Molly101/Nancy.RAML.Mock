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

            var originalDoc = GetBsonDoc(model);
            var replacementDoc = originalDoc.Merge(model.getBsonModel(), true);

            var result = getMongoCollection(model).ReplaceOne(model.getOrFilter(), replacementDoc);

            DataModel resModel = null;
            replacementDoc.Remove("_id");
            if (result.ModifiedCount == 1)
            {
                resModel = new DataModel { jsonModel = replacementDoc.ToJson() };
            }

            return resModel;
        }

        public void Drop(DataModel model)   //TO DO
        {
            database.DropCollection(model.getCollectionName());
        }

        public bool Delete(DataModel model)
        {
            var result = getMongoCollection(model).DeleteOne(model.getOrFilter());
            return result.DeletedCount == 1;
        }

        public DataModel Get(DataModel model)
        {
            var record = GetBsonDoc(model);

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

        public IList<DataModel> GetMany(DataModel model)    //TO DO
        {
            var records = getMongoCollection(model).Find(model.getOrFilter()).ToList();

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

        private BsonDocument GetBsonDoc(DataModel model)
        {
            var record = getMongoCollection(model).Find(model.getOrFilter()).ToList().FirstOrDefault();

            return record;
        }
    }
}
