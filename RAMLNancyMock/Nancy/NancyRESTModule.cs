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

            var requests = from r in ramlDoc.RamlResources
                          from m in r.RamlMethods
                          where (m.Verb == "post" && r.Path.Last() != '}') ||
                                (m.Verb == "put" || m.Verb == "delete" || m.Verb == "get") && r.Path.Last() == '}'
                          select new RequestDetails(r, m);


            foreach (var request in requests)
            {
                switch(request.Verb)
                {
                    case "post":
                        Post[request.Path] = param => postFx(param, request);
                        break;
            //        //case "get":
            //        //    Get[r.route] = param => getFx(param, r.parameter.Single());
            //        //    break;

                }
            }


        }

        private Response postFx(DynamicDictionary parameters, RequestDetails request)
        {
            string requestString = Request.Body.AsString();

            JObject requestJson = null;
            try
            {
                requestJson = JObject.Parse(requestString);
            }
            catch(Exception ex)
            {
                logger.Error("Incorrect JSON in POST request!");
                logger.Error(ex);
            }

            JSchema jsonSchema = (request.Schema != null) ? JSchema.Parse(request.Schema) : null;
            bool valid = (jsonSchema != null) ? requestJson.IsValid(jsonSchema) : false;

            if (valid)
            {
                dataStorage.Insert(new DataModel {
                    jsonSchema = request.Schema,
                    jsonModel = requestString,
                    Path = Request.Path
                });
            }
                
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
