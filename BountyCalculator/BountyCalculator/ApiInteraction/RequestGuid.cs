using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    public sealed class RequestGuid : IEquatable<RequestGuid>, IEquatable<string>
    {
        private readonly Guid _guid1;
        private readonly Guid _guid2;

        public RequestGuid()
        {
            _guid1 = Guid.NewGuid();
            _guid2 = Guid.NewGuid();
        }

        public bool Equals(string s)
        {
            return s.Equals(ToString());
        }

        public bool Equals(RequestGuid other)
        {
            return (_guid1 == other._guid1) && (_guid2 == other._guid2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is string)
            {
                return Equals((string)obj);
            }
            else if (obj is RequestGuid)
            {
                return Equals((RequestGuid)obj);
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{_guid1}_{_guid2}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_guid1, _guid2);
        }
    }
}
