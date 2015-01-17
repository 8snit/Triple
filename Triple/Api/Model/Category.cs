using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Triple.Api.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Category
    {
        Unknown,

        Accomodation,

        FoodAndDrink,

        Entertainment,

        Transportation,

        Nature,

        SightSeeing
    }
}
