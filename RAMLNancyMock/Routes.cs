using Newtonsoft.Json.Schema;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RAMLNancyMock
{
    class Route
    {
        public readonly string _route;
        private List<RouteMethod> _methods;             
        private List<string> _parameters;

        public Route(string route)
        {
            _route = route;
            _methods = new List<RouteMethod>();
            _parameters = new List<string>();

            Regex rgx = new Regex(@"(?<=\{)\w+(?=\})");     //Extracting parameters from routes i.e. {id} -> id
            foreach (Match match in rgx.Matches(route))
                _parameters.Add(match.Value);

        }

        public void ParseMethods(IEnumerable<Method> ramlMethods)
        {
            foreach (var method in ramlMethods)
            {
                RouteMethod routeMethod = null;
                if (method.Body.Count == 0)      //no Mediatype/Schema
                { 
                    routeMethod = new RouteMethod(method.Verb, String.Empty, null);
                }
                else
                {
                    string mediaType = method.Body.Single().Key;
                    routeMethod = new RouteMethod(method.Verb, 
                        mediaType, 
                        mediaType == "application/json" ? JSchema.Parse(method.Body.Single().Value.Schema) : null
                        );
                }

                foreach(var response in method.Responses)
                {
                    if(response.Body == null)
                    {
                        routeMethod.AddResponse(response.Code, response.Description, string.Empty, null);
                    }
                    else
                    {
                        string respMediaType = response.Body.Single().Key;
                        routeMethod.AddResponse(response.Code, 
                            response.Description,
                            respMediaType,
                            respMediaType == "application/json" ? JSchema.Parse(response.Body.Single().Value.Schema) : null
                            );
                    }

                }

                _methods.Add(routeMethod);
            }
        }

        public IList<RouteMethod> Methods
        {
            get
            {
                return _methods.AsReadOnly();
            }
        }

        public IList<string> Parameters
        {
            get
            {
                return _parameters.AsReadOnly();
            }
        }

    }

    class RouteMethod
    {
        public readonly string verb;
        public readonly JSchema schema;
        public readonly string mediaType;
        private List<MethodResponse> _responses;

        public RouteMethod(string verb, string mediaType, JSchema schema = null)
        {
            _responses = new List<MethodResponse>();

            this.verb = verb;
            this.mediaType = mediaType;
            this.schema = schema;
        }

        public void AddResponse(string returnCode, string description, string mediaType, JSchema schema = null)
        {
            _responses.Add(new MethodResponse(returnCode, description, mediaType, schema));
        }
        public IList<MethodResponse> Responses
        {
            get
            {
                return _responses.AsReadOnly();
            }
        }
    }

    struct MethodResponse
    {
        public readonly string returnCode;
        public readonly string description;
        public readonly JSchema schema;
        public readonly string mediaType;

        public MethodResponse(string returnCode, string description, string mediaType, JSchema schema = null)
        {
            this.returnCode = returnCode;
            this.description = description;
            this.schema = schema;
            this.mediaType = mediaType;
        }
    }




}
