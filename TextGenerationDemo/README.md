# TextGenerationDemo - Phase 1A

## Overview
Phase 1A of the AI learning series, focusing on text generation using prompt templates and structured output with Semantic Kernel.

## Concepts Covered
- KernelArguments and prompt templates with named variables
- Few-shot prompting to enforce consistent output format
- Forcing JSON output and parsing it with System.Text.Json
- Professional healthcare claim analysis

## Features
- Raw data processing from healthcare systems
- Prompt template separation from data
- JSON structured output
- Professional claim analysis

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required model: `llama3` (default)

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Model to use (default: `llama3`)

## Usage
```bash
# Run with default settings
cd TextGenerationDemo
dotnet run

# Run with custom configuration
OLLAMA_ENDPOINT=http://localhost:11434 OLLAMA_MODEL=llama3 dotnet run
```

## Examples

### Demo 1: Prompt Template with Named Variables
```csharp
var summaryTemplate = """
    You are a healthcare claim analyst.
    Write a concise 2-sentence professional summary for an auditor.

    Claim ID:  {{$claim_id}}
    Patient:   {{$patient}}
    Date:      {{$date}}
    Amount:    ${{$amount}}
    Status:    {{$status}}
    """;
```

### Demo 2: Few-Shot Prompting for Consistent Format
```csharp
var structuredTemplate = """
    You are a healthcare claim analyst.
    Analyze the claim and respond in EXACTLY this JSON format:
    {
        "summary": "1-2 sentence summary",
        "risk_level": "LOW|MEDIUM|HIGH",
        "action_required": "string"
    }

    Claim: {{$claim}}
    """;
```

### Demo 3: Professional Analysis with Guidelines
```csharp
var analysisTemplate = """
    You are a senior healthcare claim auditor.
    Analyze this claim and provide professional guidance.

    Guidelines:
    - Be concise and factual
    - Focus on compliance aspects
    - Reference specific policy sections
    - Avoid speculation

    Claim: {{$claim}}
    """;
```

## Test Cases

### 1. Template Variable Substitution
- **Objective**: Verify named variables are correctly substituted
- **Expected**: All `{{$variable}}` placeholders replaced with actual values
- **Validation**: Check output contains correct claim information

### 2. JSON Structure Enforcement
- **Objective**: Ensure JSON output format is maintained
- **Expected**: Valid JSON response with required fields
- **Validation**: Parse JSON and verify schema compliance

### 3. Professional Tone Consistency
- **Objective**: Maintain professional healthcare analysis tone
- **Expected**: Formal, factual language appropriate for auditors
- **Validation**: Manual review of output language

### 4. Data Privacy Compliance
- **Objective**: Ensure sensitive data is handled appropriately
- **Expected**: No unnecessary exposure of PHI
- **Validation**: Review output for data exposure

## Sample Data
```csharp
var claim = new
{
    ClaimId   = "CLM-2026-102",
    Patient   = "John Doe",
    Date      = "2026-04-20",
    Amount    = 1250.00m,
    Status    = "Denied",
    Reason    = "Missing pre-authorization documentation",
    Provider  = "City General Hospital",
    Procedure = "MRI – Lumbar Spine"
};
```

## Architecture
```
TextGenerationDemo/
├── Program.cs          # Main application logic
├── TextGenerationDemo.csproj  # Project configuration
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama
- System.Text.Json

## Best Practices
1. **Template Separation**: Keep templates separate from data
2. **Input Validation**: Validate all input data before processing
3. **Output Sanitization**: Ensure output doesn't expose sensitive data
4. **Error Handling**: Handle parsing and execution errors gracefully
5. **Consistency**: Use consistent formatting across templates

## Troubleshooting
1. **Template Errors**
   - Check for unmatched variable names
   - Verify template syntax is correct
   - Ensure all required variables are provided

2. **JSON Parsing Issues**
   - Validate JSON structure before parsing
   - Handle malformed JSON gracefully
   - Use proper error handling for parsing exceptions

3. **Model Performance**
   - Monitor response times
   - Handle timeouts appropriately
   - Consider token limits for complex templates

## Production Considerations
- **Template Management**: Store templates in configuration files
- **Data Validation**: Implement strict input validation
- **Output Validation**: Validate structured outputs
- **Logging**: Log template execution and results
- **Performance**: Cache frequently used templates
- **Security**: Implement proper data sanitization