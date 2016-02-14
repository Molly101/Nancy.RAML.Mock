using Raml.Parser.Expressions;
using System.Linq;

namespace NancyRAMLMock.RAMLParsing
{
    public sealed class RamlResponseWrapper: IRamlResponse
    {
        public string HttpReturnCode { get; }
        public string Description { get; }
        public string MediaType { get; }
        public string JsonSchema { get; }
        public bool IsMTJson { get; }

        public RamlResponseWrapper(Response ramlDocResponse)
        {
            HttpReturnCode = ramlDocResponse.Code;
            Description = ramlDocResponse.Description;

            if (ramlDocResponse.Body != null)
            {
                var responseBody = ramlDocResponse.Body.FirstOrDefault(kvp => kvp.Key.Contains("json"));

                MediaType = responseBody.Key;
                JsonSchema = (MediaType != null) ? responseBody.Value.Schema : null;
                IsMTJson = (JsonSchema != null);
            }
        }
    }
}
