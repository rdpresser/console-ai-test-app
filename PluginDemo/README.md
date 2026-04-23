# PluginDemo - Phase 3

## Overview
Phase 3 of the AI learning series, focusing on Semantic Kernel Plugins and native functions (tool use/function calling) for live data access.

## Concepts Covered
- [KernelFunction] + [Description] to expose C# methods as AI tools
- FunctionChoiceBehavior.Auto() to let the LLM call functions
- The LLM autonomously decides which function to invoke
- In production: functions call SQL Server, REST APIs, message queues

## Features
- Healthcare claim plugin with 4 native functions
- Automatic function calling with fallback
- Live data simulation
- Error handling and graceful degradation
- Environment-controlled debug notices

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required model: `llama3` (default)

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Model to use (default: `llama3`)
- `PLUGINDEMO_SHOW_FALLBACK_NOTICE`: Show fallback notices (default: false)

## Usage
```bash
# Run with default settings
cd PluginDemo
dotnet run

# Run with custom configuration
OLLAMA_ENDPOINT=http://localhost:11434 OLLAMA_MODEL=llama3 dotnet run

# Enable fallback notices
PLUGINDEMO_SHOW_FALLBACK_NOTICE=true dotnet run
```

## Examples

### Plugin Definition
```csharp
public class ClaimPlugin
{
    [KernelFunction]
    [Description("Get claim details by claim ID")]
    public async Task<ClaimDetails> GetClaimByIdAsync(
        [Description("The claim ID to look up")] string claimId)
    {
        // Implementation calls database/API
        return await GetClaimFromDatabase(claimId);
    }

    [KernelFunction]
    [Description("Get all claims for a specific patient")]
    public async Task<List<ClaimSummary>> GetClaimsByPatientAsync(
        [Description("The patient name or ID")] string patient)
    {
        // Implementation retrieves patient claims
        return await GetPatientClaims(patient);
    }
}
```

### Function Registration
```csharp
kernel.Plugins.AddFromType<ClaimPlugin>("Claims");
var claimsPlugin = kernel.Plugins["Claims"];
```

### Automatic Function Calling
```csharp
var result = await kernel.InvokePromptAsync(
    "What is the status of claim CLM-2026-102?",
    new KernelArguments
    {
        [FunctionChoiceBehavior] = FunctionChoiceBehavior.Auto()
    });
```

### Fallback Mechanism
```csharp
try
{
    var result = await kernel.InvokePromptAsync(prompt, arguments);
}
catch (Exception ex) when (IsFunctionCallingError(ex))
{
    // Fallback to manual plugin invocation
    var manualResult = await InvokePluginManually(kernel, prompt);
}
```

## Test Cases

### 1. Function Registration
- **Objective**: Verify plugins are correctly registered
- **Test**: Check plugin availability and function descriptions
- **Expected**: All functions properly registered with descriptions
- **Validation**: Inspect kernel.Plugins collection

### 2. Automatic Function Calling
- **Objective**: Test LLM's ability to call appropriate functions
- **Test**: Ask questions requiring specific functions
- **Expected**: LLM calls correct functions automatically
- **Validation**: Check function execution and results

### 3. Fallback Mechanism
- **Objective**: Verify fallback when Auto() fails
- **Test**: Test with models that don't support function calling
- **Expected**: Graceful fallback to manual invocation
- **Validation**: Check fallback execution and results

### 4. Error Handling
- **Objective**: Test error handling for various scenarios
- **Test**: Handle invalid inputs, missing data, etc.
- **Expected**: Graceful error handling with informative messages
- **Validation**: Check error messages and recovery

### 5. Performance Testing
- **Objective**: Verify function call performance
- **Test**: Execute multiple function calls
- **Expected**: Reasonable response times
- **Validation**: Monitor execution times

## Sample Plugin Functions
```csharp
public class ClaimPlugin
{
    [KernelFunction]
    [Description("Get claim details by claim ID")]
    public async Task<ClaimDetails> GetClaimByIdAsync(string claimId)
    {
        // Returns claim details from database
        return new ClaimDetails
        {
            ClaimId = claimId,
            Patient = "John Doe",
            Amount = 1250.00m,
            Status = "Denied",
            Reason = "Missing pre-authorization"
        };
    }

    [KernelFunction]
    [Description("Get all claims for a specific patient")]
    public async Task<List<ClaimSummary>> GetClaimsByPatientAsync(string patient)
    {
        // Returns patient's claim history
        return new List<ClaimSummary>
        {
            new ClaimSummary { ClaimId = "CLM-2026-102", Amount = 1250.00m, Status = "Denied" },
            new ClaimSummary { ClaimId = "CLM-2026-099", Amount = 850.00m, Status = "Approved" }
        };
    }

    [KernelFunction]
    [Description("Get claims by status")]
    public async Task<List<ClaimSummary>> GetClaimsByStatusAsync(string status)
    {
        // Returns claims filtered by status
        return GetClaims().Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    [KernelFunction]
    [Description("Get claims summary statistics")]
    public async Task<ClaimSummary> GetClaimsSummaryAsync()
    {
        // Returns summary statistics
        return new ClaimSummary
        {
            TotalClaims = 150,
            Approved = 120,
            Denied = 30,
            TotalAmount = 125000.00m
        };
    }
}
```

## Sample Interactions
```
=== PHASE 3: Native Functions / Plugins ===
The LLM has access to these tools:
  Claims.GetClaimById: Get claim details by claim ID
  Claims.GetClaimsByPatient: Get all claims for a specific patient
  Claims.GetClaimsByStatus: Get claims by status
  Claims.GetClaimsSummary: Get claims summary statistics

User: What is the status of claim CLM-2026-102?
AI: Claim CLM-2026-102 has a status of "Denied" with an amount of $1,250.00.

User: Show me all claims for John Doe
AI: John Doe has 2 claims:
- CLM-2026-102: $1,250.00 (Denied)
- CLM-2026-099: $850.00 (Approved)

User: How many claims have been denied?
AI: 30 claims have been denied out of 150 total claims.

User: What's the total amount of all claims?
AI: The total amount of all claims is $125,000.00.
```

## Architecture
```
PluginDemo/
├── Program.cs          # Main application logic
├── Plugins/
│   └── ClaimPlugin.cs  # Plugin implementation
├── PluginDemo.csproj    # Project configuration
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama

## Best Practices
1. **Function Design**: Create focused, single-purpose functions
2. **Description Quality**: Write clear, descriptive function descriptions
3. **Error Handling**: Handle errors gracefully in function implementations
4. **Input Validation**: Validate all function inputs
5. **Performance**: Optimize function implementations for performance

## Production Considerations
- **Database Integration**: Connect to real databases instead of mock data
- **API Integration**: Integrate with external APIs for live data
- **Security**: Implement proper authentication and authorization
- **Monitoring**: Monitor function usage and performance
- **Caching**: Implement caching for frequently accessed data

## Troubleshooting
1. **Function Registration Issues**
   - Check plugin class accessibility
   - Verify function method signatures
   - Ensure proper attribute usage

2. **Function Calling Problems**
   - Verify function descriptions are clear
   - Check for conflicting function names
   - Ensure proper parameter types

3. **Fallback Issues**
   - Verify error detection logic
   - Check manual invocation implementation
   - Ensure fallback provides same results

## Advanced Features
- **Plugin Chaining**: Allow functions to call other functions
- **Dynamic Plugins**: Load plugins dynamically at runtime
- **Plugin Validation**: Validate plugin implementations
- **Plugin Documentation**: Generate documentation for plugins
- **Plugin Testing**: Create comprehensive test suites

## Development Notes
- Implements fallback for llama3's lack of native function calling
- Environment-controlled debug notices for transparency
- Mock data simulates real healthcare claim system
- Error handling covers various edge cases
- Extensible plugin architecture