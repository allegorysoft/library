# Allegory.Filter

The filter package is designed to enable the creation of flexible and customizable queries in applications that require dynamic filtering of data. This package allows users to define complex filtering conditions on-the-fly, based on user input or other programmatic criteria, without the need for extensive coding or SQL knowledge.

### Usage

```csharp
//Create conditions
var conditions = new Condition
{
    Group = new List<Condition>
    {
        new Condition(nameof(Product.Name), Operator.Contains, "drinks"),
        new Condition(nameof(Product.Price), Operator.IsGreaterThanOrEqualto, 15)
    }
};

//Dapper 
var query = string.Format(
    "SELECT * FROM Products WHERE Active = 0 {0}",
    conditions.GetFilterQuery<Product>(
	out IDictionary<string, object> parameters,
	OperatorCombine.WithAnd));
await connection.QueryAsync<Product>(query, parameters);

//Expression
var products = _productRepository.GetListAsync(conditions.ToExpression<Product>());

//Lambda Expression
var products = new List<Product>(){ ... };
var filteredProducts = products.Where(conditions.ToLambdaExpression<Product>());

public class Product
{
  public string Name { get; set; } = null!;
  public int Price { get; set;}
}
```
