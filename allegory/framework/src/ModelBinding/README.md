# Allegory.ModelBinding

Windows form app two way binding extension

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