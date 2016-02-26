# Nancy.RAML.Mock

### C#/.Net application mocking basic REST/JSON server using RAML 0.8 as API definition

## Usage

Application accept one optional command line argument - path to configuration file in JSON format.
The `ApiConfig.json` from startup folder will be loaded by default. Example configuration file is included in the application folder:

```
{
  "MockUri": "http://localhost:52109",
  "RAMLFilePath": "ApiDescr.raml",
  "MongoConnectionString": "mongodb://localhost:27017",
  "DataBaseName": "NancyRAMLMock"
}
```

1. `MockUri` URI for the Mock service to listen
2. `RAMLFilePath` path to RAML file with API definition
3. `MongoConnectionString` MongoDB server connection string
4. `DataBaseName` MongoDB database name that will store JSON models received/modified during by this Mock


## Details

Application will start self-hosted [NancyFx](https://github.com/NancyFx) HTTP server listening for the incoming requests. Mock server can accept `Post`, `Get`, `Put` and `Delete` HTTP request defined in loaded RAML specification under following conditions:

1. Request path (mixed with the URI received from the RAML spec.) is used as a database collection name basis. To be able to insert, request, modify or delete JSON models in the same collection request *group* must share similar *path*. I.e.:
  * **Post** `http://localhost:52109/movies`
  * **Get**, **Put** or **Delete** `http://localhost:52109/movies/:id` - *path* is the same as of the **Post** but ends with one parameter. It's *name* (taken from RAML spec.) and *value*  will be used to identify record in particular MongoDB collection.
2. The `:id` parameter is not the same as `_id` that MongoDB assigns to record automatically. It's actual name will be taken from the RAML resources and must comply with the one of the fields of the model because this parameter name and it's received value will be used to find and perform record manipulation. Even if multiple records share the similar field that was used as an *id* - only one record will me modified, deleted or returned per request.
