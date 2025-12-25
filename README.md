# Ashutosh.Common.Logger

A flexible .NET logging library with runtime logger discovery using MEF (Managed Extensibility Framework).

## Overview

This library provides a unified logging interface that discovers and loads logger implementations at runtime using MEF. If no custom logger is found, it automatically falls back to Console and Trace output.

## Features

- **Runtime Logger Discovery**: Uses MEF (`System.ComponentModel.Composition`) to discover `ILogger` implementations at runtime via `ApplicationCatalog`
- **Automatic Fallback**: Falls back to `FallbackLogger` (which writes to `System.Diagnostics.Trace`) when no MEF-exported logger is found
- **Console Logging**: Optional console output with color-coded severity levels
- **Multiple Logger Support**: Can route logs to multiple logger implementations simultaneously
- **Severity Levels**: Supports `None`, `Verbose`, `Info`, `Warning`, `Error`, and `Fatal`
- **Exception Handling**: Built-in support for logging exceptions including `AggregateException` expansion
- **Thread-Safe**: Console logging is synchronized for thread safety

## How It Works

### Logger Discovery

1. The `LoggerContainer` class uses MEF to scan all assemblies in the application for types that export `ILogger`
2. If a valid logger is discovered, it is added to the list of active loggers
3. If no logger is found and no additional loggers are configured, the `FallbackLogger` is used
4. The `FallbackLogger` writes formatted log messages to `System.Diagnostics.Trace`

### Fallback Behavior

When no MEF-exported logger is discovered:
- Log output is directed to `System.Diagnostics.Trace.WriteLine()`
- This ensures logs are never silently lost
- Trace output can be captured by attaching trace listeners

## Usage

### Basic Logging

```csharp
using Ashutosh.Common.Logger;

// Create a logger instance
var logger = new Logger(typeof(MyClass), "MyClass");

// Log messages at different severity levels
logger.Log("Information message");
logger.LogVerbose("Verbose details");
logger.LogWarning("Warning message");
logger.LogError("Error occurred");

// Log with exceptions
try
{
    // some code
}
catch (Exception ex)
{
    logger.LogError(ex, "Operation failed: {0}", operationName);
}
```

### Static Logging

```csharp
Logger.Log("ModuleName", "Message", Severity.Info);
Logger.Log("ModuleName", "Error message", Severity.Error, exception);
```

### Automatic Method Tracing

```csharp
using (var log = new Log(typeof(MyClass), "MyMethod", arg1, arg2))
{
    // Method body - entry and exit are automatically logged
    log.WriteLine("Custom trace message");
}
```

## Implementing a Custom Logger

To provide your own logger implementation that will be discovered at runtime:

```csharp
using System.ComponentModel.Composition;
using Ashutosh.Common.Logger;

[Export(typeof(ILogger))]
public class MyCustomLogger : ILogger
{
    public void Log(LogData logData)
    {
        // Your custom logging implementation
        // logData contains: ModuleName, NameSpace, Message, Severity, Exception, ProcessId, ThreadId
    }
}
```

The `[Export(typeof(ILogger))]` attribute marks the class for MEF discovery. When your assembly is loaded, MEF will automatically find and use your logger.

## Configuration

### Force Console Output

```csharp
LoggerInitializer.ForceLogToConsole = true;
```

### Add Additional Loggers

```csharp
LoggerInitializer.AdditionalLoggers.Add(new MyCustomLogger());
```

## Log Data Structure

The `LogData` class contains:
- **ModuleName**: Name of the logging module/class
- **NameSpace**: Namespace of the logging source
- **Message**: The log message
- **Severity**: Log level (None, Verbose, Info, Warning, Error, Fatal)
- **Exception**: Formatted exception details
- **ProcessId**: Current process ID
- **ThreadId**: Current thread ID

## License

All rights reserved. Reproduction or transmission in whole or in part, in any form or by any means, electronic, mechanical or otherwise, is prohibited without prior written permission.
