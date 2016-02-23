# Nancy.RAML.Mock

### _REST/JSON API Mock Service_

C# application based on Nancy simulating basic REST/JSON API accepting RAML 0.8 as API definition

## Usage

Application can accept 2 optional command line arguments: 

1. Path to RAML file that will be used as an API definition (defaults to API.RAML located in the same folder with an executable) 
2. MongoDB connection string (default to @"mongodb://localhost:27017")

An URI for the Mock service to listen on will be extracted from RAML "baseUri" parameter.

## Details



