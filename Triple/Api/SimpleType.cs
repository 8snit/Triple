using System;
using System.Collections.Generic;

namespace Triple.Api
{
    public struct SimpleType
    {
        private readonly bool _isEnumerable;

        private readonly Type _raw;

        private readonly Type _resolved;

        public SimpleType(Type raw)
            : this()
        {
            this._raw = raw;

            foreach (var rawInterface in raw.GetInterfaces())
            {
                if (rawInterface.IsGenericType
                    && rawInterface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    this._resolved = rawInterface.GetGenericArguments()[0];
                    this._isEnumerable = true;
                    return;
                }
            }

            if (raw.IsGenericType)
            {
                this._resolved = raw.GetGenericArguments()[0];
                this._isEnumerable = false;
                return;
            }

            this._resolved = raw;
            this._isEnumerable = raw.IsArray;
        }

        public Type Raw
        {
            get
            {
                return this._raw;
            }
        }

        public Type Resolved
        {
            get
            {
                return this._resolved;
            }
        }

        public bool IsEnumerable
        {
            get
            {
                return this._isEnumerable;
            }
        }

        public override string ToString()
        {
            return string.Format("Raw: {0}, Resolved: {1}, IsEnumerable: {2}", this._raw, this._resolved,
                this._isEnumerable);
        }
    }
}
