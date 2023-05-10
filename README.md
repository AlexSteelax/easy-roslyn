# easy-roslyn
[![Steelax.EasyRoslyn](https://img.shields.io/nuget/v/Steelax.EasyRoslyn.svg)](https://www.nuget.org/packages/Steelax.EasyRoslyn) [![Steelax.EasyRoslyn](https://img.shields.io/nuget/dt/Steelax.EasyRoslyn.svg)](https://www.nuget.org/packages/Steelax.EasyRoslyn/)

EasyRoslyn is a simple wrapper around the standard library. This simplifies code generation and debugging.

## Supported Runtimes
- .NET 6.0+
- .NET Standard 2.0, 2.1
- .NET Framework 4.8

## Runtime Installation

All stable packages are available on [NuGet](https://www.nuget.org/packages/Steelax.EasyRoslyn/).

## Basic usage

### 1 First Configure your codegenerator
```csharp
var result = EasyRoslyn
  //Set output type
  .CreateCSharpBuilder(OutputKind.DynamicallyLinkedLibrary)
  
  //Set compilation options
  .ConfigureCompilationOptions(s => s
    .WithCurrentPlatform()
    .WithOptimizationLevel(OptimizationLevel.Debug))
  
  //Add sources from files or text
  .ConfigureSources(s => s.FromText(code, "Class.cs"))
  
  //Add extra references
  .ConfigureReferences(s => s
    .UseNetStandard()
    .UseSystemRuntime()
    .UseReference(typeof(Console)))
  
  //Set emit options
  .ConfigureEmitOptions(s => s
    .WithDebugInformationFormat(DebugInformationFormat.PortablePdb))
  
  //Run compilation
  .Build();
```

### 2 Load your assembly and/or print messages
```csharp
result
  //Try load if successful
  .TryLoad(out Assembly assembly)
  //Print diagnostic
  .WriteLog(logger);
```
### 3 Save assembly and symbols (optional)
```csharp
result.TrySave(dir);
```


