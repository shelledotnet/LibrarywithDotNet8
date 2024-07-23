using Newtonsoft.Json;
using Serilog;
using System;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using Azure;

namespace Books.API.Extensions
{
    public static class LogHelper
    {
        public static string RequestPayload = "";

       

        public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            try
            {
                var request = httpContext.Request;
                ResponseBody responseBody = null;
                dynamic reqBody = JsonConvert.DeserializeObject<dynamic>(RequestPayload);
                if (reqBody?.ContainsKey("bvn") == true)
                    reqBody.bvn = "******";

                RequestPayload = JsonConvert.SerializeObject(reqBody);
                var requestHeader = JsonConvert.SerializeObject(httpContext.Request.Headers);
                diagnosticContext.Set("RequestHeader", requestHeader);
                diagnosticContext.Set("RequestBody", RequestPayload);
                diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString());
                diagnosticContext.Set("CorrelationId", httpContext.Request.Headers["x-correlation-id"].ToString());
                diagnosticContext.Set("ClientId", httpContext.Request.Headers["client_id"].ToString());
                diagnosticContext.Set("ProductId", httpContext.Request.Headers["product_id"].ToString());
                string responseBodyPayload = await ReadResponseBody(httpContext.Response);
                diagnosticContext.Set("ResponseBody", responseBodyPayload);

                if (IsValidXmlUsingXDocument(responseBodyPayload))
                {
                    string Message = null, ResponseId = null, IsSuccess = null;
                  XDocument xdocs = XDocument.Parse(responseBodyPayload);
                  


                    responseBody = new ResponseBody { Message = Message, ResponseId = ResponseId, IsSuccess = Convert.ToBoolean(IsSuccess) };
                }
                else if (!IsValidXmlUsingXDocument(responseBodyPayload))
                {
                   responseBody = JsonConvert.DeserializeObject<ResponseBody>(responseBodyPayload);

                }

                if (responseBody != null)
                {
                    diagnosticContext.Set("ResponseCode", responseBody.IsSuccess);
                    diagnosticContext.Set("Description", responseBody.Message);
                    diagnosticContext.Set("ResponseId", responseBody.ResponseId);
                }


                diagnosticContext.Set("ResponseHeader", JsonConvert.SerializeObject(httpContext.Response.Headers));
                // Set all the common properties available for every request
                diagnosticContext.Set("Host", request.Host);
                diagnosticContext.Set("Protocol", request.Protocol);
                diagnosticContext.Set("Scheme", request.Scheme);

                // Only set it if available. You're not sending sensitive data in a querystring right?!
                if (request.QueryString.HasValue)
                {
                    diagnosticContext.Set("QueryString", request.QueryString.Value);
                }

                // Set the content-type of the Response at this point
                diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

                // Retrieve the IEndpointFeature selected for the request
                var endpoint = httpContext.GetEndpoint();
                if (endpoint is object) // endpoint != null
                {
                    diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                }

                // Only set it if available. You're not sending sensitive data in the cookies right?!
                var cookies = httpContext.Response.Headers["Set-Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookies))
                {
                    diagnosticContext.Set("Set-Cookie", cookies);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{responseBody}";

        }

        public static bool IsValidXmlUsingXDocument(string xmlString)
        {
            try
            {
                XDocument.Parse(xmlString);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }
    }

    [XmlRoot("ResponseBody")]
    public class ResponseBody
    {
        [XmlElement("Data")]
        public dynamic? Data{ get; set; }

        [XmlElement("ResponseId")]
        public string? ResponseId { get; set; }

        [XmlElement("IsSuccess")]
        public bool IsSuccess { get; set; }

        [XmlElement("Message")]
        public dynamic? Message { get; set; }
    }

    public class XmlOjectConverter
    {
        public static string ObjectToXML(object obj)
        {
            using (var stringwriter = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            };

        }


        public static T XmlToObject<T>(string input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                var result = (T)serializer.Deserialize(sr);
                return result;
            };

        }
    }
    public class Utf8StringWriter : StringWriter
    {
        // Use UTF8 encoding but write no BOM to the wire
        public override Encoding Encoding
        {
            get { return new UTF8Encoding(false); } // in real code I'll cache this encoding.
        }
    }
}
