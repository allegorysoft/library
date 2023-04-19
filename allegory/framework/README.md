# Allegory.Filter

The filter package is designed to enable the creation of flexible and customizable queries in applications that require dynamic filtering of data. This package allows users to define complex filtering conditions on-the-fly, based on user input or other programmatic criteria, without the need for extensive coding or SQL knowledge.

### Usage
```csharp
//Create conditions
var conditions = new Condition
{
    Group = new List<Condition>
    {
        new Condition("ProductName", Operator.Contains, "drinks"),
        new Condition("Price", Operator.IsGreaterThanOrEqualto, 15)
    }
};

//Dapper 
var query = string.Format(
    "SELECT * FROM Products WHERE Active = 0 {0}",
    conditions.GetFilterQuery<ReportBikRecord>(
	out IDictionary<string, object> parameters,
	OperatorCombine.WithAnd));
await connection.QueryAsync<Product>(query, parameters);

//Expression
var products = _productRepository.GetListAsync(conditions.ToExpression<Product>());

//Lambda Expression
var products = new List<Product>(){ ... };
var filteredProducts = products.Where(conditions.ToLambdaExpression<Product>());
```

# Allegory.ModelBinding

With this extension, developers can easily establish a binding relationship between a control and a data source, and changes made to the control will automatically be reflected in the data source, and vice versa. This can significantly simplify the code required to handle data in a Windows Forms application and make it easier to maintain and update.

> Program.cs
```csharp
using Allegory.ModelBinding.Abstract;
using Allegory.ModelBinding.Concrete;
using Allegory.ModelBinding.Dx.Concrete;

ControlBindingExtension.SetControlBindings(new HashSet<ControlBindingBase>
{
	new ControlBindingWin(),
	new ControlBindingDevExpress()
	//etc binding base inherited classes
});
```
> AnyForm.cs
```csharp
using Allegory.ModelBinding.Concrete;

SampleModel model = new SampleModel();

@OnLoad
ControlBindingExtension.GetFromModel(Controls, ref model);
Controls.GetFromModel(ref model);//same thing with extension method

@OnSave
ControlBindingExtension.GetFromControl(Controls, ref model);
Controls.GetFromControl(ref model);//same thing with extension method
```
