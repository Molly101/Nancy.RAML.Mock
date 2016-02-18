using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Text;

namespace NancyRAMLMock.Data
{
    /// <summary>
    /// Data Storage model extension methods
    /// </summary>
    public static class DataModelExtension
    {
        private static char[] urlSeparators = { '/', ':' };

        public static BsonDocument getBsonModel(this DataModel model) => (!String.IsNullOrEmpty(model.jsonModel)) ? BsonDocument.Parse(model.jsonModel) : null;

        public static string getCollectionName(this DataModel model) => String.Join("_", model.Path.Split(urlSeparators, StringSplitOptions.RemoveEmptyEntries));        // "http://localhost:52109/movies/" => "http_localhost_52109_movies" 
        
        public static BsonDocument getFilter(this DataModel model) => BsonDocument.Parse(model.jsonQuery);

        public static FilterDefinition<BsonDocument> getOrFilter(this DataModel model)
        {
            var builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> part1 = getFilter(model);
            FilterDefinition<BsonDocument> part2 = getFilter(model).parseNumericElements();
            return part1 | part2;
        }
    }
}
