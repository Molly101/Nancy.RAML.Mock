using Raml.Parser;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMLNancyMock
{
    class RAMLDocument
    {
        private readonly RamlDocument _ramlDocument = null;
        private Uri _baseUri = null;
        private List<Route> _routes = null;

        public RAMLDocument(string ramlFilePath)
        {
            if (!File.Exists(ramlFilePath))
                throw new FileNotFoundException($"Could not find the specified RAML file \"{ramlFilePath}\"!");

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

        public IList<Route> Routes
        {
            get
            {
                if(_routes == null)
                {
                   _routes = new List<Route>();
                    _parseResources(_ramlDocument.Resources, _routes, String.Empty);
                }

                return _routes.AsReadOnly();
            }
        }

        private void _parseResources(ICollection<Resource> resources, List<Route> routesList, string baseRoute)
        {
            foreach (var resource in resources)
            {
                string route = String.Concat(baseRoute, resource.RelativeUri);
                routesList.Add(new Route(route));
                if (resource.Resources.Count > 0)
                    _parseResources(resource.Resources, routesList, route);
            }
        }
    }
}
