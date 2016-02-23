using NancyRAMLMock;
using NancyRAMLMock.RAMLParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json.Schema;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace xUnit.Tests
{
    public class RAMLDocWrapperTest
    {
        private IRamlDocument testRaml;

        public RAMLDocWrapperTest()
        {
            testRaml = new RamlDocWrapper("test.raml");
        }

        [Fact]
        public void ShouldReturnCorrectUri()
        {
            Assert.Equal<Uri>(new Uri("http://remote-vending/api"), testRaml.BaseUri);
        }

        [Fact]
        public void ShouldContain3Resource()
        {
            Assert.Equal(3, testRaml.RamlResources.Count);
        }

        [Theory]
        [InlineData("/sales", 0)]
        [InlineData("/machines", 1)]
        [InlineData("/machines/{machine}", 2)]
        public void ShouldContainCorrectResourcePath(string path, int i)
        {
            Assert.Equal(path, testRaml.RamlResources[i].Path);
        }

        [Fact]
        public void ShouldContainCorrectResourceParameter()
        {
            Assert.Equal("machine", testRaml.RamlResources[2].RamlParameters.Single());
        }

        [Fact]
        public void ResourceShouldContainCorrectMethod()
        {
            Assert.Equal("get", testRaml.RamlResources.Last().RamlMethods.Single().Verb);
        }

        [Fact]
        public void MethodShouldContainCorrectResponse()
        {
            var response = testRaml.RamlResources.Last().RamlMethods.Single().RamlResponses.Single();

            Assert.Equal("application/json", response.MediaType);
            Assert.Equal(true, response.IsMTJson);
            
            JObject machine = JObject.Parse(@"{
                  'id' : 'ZX4102',
                  'location' : 'Starbucks, 442 Geary Street, San Francisco, CA 94102',
                  'sales' : [
                    {
                      'dateAndTime' : '2013-10-22 16:17:00',
                      'value' : 450,
                      'machineId' : 'ZX4102',
                      'productId' : 'Cad-CB1012'
                    },
                    {
                      'dateAndTime' : '2013-10-22 16:17:00',
                      'value' : 150,
                      'machineId' : 'ZX5322',
                      'productId' : 'CC-LB1'
                    }
                  ],
                  'floatsToBeReplenished' : [20, 40, 20, 80, 20, 40, 40],
                  'stockToBeReplenished' : 54
              }");

            Assert.Equal<bool>(true, machine.IsValid(JSchema.Parse(response.JsonSchema)));
        }

        [Theory]
        [InlineData("NoSuchFile.raml")]
        [InlineData("Invalid_version.raml")]
        [InlineData("Invalid_schema.raml")]
        [InlineData("Invalid_model.raml")]
        [InlineData("Invalid_resource.raml")]
        public void IncorrectRamFile(string ramlFile)
        {
            Assert.Throws<System.AggregateException>(() => new RamlDocWrapper(ramlFile));
        }
    }
}
