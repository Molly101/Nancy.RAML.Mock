using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyRAMLMock.Data
{
    public static class BsonDocumentExtension
    {
        /// <summary>
        /// Converts all numeric BsonElement values from string represenjtation to numeric values (Int32/Double), i.e. {"id":"1"} => {"id":1}
        /// </summary>
        public static BsonDocument parseNumericElements(this BsonDocument original)         
        {
            var result = new BsonDocument();
            int numInt;
            double numDouble;
            
            foreach(var el in original)
            {
                string elValue = el.Value.ToString();

                if (elValue.Contains('.') && Double.TryParse(elValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out numDouble))
                {
                    result.Add(el.Name, numDouble); 
                }
                else if(Int32.TryParse(elValue, out numInt))
                {
                    result.Add(el.Name, numInt);
                }
                else
                {
                    result.Add(el);
                }

            }

            return result;
        }

    }
}
