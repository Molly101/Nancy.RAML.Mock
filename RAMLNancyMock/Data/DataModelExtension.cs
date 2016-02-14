using MongoDB.Bson;
using MongoDB.Driver;

namespace NancyRAMLMock.Data
{
    public static class DataModelExtension
    {
        public static BsonDocument getBsonDoc(this DataModel model) => BsonDocument.Parse(model.jsonModel);

        public static FilterDefinition<BsonDocument> getFilter(this DataModel model) => BsonDocument.Parse(model.jsonQuery);

        public static string getCollectionName(this DataModel model) => model.Path.Replace('/', '_');
    }
}
