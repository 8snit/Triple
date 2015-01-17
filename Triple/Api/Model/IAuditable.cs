using System;
using System.Collections.Generic;
using System.Linq;

namespace Triple.Api.Model
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; set; }

        DateTimeOffset? LastChangedAt { get; set; }
    }
}
