using Raml.Parser;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NancyRAMLMock
{
    public sealed class RAML
    {
        private readonly RamlDocument _ramlDocument = null;
        private Uri _baseUri = null;
        private List<RAMLRoute> _routes = null;

        public RAML(string ramlFilePath)
        {
            //Raml parser
            var parser = new RamlParser();
            _ramlDocument = parser.LoadAsync(ramlFilePath).Result;
        }
       
        public Uri BaseUri
        {
            get
            {
                if(_baseUri == null)
                { 
                    StringBuilder uriString = new StringBuilder(_ramlDocument.BaseUri);
                    foreach(var uriPar in _ramlDocument.BaseUriParameters)                  //Composing Uri from BaseUri replacing all UriParameters with their real values
                        uriString.Replace($"{{{uriPar.Key}}}", uriPar.Value.Enum.Single());

                    _baseUri = new Uri(uriString.ToString());
                }

                return _baseUri;
            }
        }

        public IList<RAMLRoute> Routes
        {
            get
            {
                if (_routes == null)
                {
                    _routes = new List<RAMLRoute>();
                    _parseResourcesToRoutes(_ramlDocument.Resources, _routes, String.Empty);
                }

                return _routes.AsReadOnly();
            }
        }

        private void _parseResourcesToRoutes(IEnumerable<Resource> resources, List<RAMLRoute> routesList, string baseRoute)
        {
            //Extract & flatten Routes tree (from Resources), Methods and Responses information
            foreach (var resource in resources)
            {
                string routePath = String.Concat(baseRoute, resource.RelativeUri);
                var route = new RAMLRoute(routePath, resource.Methods);
                routesList.Add(route);

                //down the tree
                if (resource.Resources.Count > 0)
                    _parseResourcesToRoutes(resource.Resources, routesList, routePath);
            }
        }
    }
}
