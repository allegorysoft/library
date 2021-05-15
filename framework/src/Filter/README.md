# Allegory.Filter

Define conditions without lambda expression or sql parameter

### Quick start
> Create conditions
```csharp
Condition conditions = new Condition
{
    Group = new List<Condition>
    {
        new Condition("ProductName", Operator.Contains, "drinks"),
        new Condition("Price", Operator.IsGreaterThanOrEqualto, 15)
    }
};
```
> Work with ADO.NET
```csharp
conditions = conditions.RemoveConditions<Product>();//Remove condition not member of entity
using (SqlDataAdapter dataAdapter = new SqlDataAdapter(string.Format("select * from Products {0}", conditions.GetFilterQuery()), connection))
{
    dataAdapter.SelectCommand.Parameters.AddRange(conditions.GetSqlParameters().ToArray());
    DataTable dataTable = new DataTable();
    dataAdapter.Fill(dataTable);
}
```
> Work with Dapper
```csharp
string query = string.Format("select * from Products where Active = 0 {0}", conditions.GetFilterQuery(OperatorCombine.WithAnd));
var parameters= conditions.GetDictionaries();
connection.Query<Product>(query, parameters).ToList();
```
> Work with Allegory.EntityRepository
```csharp
_productDal.GetList(conditions.GetLambdaExpression<Product>());
```
> Work with DevExpress GridControl
```csharp
Condition conditions = CriteriaOperator.Parse(gridView1.ActiveFilterString).GetConditions();
```
