using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;

namespace NancyRAMLMock.Data
{
    /// <summary>
    /// Data Storage model extension methods
    /// </summary>
    public static class DataModelExtension
    {
        public static BsonDocument getBsonModel(this DataModel model) => BsonDocument.Parse(model.jsonModel);

        public static string getCollectionName(this DataModel model) => model.Path.TrimStart('/').TrimEnd('/').Replace('/', '_');       // "/Somewhere/Something/" => "Somewhere_Something" 

        public static BsonDocument getBsonQuery(this DataModel model) => BsonDocument.Parse(model.jsonQuery);

        public static BsonDocument getBsonQueryUnquoted(this DataModel model)
        {
            string unquotedQuery = model.jsonQuery;

            unquotedQuery = unquotedQuery.Remove(unquotedQuery.LastIndexOf('"'),1);
            unquotedQuery = unquotedQuery.Remove(unquotedQuery.LastIndexOf('"'),1);

            return BsonDocument.Parse(unquotedQuery);
        }

    }
}
