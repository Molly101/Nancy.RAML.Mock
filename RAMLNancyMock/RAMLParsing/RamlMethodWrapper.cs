using Raml.Parser.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace NancyRAMLMock.RAMLParsing
{
    public sealed class RamlMethodWrapper : IRamlMethod
    {
        public string Verb { get; }
        public string MediaType { get; }
        public string JsonSchema { get; }
        private List<IRamlResponse> ramlResponses;
        public IList<IRamlResponse> RamlResponses => ramlResponses.AsReadOnly();
        public bool IsMTJson { get; }

        public RamlMethodWrapper(Method ramlDocMethod)
        {
            Verb = ramlDocMethod.Verb;

            var methodBody = ramlDocMethod.Body.FirstOrDefault(kvp => kvp.Key.Contains("json"));
            MediaType = methodBody.Key;
            JsonSchema = (MediaType != null) ? methodBody.Value.Schema : null;
            IsMTJson = (JsonSchema != null);

            ramlResponses = parseRAMLResponses(ramlDocMethod.Responses);
        }

        public List<IRamlResponse> parseRAMLResponses(IEnumerable<Response> ramlResponses)
        {
            var result = new List<IRamlResponse>();

            foreach (var response in ramlResponses)
                result.Add(new RamlResponseWrapper(response));

            return result;
        }

    }
}
