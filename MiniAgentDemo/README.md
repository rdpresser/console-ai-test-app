# MiniAgentDemo - Phase 5-6 (Capstone)

## Overview
Capstone project combining all previous phases: RAG + plugins + chat with memory. This is the most advanced demonstration of Semantic Kernel capabilities for healthcare AI applications.

## Concepts Covered
- **Phase 1a**: Prompt templates and structured output
- **Phase 1b**: ChatHistory for stateful conversation
- **Phase 2**: Document processing (text chunking)
- **Phase 3**: Plugins/native functions (live data access)
- **Phase 4**: RAG (semantic retrieval of policy documents)
- **Phase 5-6**: Advanced agent with integrated capabilities

## Features
- **RAG Integration**: Answer policy questions using retrieved context
- **Live Data Access**: Look up claim data using plugins
- **Action Execution**: Submit appeals and other actions via plugins
- **Conversation Memory**: Remember context across multiple turns
- **Structured Reporting**: Generate comprehensive final reports
- **Fallback Mechanism**: Automatic handling of llama3's tool limitations

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required models: `llama3` (chat), `nomic-embed-text` (embeddings)

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Chat model to use (default: `llama3`)
- `OLLAMA_EMBED_MODEL`: Embedding model to use (default: `nomic-embed-text`)
- `MINIAGENT_SHOW_FALLBACK_NOTICE`: Show fallback notices (default: false)

## Usage
```bash
# Run with default settings
cd MiniAgentDemo
dotnet run

# Run with custom configuration
OLLAMA_ENDPOINT=http://localhost:11434 OLLAMA_MODEL=llama3 dotnet run

# Enable fallback notices
MINIAGENT_SHOW_FALLBACK_NOTICE=true dotnet run
```

## Examples

### Agent Capabilities
```csharp
// The agent can:
// 1. Answer policy questions using RAG (no hallucination)
// 2. Look up live claim data using plugins
// 3. Submit appeals using plugins
// 4. Remember the conversation context across turns
// 5. Generate a structured final report
```

### Fallback Mechanism
```csharp
try
{
    // Try automatic function calling
    var result = await kernel.InvokePromptAsync(prompt, arguments);
}
catch (Exception ex) when (IsFunctionCallingError(ex))
{
    // Fallback to manual plugin invocation
    var manualResult = await InvokePluginManually(kernel, prompt);
}
```

### RAG Integration
```csharp
// Retrieve relevant policy documents
var relevantDocs = await RetrieveRelevantDocumentsAsync(query);
var context = string.Join("\n\n", relevantDocs);

// Inject context into prompt
var enhancedPrompt = $"""
    You are a healthcare policy expert.
    Use ONLY the following context to answer questions.
    
    Context: {context}
    
    Question: {query}
    """;
```

## Test Cases

### 1. RAG Integration
- **Objective**: Verify agent uses retrieved context accurately
- **Test**: Ask questions requiring specific policy knowledge
- **Expected**: Agent uses retrieved documents to answer accurately
- **Validation**: Check responses reference retrieved context

### 2. Plugin Integration
- **Objective**: Test live data access via plugins
- **Test**: Ask questions requiring claim data lookup
- **Expected**: Agent calls appropriate plugins and uses results
- **Validation**: Verify plugin execution and result usage

### 3. Conversation Memory
- **Objective**: Test multi-turn conversation capabilities
- **Test**: Engage in extended conversations
- **Expected**: Agent remembers context across turns
- **Validation**: Check context persistence and relevance

### 4. Action Execution
- **Objective**: Test plugin-based action execution
- **Test**: Request actions like appeal submissions
- **Expected**: Agent executes appropriate plugin actions
- **Validation**: Verify action execution and results

### 5. Fallback Mechanism
- **Objective**: Verify graceful handling of tool limitations
- **Test**: Test with models lacking tool support
- **Expected**: Seamless fallback to manual plugin invocation
- **Validation**: Check fallback execution and consistency

### 6. Report Generation
- **Objective**: Test structured report generation
- **Test**: Request comprehensive reports
- **Expected**: Well-structured, comprehensive reports
- **Validation**: Check report format and content quality

## Sample Agent Interactions
```
=== PHASE 5: Mini Agent – Capstone combining all previous phases ===

User: What is the pre-authorization policy for MRI scans?
AI: Based on the retrieved policy documents, all elective surgeries, MRI scans,
    CT scans, and specialist referrals require prior authorization before the
    service date. Retroactive authorization is not accepted unless the procedure
    was an emergency.

User: What's the status of claim CLM-2026-102?
AI: Claim CLM-2026-102 has a status of "Denied" with an amount of $1,250.00.
    The denial reason is "Missing pre-authorization documentation."

User: Can you help submit an appeal for this claim?
AI: I can help you submit an appeal for claim CLM-2026-102. To proceed with
    the appeal submission, I'll need the following information:
    - Clinical notes from the treating physician
    - Supporting documentation for the medical necessity
    - Any additional information that supports the appeal

User: Here are the clinical notes...
AI: Thank you for providing the clinical notes. I will now submit the appeal
    for claim CLM-2026-102 with the provided documentation. The appeal has
    been successfully submitted and you should receive a response within 60 days.

User: Can you generate a summary of our conversation?
AI: Here's a summary of our conversation:
    1. Discussed pre-authorization policy for MRI scans
    2. Reviewed claim CLM-2026-102 status and denial reason
    3. Submitted appeal for the claim with clinical documentation
    4. Appeal successfully submitted with expected 60-day response time
```

## Architecture
```
MiniAgentDemo/
├── Program.cs          # Main application logic
├── Plugins/
│   └── HealthcarePlugin.cs  # Healthcare plugin implementation
├── MiniAgentDemo.csproj    # Project configuration
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama
- nomic-embed-text model

## Best Practices
1. **Agent Design**: Create focused, capable agents with clear roles
2. **Context Management**: Balance context length with relevance
3. **Error Handling**: Implement comprehensive error handling
4. **Plugin Integration**: Design plugins that work well with agent context
5. **User Experience**: Provide clear, helpful responses

## Production Considerations
- **Security**: Implement proper authentication and authorization
- **Monitoring**: Monitor agent performance and usage
- **Scalability**: Handle multiple concurrent agent instances
- **Performance**: Optimize for response times
- **Reliability**: Ensure high availability and fault tolerance

## Troubleshooting
1. **Context Issues**
   - Monitor context window usage
   - Implement context summarization
   - Handle context overflow gracefully

2. **Plugin Problems**
   - Verify plugin availability
   - Check plugin execution errors
   - Ensure proper error handling

3. **Performance Issues**
   - Monitor response times
   - Optimize plugin execution
   - Handle timeouts appropriately

## Advanced Features
- **Multi-modal Support**: Add image and document processing
- **Personalization**: Customize agent behavior per user
- **Learning**: Implement continuous learning from interactions
- **Integration**: Connect to external systems and APIs
- **Analytics**: Track agent performance and user satisfaction

## Development Notes
- This is the capstone project combining all previous phases
- Implements robust fallback for llama3's tool limitations
- Uses both RAG and plugin capabilities simultaneously
- Maintains conversation context across multiple turns
- Generates structured reports and summaries
- Environment-controlled debug notices for transparency

## Integration Patterns
- **RAG + Plugins**: Agent uses both retrieved context and live data
- **Multi-turn Context**: Maintains conversation state
- **Action Execution**: Can perform real actions via plugins
- **Error Recovery**: Graceful handling of various failure modes
- **User Guidance**: Provides clear instructions and feedback