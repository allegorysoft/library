using System;

namespace Allegory.Standart.Filter.Concrete
{
    public class FilterException : Exception
    {
        public FilterException() { }
        public FilterException(string message) : base(message) { }
    }
}
