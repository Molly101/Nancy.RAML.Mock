using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NLog;
using NancyRAMLMock.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NancyRAMLMock.RAMLParsing;

namespace NancyRAMLMock

{
    public class NANCYRestModule : NancyModule
    {
        private IDataStorage dataStorage;
        private ILogger logger;
        private IRamlDocument ramlDoc;

        public NANCYRestModule(IDataStorage dataStorage, ILogger logger, IRamlDocument ramlDoc)
        {
            this.dataStorage = dataStorage;
            this.logger = logger;
            this.ramlDoc = ramlDoc;

            var queries = from r in ramlDoc.RamlResources
                          from m in r.RamlMethods
                          where (m.Verb == "post" && r.Path.Last() != '}') ||
                                (m.Verb == "put" || m.Verb == "delete" || m.Verb == "get") && r.Path.Last() == '}'
                          select new RequestDetails(r, m);


            foreach (var query in queries)
            {
            //    switch(query.verb)
            //    {
            //        //case "post":
            //        //    Post[query.route] = param => postFx(param, query.methods);
            //        //    break;
            //        //case "get":
            //        //    Get[r.route] = param => getFx(param, r.parameter.Single());
            //        //    break;

            //    }
            }


        }

        private Response postFx(DynamicDictionary parameters, JSchema schema)
        {
            string requestString = Request.Body.AsString();
            JObject requestJson = JObject.Parse(requestString);
            bool valid = requestJson.IsValid(schema);


            //dataStorage.TryAdd(requestJson.GetValue("id").ToString(), requestString);

            var a = dataStorage;
            Response response = requestString;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }


        private Response getFx(DynamicDictionary parameters, string parameterName)
        {
            string responseString = "OK";
            //dataStorage.TryGetValue(parameters[parameterName], out responseString);

            Response response = responseString;
            response.StatusCode = (HttpStatusCode) 200;

            return response;
        }

        
    }
}
