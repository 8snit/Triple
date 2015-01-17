using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Triple.Api.Model
{
    public class MetaData
    {
        private readonly Dictionary<string, object> _data;

        public MetaData()
        {
            this._data = new Dictionary<string, object>();
        }

        public MetaData(IEnumerable<KeyValuePair<string, object>> data)
        {
            this._data = data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public IEnumerable<KeyValuePair<string, object>> Data
        {
            get
            {
                return this._data;
            }
        }

        public object this[string key]
        {
            get
            {
                return this._data.GetOrDefault(key);
            }
            set
            {
                if (value == null)
                {
                    this._data.Remove(key);
                }
                else
                {
                    this._data.AddOrUpdate(key, value);
                }
            }
        }

        public static implicit operator string(MetaData metaData)
        {
            return JsonConvert.SerializeObject(metaData.Data, new KeyValuePairConverter());
        }

        public static implicit operator MetaData(string metaData)
        {
            return
                new MetaData(JsonConvert.DeserializeObject<Dictionary<string, object>>(metaData,
                    new KeyValuePairConverter()));
        }

        public static MetaData ForVenue(string id, string name, string address, decimal? rating)
        {
            var metaData = new MetaData();
            metaData["id"] = id;
            metaData["name"] = name;
            metaData["address"] = name;
            if (rating.HasValue)
            {
                metaData["rating"] = rating.Value;
            }
            return metaData;
        }

        public static MetaData ForImage(int height, int width)
        {
            var metaData = new MetaData();
            metaData["height"] = height;
            metaData["width"] = width;
            return metaData;
        }
    }
}
