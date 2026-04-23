# ConsoleAppAITest - Main Project

## Overview
This is the main entry point project for the AI learning series, demonstrating basic connectivity to Ollama and Semantic Kernel integration.

## Features
- Basic Ollama connectivity testing
- Environment variable configuration
- Error handling for connection issues
- Simple prompt execution

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required models: `llama3` (default)

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Model to use (default: `llama3`)

## Usage
```bash
# Run with default settings
dotnet run

# Run with custom endpoint
OLLAMA_ENDPOINT=http://localhost:11434 dotnet run

# Run with custom model
OLLAMA_MODEL=llama3 dotnet run
```

## Examples
1. **Basic Connectivity Test**
   ```bash
   dotnet run
   ```
   Output:
   ```
   Using Ollama endpoint: http://127.0.0.1:11434
   Using Ollama model: llama3
   [Response from LLM about Semantic Kernel]
   ```

2. **Custom Configuration**
   ```bash
   OLLAMA_ENDPOINT=http://localhost:11434 OLLAMA_MODEL=llama3 dotnet run
   ```

## Error Handling
The project handles common errors:
- Invalid endpoint URLs
- Connection failures
- Model availability issues
- General exceptions

## Test Cases
1. **Successful Connection**
   - Verify Ollama is running
   - Check model availability
   - Execute prompt successfully

2. **Connection Errors**
   - Invalid endpoint format
   - Ollama server not running
   - Network connectivity issues

3. **Configuration Errors**
   - Missing environment variables
   - Invalid model names
   - Malformed URLs

## Architecture
```
ConsoleAppAITest/
├── Program.cs          # Main application logic
├── ConsoleAppAITest.csproj  # Project configuration
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama

## Development
- Target Framework: .NET 10.0
- C# Version: Latest
- Implicit Usings: Enabled
- Nullable Reference Types: Enabled

## Troubleshooting
1. **Connection Issues**
   - Ensure Ollama is running
   - Check endpoint URL format
   - Verify network connectivity

2. **Model Issues**
   - Pull required models: `ollama pull llama3`
   - Check model availability with `ollama list`

3. **Environment Variables**
   - Verify variable names are correct
   - Check for typos in values

## Contributing
This is a foundational project. Contributions should focus on:
- Improving error handling
- Adding more test cases
- Enhancing documentation
- Adding configuration validation