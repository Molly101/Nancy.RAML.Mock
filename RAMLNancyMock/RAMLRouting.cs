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
    public sealed class RAMLRoute
    {
        public string route { get; }
        private List<RAMLRouteMethod> _methods;
        public IList<RAMLRouteMethod> Methods => _methods.AsReadOnly();
        private List<string> _parameters;
        public IList<string> Parameters => _parameters.AsReadOnly();


        public RAMLRoute(string route, IEnumerable<Method> ramlMethods)
        {
            this.route = route;

            _parameters = new List<string>();
            Regex rgx = new Regex(@"(?<=\{)\w+(?=\})");     //Extracting parameters from routes i.e. {id} -> id
            foreach (Match match in rgx.Matches(route))
                _parameters.Add(match.Value);

            _methods = ParseRAMLMethods(ramlMethods);
        }

        public List<RAMLRouteMethod> ParseRAMLMethods(IEnumerable<Method> ramlMethods)
        {
            var result = new List<RAMLRouteMethod>();

            foreach (var method in ramlMethods)
                result.Add(new RAMLRouteMethod(method));

            return result;
        }

    }

    public sealed class RAMLRouteMethod
    {
        public string verb { get; }
        public string mediaType { get; }
        public JSchema schema { get; }
        private List<RAMLMethodResponse> _responses;
        public IList<RAMLMethodResponse> Responses => _responses.AsReadOnly();

        public RAMLRouteMethod(Method ramlMethod)
        {
            verb = ramlMethod.Verb;

            var methodBody = ramlMethod.Body.FirstOrDefault(kvp => kvp.Key.Contains("json"));
            mediaType = methodBody.Key;
            schema = (mediaType == "application/json") ? JSchema.Parse(methodBody.Value.Schema) : null;

            _responses = parseRAMLResponses(ramlMethod.Responses);
        }

        public List<RAMLMethodResponse> parseRAMLResponses(IEnumerable<Response> ramlResponses)
        {
            var result = new List<RAMLMethodResponse>();

            foreach(var response in ramlResponses)
                result.Add(new RAMLMethodResponse(response));

            return result;
        }
        
    }

    public sealed class RAMLMethodResponse
    {
        public string returnCode { get; }
        public string description { get; }
        public string mediaType { get; }
        public JSchema schema { get; }

        public RAMLMethodResponse(Response ramlResponse)
        {
            returnCode = ramlResponse.Code;
            description = ramlResponse.Description;

            if(ramlResponse.Body != null)
            { 
                var responseBody = ramlResponse.Body.FirstOrDefault(kvp => kvp.Key.Contains("json"));

                mediaType = responseBody.Key;
                schema = (mediaType == "application/json") ? JSchema.Parse(responseBody.Value.Schema) : null;
            }
            else
            {
                mediaType = null;
                schema = null;
            }
        }
    }




}
