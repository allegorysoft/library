namespace Allegory.Standard.Filter.Enums;

public enum Operator
{
    Equals,//=
    DoesntEquals,//!=
    IsGreaterThan,//>
    IsGreaterThanOrEqualto,//>=
    IsLessThan,//<
    IsLessThanOrEqualto,//<=
    IsBetween,//>= and <=
    Contains,//like
    StartsWith,//like  @parameter+'%'
    EndsWith,//like '%'+@parameter
    IsNull,//is null
    IsNullOrEmpty,//nullif(column,'') is null
    In,//in(@values)
}