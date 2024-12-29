### Don't use a properties over a field when not necessary

Long property definitions, such as

```
private int _myProperty1;
public int MyProperty1;

or

private int _myProperty;
public int MyProperty
{
	get => _myProperty;
	set 
	{
		if(value == _myProperty) return;
		_myProperty = value;
	}
}
```

can be written much cleaner with auto-generated properties:

```
public int MyProperty1 { get; private set;}
public int MyProperty2 { get; set; }
```

These have the same result but take up much less space and don't leave a question of which one should be used in the private context. Only if there is additional code that needs to be executed on getting or setting the value is there a need for a field with a property over it. If [field keyword in properties](https://github.com/dotnet/csharplang/blob/main/proposals/field-keyword.md) is ever implemented in the C# language, even in this use case would an additional field be unnecessary.