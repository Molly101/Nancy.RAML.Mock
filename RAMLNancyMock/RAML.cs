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
        private RamlDocument _ramlDocument = null;
        private Uri _baseUri = null;

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
                    foreach(var uriPar in _ramlDocument.BaseUriParameters)                  //Composing Uri from base + parameters
                        uriString.Replace($"{{{uriPar.Key}}}", uriPar.Value.Enum.Single());

                    _baseUri = new Uri(uriString.ToString());
                }

                return _baseUri;
            }
        }
    }
}
