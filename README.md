# Linql.Client

A C# Client for the Linql Language.  Allows you to use your api as if it were an IQueryable. 

[Linql Overview](https://github.com/LinqlLang/Linql)

```cs
LinqlContext Context = new LinqlContext("https://localhost:8080");
LinqlSearch<DataModel> search = Context.Set<DataModel>();
string output = await search.Where(r => r.Boolean && r.OneToOne.Boolean).ToListAsync();
```

## Installation

```powershell
dotnet add package Linql.Client -v 1.0.0-alpha1
```

## Basic Usage

### Create a LinqlContext

```cs
LinqlContext Context = new LinqlContext("https://localhost:8080");

//Optional: Change TopLevel functions to be flat, instead of chained through .Next
Context.FlattenTopLevelFunctions = true;
```

### Start a Query

```cs
LinqlSearch<MyModel> basicSearch = Context.Set<MyModel>();
LinqlSearch<MyModel> searchOne = basicSearch.Where(r => r.Integers.Contains(1));
LinqlSearch<MyModel> searchTwo = basicSearch.Where(r => r.Integers.Contains(2));

var results = await Task.WhenAll(
    searchOne.ToListAsync(),
    searchTwo.TwoListASync()
)
```

### Defaults 

The provided LinqlContext above has the following defaults: 

- Endpoint is set to `{BaseUrl}/linql/{TypeName}`
- Ignores serializing default values
- PropertyNames are case insensitive 


## Customization 

To customize the LinqlContext, simply create your own derivation of the default LinqlContext.

```cs
using System.Text.Json.Serialization;
using Linql.Client;
using Linql.Core;
using NetTopologySuite.IO.Converters;

public class CustomLinqlContext : LinqlContext
{
    ///Override constructor to add GeoJson support and allow floating point literals. 
    public CustomLinqlContext(string BaseUrl) : base(BaseUrl)
    {
        var geoJsonConverterFactory = new GeoJsonConverterFactory();
        this.JsonOptions.Converters.Add(geoJsonConverterFactory);
        this.JsonOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;

    }

    ///Instead of {baseurl}/linql/{TypeName}, set the endpoint as {baseurl}/TypeName
    protected override string GetEndpoint(LinqlSearch Search)
    {
        return $"{Search.Type.TypeName}";
    }
}
```

## Full Example

Checkout our full example [here](https://github.com/LinqlLang/CSharp.Linql.Examples).

## Development 

- Visual Studio 2022 17.4
- .Net 6

## Testing 

Unit tests are located [here](./Linql.Client.Test/).