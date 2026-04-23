# ChatHistoryDemo - Phase 1B

## Overview
Phase 1B of the AI learning series, focusing on multi-turn conversation with memory using ChatHistory and Semantic Kernel.

## Concepts Covered
- IChatCompletionService (lower-level than InvokePromptAsync)
- ChatHistory to maintain conversation context across turns
- System prompt to define AI persona and scope
- Stateful sessions: why context matters for complex Q&A

## Features
- Multi-turn conversation with memory
- System prompt for consistent AI persona
- Context injection for real data
- Professional healthcare claim assistance

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required model: `llama3` (default)

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Model to use (default: `llama3`)

## Usage
```bash
# Run with default settings
cd ChatHistoryDemo
dotnet run

# Interactive conversation
# Type your questions and press Enter
# Type 'exit' to quit
```

## Examples

### System Prompt Configuration
```csharp
var history = new ChatHistory("""
    You are a healthcare claim assistant for a U.S. insurance company.
    You help auditors and claim processors understand claim data.
    You are concise, factual, and professional.
    If you don't know something, say so clearly rather than guessing.
    You remember everything said in this conversation.
    """);
```

### Context Injection
```csharp
history.AddUserMessage("""
    I'm reviewing claim CLM-2026-102.
    Patient: John Doe
    Date: 2026-04-20
    Amount: $1,250.00
    Status: Denied
    Reason: Missing pre-authorization documentation
    """);
```

### Multi-Turn Conversation
```csharp
// Turn 1: Initial question
history.AddUserMessage("What was the claim amount for CLM-2026-102?");

// Turn 2: Follow-up question
history.AddUserMessage("What was the denial reason again?");

// Turn 3: Complex question requiring context
history.AddUserMessage("Based on the pre-authorization policy, what should we do?");
```

## Test Cases

### 1. Memory Persistence
- **Objective**: Verify AI remembers previous conversation context
- **Test**: Ask about claim details, then ask follow-up questions
- **Expected**: AI references previous claim information correctly
- **Validation**: Check responses contain accurate historical data

### 2. System Prompt Adherence
- **Objective**: Ensure AI maintains professional persona
- **Test**: Engage in conversation and verify tone and accuracy
- **Expected**: Consistent professional healthcare assistant behavior
- **Validation**: Manual review of conversation quality

### 3. Context Injection Effectiveness
- **Test**: Provide claim data and ask related questions
- **Expected**: AI uses injected data accurately in responses
- **Validation**: Verify responses reference provided claim details

### 4. Conversation Flow
- **Objective**: Test natural conversation progression
- **Test**: Engage in multi-turn conversation
- **Expected**: Smooth context transitions and relevant responses
- **Validation**: Review conversation coherence and relevance

### 5. Error Handling
- **Objective**: Verify graceful handling of unknown information
- **Test**: Ask questions beyond AI's knowledge scope
- **Expected**: AI acknowledges limitations clearly
- **Validation**: Check for appropriate "I don't know" responses

## Sample Conversation Flow
```
=== PHASE 1B: Multi-Turn Chat with Memory ===
Type your question and press Enter. Type 'exit' to quit.
Tip: Ask follow-up questions — the AI remembers the whole conversation.
Example: Ask about claim CLM-2026-102, then ask 'What was that amount again?'

User: What was the claim amount for CLM-2026-102?
AI: The claim amount for CLM-2026-102 was $1,250.00.

User: What was the denial reason again?
AI: The claim was denied due to missing pre-authorization documentation.

User: Based on the pre-authorization policy, what should we do?
AI: According to the pre-authorization policy, all elective surgeries, MRI scans,
    CT scans, and specialist referrals require prior authorization before the
    service date. To resolve this denial, you should submit form PA-2026 with
    the treating physician's clinical notes within 60 days of the denial date.
```

## Architecture
```
ChatHistoryDemo/
├── Program.cs          # Main application logic
├── ChatHistoryDemo.csproj  # Project configuration
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama

## Best Practices
1. **System Prompt Design**: Craft clear, comprehensive system prompts
2. **Context Management**: Balance context length with relevance
3. **Memory Limits**: Be aware of context window limitations
4. **Error Handling**: Handle conversation gracefully
5. **User Experience**: Provide clear interaction instructions

## Production Considerations
- **Session Management**: Implement proper session lifecycle
- **Context Optimization**: Implement context summarization for long conversations
- **Privacy**: Ensure conversation data is handled securely
- **Performance**: Monitor conversation response times
- **Scalability**: Consider conversation volume and storage

## Troubleshooting
1. **Memory Issues**
   - Check context window limits
   - Implement context summarization if needed
   - Monitor conversation length

2. **Conversation Quality**
   - Review system prompt effectiveness
   - Adjust AI persona as needed
   - Provide better context injection

3. **Performance Issues**
   - Monitor response times
   - Consider context optimization
   - Handle timeouts appropriately

## Advanced Features
- **Context Summarization**: Automatically summarize old conversations
- **Session Persistence**: Save and restore conversation sessions
- **Multi-User Support**: Handle multiple concurrent conversations
- **Analytics**: Track conversation quality and user satisfaction
- **Integration**: Connect to external data sources for enhanced responses