using Raml.Parser;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NancyRAMLMock.RAMLParsing
{
    public sealed class RamlDocWrapper: IRamlDocument
    {
        private readonly RamlDocument ramlDocument = null;
        private Uri baseUri = null;
        private List<IRamlResource> parsedRamlDocResources = null;

        public RamlDocWrapper(string ramlFilePath)
        {
            //MuleSoft Raml parser
            var parser = new RamlParser();
            try
            {
                ramlDocument = parser.LoadAsync(ramlFilePath).Result;
            }
            catch (AggregateException ae)            //Exceptions received from assync RamlParser
            {
                throw ae.Flatten();
            }
        }

        public Uri BaseUri
        {
            get
            {
                if(baseUri == null)
                { 
                    StringBuilder uriString = new StringBuilder(ramlDocument.BaseUri);
                    foreach(var uriPar in ramlDocument.BaseUriParameters)                  //Composing Uri from BaseUri replacing all UriParameters with their real values
                        uriString.Replace($"{{{uriPar.Key}}}", uriPar.Value.Enum.Single());

                    baseUri = new Uri(uriString.ToString());
                }

                return baseUri;
            }
        }

        public IList<IRamlResource> RamlResources
        {
            get
            {
                if (parsedRamlDocResources == null)
                {
                    parsedRamlDocResources = new List<IRamlResource>();
                    parseRamlResources(ramlDocument.Resources, parsedRamlDocResources, String.Empty);
                }

                return  parsedRamlDocResources.AsReadOnly();
            }
        }

        private void parseRamlResources(IEnumerable<Resource> resources, List<IRamlResource> resourcesList, string basePath)
        {
            //Extract & flatten Routes tree (from Resources), Methods and Responses information
            foreach (var resource in resources)
            {
                string deeperPath = String.Concat(basePath, resource.RelativeUri);
                resourcesList.Add(new RamlResourceWrappper(deeperPath, resource.Methods));

                //down the tree
                if (resource.Resources.Count > 0)
                    parseRamlResources(resource.Resources, resourcesList, deeperPath);
            }
        }
    }
}
