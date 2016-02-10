using Raml.Parser;
using Raml.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RAMLNancyMock
{
    public sealed class RAMLDocument
    {
        private readonly RamlDocument _ramlDocument = null;
        private Uri _baseUri = null;
        private List<Route> _routes = null;

        public RAMLDocument(string ramlFilePath)
        {
            if (!File.Exists(ramlFilePath))
                throw new FileNotFoundException($"Could not find the specified RAML file \"{ramlFilePath}\"!");

            //Raml parser
            var parser = new RamlParser();
            _ramlDocument = parser.LoadAsync(ramlFilePath).Result;

            //Extracting Routes (Resources), Methods and Response information
            _routes = new List<Route>();
            _parseResourcesToRoutes(_ramlDocument.Resources, _routes, String.Empty);
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
                return _routes.AsReadOnly();
            }
        }

        private void _parseResourcesToRoutes(IEnumerable<Resource> resources, List<Route> routesList, string baseRoute)
        {
            foreach (var resource in resources)
            {
                string routePath = String.Concat(baseRoute, resource.RelativeUri);

                var route = new Route(routePath);
                route.ParseMethods(resource.Methods);
                routesList.Add(route);

                //down the tree
                if (resource.Resources.Count > 0)
                    _parseResourcesToRoutes(resource.Resources, routesList, routePath);
            }
        }
    }
}
