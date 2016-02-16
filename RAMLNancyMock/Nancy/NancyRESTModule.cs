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

            //selecting only Post without parameter as the last element of the PATH 
            //selecting Get, Put and Delete with a parameter (some kind of id) as the as last element of the PATH 
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
                    case "get":
                        Get[request.Path] = param => getFx(param, request);
                        break;
                    case "put":
                        Put[request.Path] = param => putFx(param, request);
                        break;
                    case "delete":
                        Delete[request.Path] = param => deleteFx(param, request);
                        break;

                }
            }


        }

        private Response postFx(DynamicDictionary parameters, RequestDetails request)
        {
            string requestString = Request.Body.AsString();
            var  response = new Response();
            JObject requestJson = null;

            try
            {
                requestJson = JObject.Parse(requestString);
            }
            catch(Exception ex)
            {
                logger.Error("Incorrect JSON in POST request!");
                logger.Error(ex);

                response = "Bad Request";
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            JSchema jsonSchema = (request.Schema != null) ? JSchema.Parse(request.Schema) : null;
            bool valid = (jsonSchema != null && requestJson != null) ? requestJson.IsValid(jsonSchema) : false;

            if (valid)
            {
                dataStorage.Insert(new DataModel {
                    jsonSchema = request.Schema,
                    jsonModel = requestString,
                    Path = Request.Path
                });

                response = requestString;
                response.ContentType = "application/json";
                response.StatusCode = (HttpStatusCode) request.SuccessResponseCode;
            }
            else
            {
                logger.Error("JSON in POST request does not match RAML schema!!!");
                response.StatusCode = HttpStatusCode.UnprocessableEntity;
            }
                
            return response;
        }


        private Response getFx(DynamicDictionary parameters, RequestDetails request)
        {
            string lastParName = request.Parameters.Last();
            string lastParValue = parameters[lastParName].Value;
            string mongoPath = Request.Path.Replace($"{lastParValue}", "");
            string jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}";

            var res = dataStorage.Get(new DataModel {
                jsonSchema = request.Schema,
                Path = mongoPath,
                jsonQuery = jsonQuery
            });


            Response response = new Nancy.Response();
            if (res != null)
            {
                response = res.jsonModel;
                response.ContentType = "application/json";
                response.StatusCode = (HttpStatusCode)request.SuccessResponseCode;
            }
            else
            { 
                response.StatusCode = HttpStatusCode.NotFound;
            }
            return response;
        }

        private Response putFx(DynamicDictionary parameters, RequestDetails request)
        {
            string lastParName = request.Parameters.Last();
            string lastParValue = parameters[lastParName].Value;
            string mongoPath = Request.Path.Replace($"{lastParValue}", "");
            string jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}";

            string requestString = Request.Body.AsString();

            var result = dataStorage.Update(new DataModel
            {
                jsonSchema = request.Schema,
                Path = mongoPath,
                jsonQuery = jsonQuery,
                jsonModel = requestString
            });

            Response response = new Nancy.Response();
            if (result != null)
            {
                response = result.jsonModel;
                response.ContentType = "application/json";
                response.StatusCode = (HttpStatusCode)request.SuccessResponseCode;
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }

            return response;
        }

        private Response deleteFx(DynamicDictionary parameters, RequestDetails request)
        {
            string lastParName = request.Parameters.Last();
            string lastParValue = parameters[lastParName].Value;
            string mongoPath = Request.Path.Replace($"{lastParValue}", "");
            string jsonQuery = $"{{\"{lastParName}\":\"{lastParValue}\"}}";

            var res = dataStorage.Delete(new DataModel
            {
                jsonSchema = request.Schema,
                Path = mongoPath,
                jsonQuery = jsonQuery,
            });

            return "Ok";
        }
    }
}
