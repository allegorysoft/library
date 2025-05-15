using System;

namespace Allegory.Standard.Filter.Concrete;

public class FilterException : Exception
{
    public FilterException() {}
    public FilterException(string message) : base(message) {}
}