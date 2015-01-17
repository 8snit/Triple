using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Triple.Api.Model;

namespace Triple.Api.Spatial
{
    public class GeoJsonFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializer _jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        public GeoJsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.geo+json"));
            this.SupportedEncodings.Add(new UTF8Encoding(false, true));
            this.SupportedEncodings.Add(new UnicodeEncoding(false, true, true));
        }

        public GeoJsonFormatter(MediaTypeMapping mediaTypeMapping)
            : this()
        {
            this.MediaTypeMappings.Add(mediaTypeMapping);
        }

        public GeoJsonFormatter(IEnumerable<MediaTypeMapping> mediaTypeMappings)
            : this()
        {
            foreach (var mediaTypeMapping in mediaTypeMappings)
            {
                this.MediaTypeMappings.Add(mediaTypeMapping);
            }
        }

        public override bool CanWriteType(Type type)
        {
            var simpleType = new SimpleType(type);
            return simpleType.Resolved == typeof(Trip)
                   || simpleType.Resolved == typeof(Attachment)
                   || simpleType.Resolved == typeof(Activity)
                   || simpleType.Resolved == typeof(Ride);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger,
            CancellationToken cancellationToken)
        {
            var simpleType = new SimpleType(type);
            if (simpleType.IsEnumerable)
            {
                throw new NotSupportedException();
            }
            throw new NotSupportedException();
            //var encoding = this.SelectCharacterEncoding(content.Headers);
            //var streamReader = new StreamReader(readStream, encoding);
            //var jsonTextReader = new JsonTextReader(streamReader);
            //var target = _geoJsonConverter.ReadJson(jsonTextReader, simpleType.Resolved, null, _jsonSerializer);
            //return Task.FromResult(target);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext, CancellationToken cancellationToken)
        {
            if (value != null)
            {
                var encoding = this.SelectCharacterEncoding(content.Headers);
                var buffer = encoding.GetBytes(JsonConvert.SerializeObject(value, Formatting.Indented));
                writeStream.Write(buffer, 0, buffer.Length);
                writeStream.Flush();
            }
            return Task.FromResult(0);
        }
    }
}
