# AI Learning Series - Semantic Kernel with Ollama

## Overview
A comprehensive 6-phase learning series demonstrating Semantic Kernel integration with Ollama for AI-powered healthcare applications. This series progresses from basic connectivity to advanced agent capabilities.

## 🎯 Learning Objectives
- Master Semantic Kernel fundamentals
- Understand AI/GenAI production patterns
- Build scalable AI solutions for healthcare
- Implement RAG, plugins, and conversational AI
- Handle real-world edge cases and limitations

## 📚 Project Structure

### Phase 1: Foundations
- **TextGenerationDemo** (1A) - Prompt templates & structured output
- **ChatHistoryDemo** (1B) - Multi-turn conversation with memory

### Phase 2: Document Processing
- **DocumentProcessingDemo** - PDF/text extraction & chunking

### Phase 3: Function Calling
- **PluginDemo** - Native functions & autonomous tool use

### Phase 4: RAG
- **RAGDemo** - Retrieval-augmented generation with embeddings

### Phase 5-6: Capstone
- **MiniAgentDemo** - Complete agent combining RAG + plugins + chat

### Main Project
- **ConsoleAppAITest** - Basic connectivity & integration testing

## 🚀 Quick Start

### Prerequisites
1. **Install Ollama**: 
   ```bash
   # Windows (using WSL or Docker)
   docker run -d -p 11434:11434 --name ollama ollama/ollama
   
   # Or install directly on your system
   # Follow instructions at https://ollama.ai
   ```

2. **Pull Required Models**:
   ```bash
   ollama pull llama3
   ollama pull nomic-embed-text
   ```

3. **.NET Development Environment**:
   ```bash
   # Install .NET 10.0 SDK
   # https://dotnet.microsoft.com/download/dotnet/10.0
   ```

### Running the Projects

```bash
# Navigate to any project directory
cd ConsoleAppAITest
cd MiniAgentDemo
cd PluginDemo
# etc.

# Run with default settings
dotnet run

# Run with custom configuration
OLLAMA_ENDPOINT=http://localhost:11434 OLLAMA_MODEL=llama3 dotnet run
```

## 🏗️ Architecture Overview

```
AI Learning Series/
├── ConsoleAppAITest/          # Basic connectivity
├── TextGenerationDemo/        # Phase 1A: Prompt templates
├── ChatHistoryDemo/           # Phase 1B: Multi-turn chat
├── DocumentProcessingDemo/    # Phase 2: Document processing
├── RAGDemo/                   # Phase 4: RAG with embeddings
├── PluginDemo/                # Phase 3: Function calling
├── MiniAgentDemo/            # Phase 5-6: Capstone agent
└── README.md                 # This file
```

## 📖 Learning Progression

### Phase 1: Foundations
- **TextGenerationDemo**: Learn prompt engineering and structured output
- **ChatHistoryDemo**: Master conversation state and memory

### Phase 2: Document Processing
- **DocumentProcessingDemo**: Handle real-world documents (PDFs, text)

### Phase 3: Function Calling
- **PluginDemo**: Connect AI to backend systems via plugins

### Phase 4: RAG
- **RAGDemo**: Implement semantic search and context injection

### Phase 5-6: Capstone
- **MiniAgentDemo**: Combine all capabilities into production-ready agent

## 🔧 Key Technologies

### Core Technologies
- **Semantic Kernel**: Microsoft's AI orchestration framework
- **Ollama**: Local LLM inference engine
- **.NET 10.0**: Modern C# runtime
- **C#**: Primary development language

### Supporting Libraries
- **PdfPig**: PDF text extraction
- **System.Text.Json**: JSON processing
- **HttpClient**: HTTP communication
- **xUnit**: Unit testing (recommended)

## 🎯 Production Patterns Demonstrated

### 1. **Error Handling**
- Graceful degradation for model limitations
- Comprehensive exception handling
- User-friendly error messages

### 2. **Configuration Management**
- Environment variable support
- Configurable endpoints and models
- Fallback mechanisms

### 3. **Security & Privacy**
- Local model execution (no data transmission)
- Configurable data handling
- Privacy-focused design

### 4. **Performance Optimization**
- Context management
- Efficient plugin execution
- Response time optimization

### 5. **Scalability**
- Modular architecture
- Plugin extensibility
- Configurable resource usage

## 🧪 Testing Strategy

### Unit Testing (Recommended)
```bash
# Add test projects to each solution
dotnet new xunit -n ProjectName.Tests
dotnet add ProjectName.Tests/ProjectName.Tests.csproj
```

### Integration Testing
- Model connectivity tests
- Plugin functionality tests
- Error scenario validation

### Performance Testing
- Response time monitoring
- Resource usage analysis
- Load testing capabilities

## 🚀 Advanced Usage

### Custom Models
```bash
# Use different models
OLLAMA_MODEL=mistral OLLAMA_EMBED_MODEL=text-embedding-ada-002 dotnet run
```

### Custom Endpoints
```bash
# Use remote Ollama instances
OLLAMA_ENDPOINT=http://remote-server:11434 dotnet run
```

### Debug Mode
```bash
# Enable detailed logging
MINIAGENT_SHOW_FALLBACK_NOTICE=true dotnet run
PLUGINDEMO_SHOW_FALLBACK_NOTICE=true dotnet run
```

## 📊 Use Cases

### Healthcare Applications
- **Claim Processing**: Automate claim review and analysis
- **Policy Compliance**: Ensure regulatory compliance
- **Customer Support**: AI-powered assistance
- **Document Analysis**: Process medical records and policies

### General Business Applications
- **Document Summarization**: Process large document collections
- **Customer Service**: AI-powered support agents
- **Data Analysis**: Extract insights from unstructured data
- **Automation**: Streamline business processes

## 🔍 Troubleshooting

### Common Issues

1. **Connection Problems**
   ```bash
   # Check Ollama status
   ollama list
   
   # Test connectivity
   curl http://localhost:11434/api/tags
   ```

2. **Model Issues**
   ```bash
   # Pull missing models
   ollama pull llama3
   ollama pull nomic-embed-text
   ```

3. **Environment Variables**
   ```bash
   # Verify environment setup
   echo $OLLAMA_ENDPOINT
   echo $OLLAMA_MODEL
   ```

### Performance Optimization
- Monitor memory usage
- Optimize chunk sizes for large documents
- Use appropriate context lengths
- Implement caching strategies

## 🤝 Contributing

This learning series is designed for educational purposes. Contributions should focus on:
- Adding new examples and use cases
- Improving documentation
- Enhancing error handling
- Adding test coverage
- Performance optimizations

## 📚 Additional Resources

### Semantic Kernel Documentation
- [Official Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Getting Started Guide](https://learn.microsoft.com/en-us/semantic-kernel/getting-started/quick-start)
- [Best Practices](https://learn.microsoft.com/en-us/semantic-kernel/concepts/best-practices)

### Ollama Documentation
- [Ollama Guide](https://github.com/ollama/ollama)
- [Model Library](https://ollama.ai/library)
- [API Documentation](https://github.com/ollama/ollama/blob/main/docs/api.md)

### Healthcare AI Resources
- [Healthcare AI Guidelines](https://www.fda.gov/digital-health/artificial-intelligence-machine-learning-software)
- [HIPAA Compliance](https://www.hhs.gov/hipaa/for-professionals/privacy/index.html)

## 🎓 Learning Outcomes

Upon completing this series, you will be able to:
1. **Design AI Solutions**: Architect scalable AI applications
2. **Implement RAG**: Build knowledge-augmented AI systems
3. **Create Plugins**: Connect AI to existing systems
4. **Handle Edge Cases**: Manage real-world AI limitations
5. **Deploy Production Systems**: Build reliable AI applications

This series provides the foundation needed for the **Dev.Pro Senior AI Software Engineer** role, demonstrating practical AI/GenAI implementation skills in a healthcare context.

---

## Prerequisites (Global)

### Required
- **.NET SDK 10.0+** — available at https://dotnet.microsoft.com/download
- **Podman** (or Docker) — for running Ollama container
- **Base LLM model**: `llama3` (automatically fetched on first `ollama run llama3`)

### Setup Ollama Container (runs once)

```powershell
# Start Ollama service on port 11434
podman run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama

# Verify it's running
podman ps | findstr ollama
# Should show: ollama  Up  0.0.0.0:11434->11434/tcp

# Download llama3 model (one-time, ~4GB)
podman exec -it ollama ollama run llama3
# First run may take 1-2 min to download. Ctrl+C when done.

# Check available models
podman exec -it ollama ollama list
```

### Verify Connectivity

```powershell
# Test from Windows host
Test-NetConnection -ComputerName 127.0.0.1 -Port 11434
Invoke-WebRequest http://127.0.0.1:11434
# Expected: Status 200 and body contains "Ollama is running"
```

---

## Project Details & How to Run

### 0. ConsoleAppAITest (Baseline)

**Purpose**: Verify basic Semantic Kernel + Ollama connectivity.

**Concepts**: Configuration, endpoint setup, health checks, error handling.

**What it does**:
- Reads OLLAMA_ENDPOINT and OLLAMA_MODEL environment variables
- Performs a readiness check against Ollama's `/api/tags` endpoint
- Sends a sample Portuguese-language prompt to llama3
- Prints the full LLM response

**Run**:
```powershell
cd e:\Projects\ConsoleAppAITest
dotnet run --project ConsoleAppAITest
```

**Environment Variables** (optional; defaults shown):
```powershell
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
dotnet run --project ConsoleAppAITest
```

**Expected Output**:
```
Using Ollama endpoint: http://127.0.0.1:11434
Using Ollama model: llama3
[Ollama readiness check...]
Olá!
[Response from llama3 about Semantic Kernel in Portuguese...]
```

---

### 1A. TextGenerationDemo (Phase 1 – Structured Output)

**Purpose**: Learn prompt templating, few-shot prompting, and forcing structured JSON output from LLMs.

**Concepts Covered**:
- `KernelArguments` and named variable substitution (template pattern)
- Few-shot prompting to enforce consistent output format
- Forcing JSON output and parsing with `System.Text.Json`
- Why never concatenate user data directly into prompts

**What it does**:
- **Demo 1**: Uses a prompt template with named placeholders (`{{$claim_id}}`, etc.) to summarize healthcare claim data. Shows the production pattern of separating templates from data.
- **Demo 2**: Demonstrates few-shot prompting with 3 examples to teach the LLM an exact output format. Proves that without examples, LLM output format varies.
- **Demo 3**: Forces JSON-only output, attempts to parse the result, and handles markdown-wrapped JSON gracefully.

**Run**:
```powershell
dotnet run --project TextGenerationDemo
```

**Environment Variables**:
```powershell
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
dotnet run --project TextGenerationDemo
```

**Expected Output**:
```
=== PHASE 1A: Text Generation ===

Raw input: {"ClaimId":"CLM-2026-102", ...}

━━━ Demo 1: Prompt template with named variables ━━━
[2-sentence claim summary for auditor]

━━━ Demo 2: Few-shot prompting (consistent format) ━━━
CLAIM CLM-2026-102 | DENIED | Action: Send denial letter requesting missing surgical notes...

━━━ Demo 3: Force JSON output and parse it ━━━
LLM output:
{"risk_level":"High", "action_required":"...", ...}

Parsed fields:
  risk_level: High
  action_required: Send to medical review
```

---

### 1B. ChatHistoryDemo (Phase 1 – Stateful Conversation)

**Purpose**: Build multi-turn conversations where the LLM remembers context across messages.

**Concepts Covered**:
- `ChatHistory` object: the state container for a conversation
- `IChatCompletionService`: the lower-level service API (vs. `InvokePromptAsync`)
- System prompt: defines the AI's persona and scope for the entire session
- Why stateful sessions matter: follow-up questions, context-dependent logic

**What it does**:
- Defines a system prompt that sets the AI as a healthcare claim assistant
- Pre-seeds the ChatHistory with example claim data (simulating a retrieved database record)
- Enters an interactive loop where each user message is added to history
- Each assistant reply is also added to history, so the next turn "remembers"
- The LLM can answer follow-up questions like "What was that amount again?" without re-explaining

**Run**:
```powershell
dotnet run --project ChatHistoryDemo
```

**Environment Variables**:
```powershell
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
dotnet run --project ChatHistoryDemo
```

**Interaction Example**:
```
Assistant: I have loaded 3 claim records. How can I help you?

You: What is the status of claim CLM-2026-102?
Assistant: CLM-2026-102 is for patient John Doe... Status is DENIED because of missing pre-authorization...

You: Can I appeal that?
Assistant: Yes, you can appeal within 180 days. You'll need form PA-2026 and physician clinical notes...

You: What was that amount again?
Assistant: The claim amount was $1,250 for an MRI scan.
[^ Notice: AI remembered without being re-told the claim amount]

You: exit
```

---

### 2. DocumentProcessingDemo (Phase 2 – Document Pipeline)

**Purpose**: Extract text from PDFs, chunk large documents, and summarize via LLM (100% localhost, no Azure).

**Concepts Covered**:
- Text extraction from PDF using PdfPig (Apache 2.0, pure .NET)
- Chunking strategy: splitting text into LLM-sized pieces
- Per-chunk extraction prompts: summarizing each chunk independently
- Consolidation: merging partial summaries into a final report
- Token budget awareness: why chunking matters

**What it does**:
1. Reads a text or PDF file from disk (default: `Docs/sample.txt` included)
2. Splits the document into chunks of ~4000 characters with 200-char overlap
3. Sends each chunk to the LLM with an extraction prompt ("Find 3 key points")
4. Consolidates all chunk summaries into a final executive summary
5. Outputs the final report

**Run**:
```powershell
dotnet run --project DocumentProcessingDemo
```

**Environment Variables**:
```powershell
# Default: reads from Docs/sample.txt
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
$env:DOCUMENT_PATH = "path/to/your/file.pdf"  # Optional: override default

dotnet run --project DocumentProcessingDemo
```

**Expected Output**:
```
=== PHASE 2: Document Processing ===

Reading document: .../Docs/sample.txt
Read 8945 characters from text file.

Split into 3 chunks (chunkSize=4000, overlap=200).

Processing chunk 1/3...
  Done.

Processing chunk 2/3...
  Done.

Processing chunk 3/3...
  Done.

━━━ Final consolidated report ━━━

• Claims processed in Q1 2026: 847 total, 695 approved, 101 denied
• Primary denial driver: missing pre-authorization (38% of denials)
• Recommended action: implement automated pre-auth validation at claim intake
• Expected impact: reduce denial rate by 6-8%
```

---

### 3. PluginDemo (Phase 3 – Native Functions & Function Calling)

**Purpose**: Show how the LLM autonomously decides which C# function to call based on the user's question. **This is the most important concept for the job.**

**Concepts Covered**:
- `[KernelFunction]` attribute: exposes C# methods as AI tools
- `[Description]`: tells the LLM what the function does and what each parameter means
- `FunctionChoiceBehavior.Auto()`: enables the LLM to emit "tool call" requests
- Tool use / function calling: the bridge between AI and your backend systems
- Simulated database: in production, functions call SQL Server, REST APIs, or message queues

**What it does**:
- Defines a `ClaimPlugin` class with 4 functions:
  - `GetClaimById(claimId)` → returns full claim details
  - `GetClaimsByPatient(patientName)` → searches patient claims
  - `GetClaimsByStatus(status)` → filters by Approved/Denied/Pending
  - `GetClaimsSummary()` → counts and sums by status
- Demo 1: Asks 4 questions; the LLM calls the right function automatically for each
- Demo 2: Shows manual function invocation (fallback for models that don't support auto calling)

**Run**:
```powershell
dotnet run --project PluginDemo
```

**Environment Variables**:
```powershell
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
dotnet run --project PluginDemo
```

**Expected Output**:
```
=== PHASE 3: Native Functions / Plugins ===

[If auto function calling works with your model:]
━━━ Question: What is the status of claim CLM-2026-102? Give me all the details.
Answer: Claim CLM-2026-102 is for patient John Doe... MRI scan, $1,250, Status: DENIED...

━━━ Question: Show me all claims for the patient named Maria Silva.
Answer: [LLM called GetClaimsByPatient("Maria Silva") automatically]

[If auto calling not supported:]
[Demo 2: Manual plugin invocation...]
Direct GetClaimById: CLM-2026-115 | Robert Chen | Emergency Room Visit...

Direct GetClaimsSummary:
Claims Summary:
  Approved: 1 claims | Total: $14,800.00
  Denied: 1 claims | Total: $1,250.00
  Pending: 2 claims | Total: $3,200.00
```

---

### 4. RAGDemo (Phase 4 – Retrieval-Augmented Generation)

**Purpose**: Add semantic search to the LLM. The AI answers questions about company-specific documents (policies, procedures) without needing to be retrained.

**Concepts Covered**:
- Text embeddings: converting text into vectors (e.g., 768 floats)
- Cosine similarity: measuring semantic closeness between vectors
- Vector store: storing and querying document vectors in-memory
- RAG pipeline: embed question → find similar docs → inject into prompt → generate answer
- Why RAG prevents hallucination: the LLM can only answer from retrieved context

**What it does**:
1. Defines 7 policy documents (pre-authorization, denials, appeals, coordination of benefits, etc.)
2. Embeds each document using Ollama's `nomic-embed-text` model
3. Stores in-memory vector store
4. For each of 4 test questions:
   - Embeds the question
   - Finds the 2 most similar policy documents (cosine similarity)
   - Injects retrieved text into a prompt
   - The LLM answers using ONLY that context
5. Prevents the LLM from using general knowledge (ensuring accuracy on company policies)

**PRE-REQUISITE**: Pull embedding model once
```powershell
podman exec -it ollama ollama pull nomic-embed-text
# ~280MB download, takes 30 sec–1 min
```

**Run**:
```powershell
dotnet run --project RAGDemo
```

**Environment Variables**:
```powershell
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
$env:OLLAMA_EMBED_MODEL = "nomic-embed-text"  # Must be downloaded first
dotnet run --project RAGDemo
```

**Expected Output**:
```
=== PHASE 4: RAG – Retrieval-Augmented Generation ===

Indexing knowledge base...
  Indexed document 1/7
  Indexed document 2/7
  ...
  Indexed document 7/7

Knowledge base ready: 7 documents indexed.

━━━ Question: My MRI claim was denied. What should I do?
   Top match score: 0.7234
   Answer: You should submit an appeal using form PA-2026 within 60 days...

━━━ Question: How many physical therapy visits are covered per year?
   Top match score: 0.6891
   Answer: The annual limit is 30 physical therapy visits per calendar year...

━━━ Question: What happens when two insurance plans cover the same patient?
   Top match score: 0.8102
   Answer: This is coordination of benefits. The primary payer is determined by birthday rule...
```

---

### 5. MiniAgentDemo (Phase 5 – Capstone Agent)

**Purpose**: Combine all previous phases into a single agent that can:
- Answer policy questions using RAG (Phase 4)
- Look up live claim data using plugins (Phase 3)
- Remember conversation context (Phase 1b)
- Generate structured reports (Phase 1a)

**What it does**:
1. Loads policy knowledge base (RAG) with 6 policy documents
2. Registers the `HealthcarePlugin` (3 functions: GetClaimStatus, SubmitAppeal, GetRequiredDocuments)
3. Enters an interactive chat loop
4. For each user message:
   - Retrieves relevant policies via RAG
   - Injects policy context into the prompt
   - Lets the LLM call plugins if needed (e.g., "get me claim CLM-2026-102")
   - Maintains conversation history across turns
5. Special command `report` generates a structured session summary

**PRE-REQUISITE**: (same as RAGDemo)
```powershell
podman exec -it ollama ollama pull nomic-embed-text
```

**Run**:
```powershell
dotnet run --project MiniAgentDemo
```

**Environment Variables**:
```powershell
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
$env:OLLAMA_EMBED_MODEL = "nomic-embed-text"
dotnet run --project MiniAgentDemo
```

**Interaction Example**:
```
Agent capabilities: policy Q&A (RAG) + live claim lookup + appeal submission

You: What is the status of claim CLM-2026-102?
Agent: Claim CLM-2026-102 is for John Doe. MRI – Lumbar Spine, $1,250, Status: DENIED...
       Reason: Missing pre-authorization.

You: What documents do I need to appeal?
Agent: Based on the denial reason (missing pre-auth), you'll need:
       (1) Form PA-2026, (2) Clinical notes from physician, (3) Referral letter from PCP

You: Can you submit the appeal for me?
Agent: I'll submit the appeal now. Confirmation: APL-20260423-102. Review within 30 business days.

You: report
[Generates structured report of the session]

SUMMARY: Reviewed claim CLM-2026-102, generated appeal documents, submitted appeal.
CLAIMS REVIEWED: CLM-2026-102
ACTIONS TAKEN: Appeal submitted (APL-20260423-102)
OPEN ITEMS: Follow up on appeal status in 30 days

You: exit
```

---

## How to Run All Projects at Once

### Build all projects
```powershell
cd e:\Projects\ConsoleAppAITest
dotnet build ConsoleAppAITest.slnx
```

### Run specific projects via CLI
```powershell
# List of project run commands
dotnet run --project ConsoleAppAITest           # Baseline
dotnet run --project TextGenerationDemo         # Phase 1a
dotnet run --project ChatHistoryDemo            # Phase 1b
dotnet run --project DocumentProcessingDemo     # Phase 2
dotnet run --project PluginDemo                 # Phase 3
dotnet run --project RAGDemo                    # Phase 4 (needs nomic-embed-text)
dotnet run --project MiniAgentDemo              # Phase 5 (needs nomic-embed-text)
```

### Or use Visual Studio 2026
1. Open `ConsoleAppAITest.slnx`
2. In the "Startup Project" dropdown, select the project you want to run
3. Press `F5` (Debug) or `Ctrl+F5` (Run without debugging)

---

## Environment Setup Quick Reference

### Windows PowerShell

```powershell
# Set environment variables for current session
$env:OLLAMA_ENDPOINT = "http://127.0.0.1:11434"
$env:OLLAMA_MODEL = "llama3"
$env:OLLAMA_EMBED_MODEL = "nomic-embed-text"

# Verify
$env:OLLAMA_ENDPOINT
```

### Command Prompt (cmd.exe)

```cmd
set OLLAMA_ENDPOINT=http://127.0.0.1:11434
set OLLAMA_MODEL=llama3
set OLLAMA_EMBED_MODEL=nomic-embed-text

REM Verify
echo %OLLAMA_ENDPOINT%
```

### Persistent (Windows System Properties)

1. Right-click "This PC" → Properties
2. Advanced system settings → Environment Variables
3. Add new user variables:
   - `OLLAMA_ENDPOINT` = `http://127.0.0.1:11434`
   - `OLLAMA_MODEL` = `llama3`
   - `OLLAMA_EMBED_MODEL` = `nomic-embed-text`

---

## Troubleshooting

### "Connection refused" error
```
Connection error talking to Ollama: No connection could be made because the target machine actively refused it.
```
**Fix**:
1. Verify Ollama is running: `podman ps | findstr ollama`
2. If not running: `podman start ollama`
3. Check port: `Test-NetConnection -ComputerName 127.0.0.1 -Port 11434`

### "Model not found" error
```
Ollama readiness check failed with status code 404.
```
**Fix**:
1. List available models: `podman exec -it ollama ollama list`
2. Pull llama3: `podman exec -it ollama ollama pull llama3`
3. Pull embedding model (for Phase 4–5): `podman exec -it ollama ollama pull nomic-embed-text`

### "Function calling not supported" (Phase 3)
```
[Auto function calling not supported by this model build]
```
**Note**: Ollama's llama3 may not support native function calling. The code falls back to manual invocation (Demo 2), which still works. For full function calling support, try:
```powershell
podman exec -it ollama ollama pull llama3.1
```

### "Token limit exceeded" (Phase 2–4)
If the document is very large, chunk size may be too big:
1. Open `DocumentProcessingDemo/Program.cs`
2. Change `const int ChunkSize = 4000;` to a smaller value (e.g., `2000`)
3. Rebuild and rerun

### "404 Not Found - Embedding model" (Phase 4–5)
```
System.Net.Http.HttpRequestException: Response status code does not indicate success: 404 (Not Found).
```
**Cause**: RAGDemo and MiniAgentDemo require the `nomic-embed-text` model for local embeddings.

**Fix**: Pull the embedding model once (before running Phases 4–5):
```powershell
podman exec -it ollama ollama pull nomic-embed-text
# Downloads ~280MB, takes 30 sec–1 min
```

After pulling, Phases 4 and 5 will work.

---

## Learning Path

1. **Start with Baseline** (`ConsoleAppAITest`) — 2 min
   - Verify connectivity
   - Understand environment setup

2. **Phase 1a** (`TextGenerationDemo`) — 10 min
   - Learn prompt templates and few-shot prompting
   - Understand structured output

3. **Phase 1b** (`ChatHistoryDemo`) — 15 min (interactive)
   - Interact with a stateful chat bot
   - See memory in action

4. **Phase 2** (`DocumentProcessingDemo`) — 10 min
   - Upload a test PDF or text file
   - Watch document chunking and summarization

5. **Phase 3** (`PluginDemo`) — 10 min
   - See function calling in action
   - Understand how AI calls backend code

6. **Phase 4** (`RAGDemo`) — 15 min
   - Query a policy database
   - Observe semantic search preventing hallucination

7. **Phase 5** (`MiniAgentDemo`) — 20+ min (interactive)
   - Combine all techniques
   - Interact with a full-featured agent
   - Type `report` to see structured output

**Total time**: ~90 minutes for the full learning path.

---

## Key Concepts Summary

| Concept | Where Taught | Why It Matters |
|---------|--------------|-------------------|
| Prompt Templates | Phase 1a | Separate data from logic; prevent injection attacks |
| Few-Shot Prompting | Phase 1a | Enforce consistent LLM output format |
| JSON Extraction | Phase 1a | Transform LLM text into structured data |
| ChatHistory | Phase 1b | Build stateful applications with context memory |
| Document Chunking | Phase 2 | Handle long documents beyond LLM context limits |
| Per-Chunk Summarization | Phase 2 | Implement the "Map-Reduce" pattern for documents |
| Native Functions | Phase 3 | Bridge AI and backend systems (SQL, APIs, queues) |
| Function Calling | Phase 3 | Let the LLM autonomously decide which function to call |
| Embeddings | Phase 4 | Convert text to vectors for semantic search |
| Cosine Similarity | Phase 4 | Measure text similarity without keywords |
| RAG Pipeline | Phase 4 | Retrieve relevant context before prompting |
| Agent Orchestration | Phase 5 | Combine RAG, plugins, chat, and structured output |

---

## Production Readiness Notes

These projects teach **localhost learning**. To deploy to production:

1. **Move from Ollama to cloud LLMs**:
   - Replace `AddOllamaChatCompletion()` with `AddAzureOpenAIChatCompletion()` or equivalent
   - Use Azure OpenAI, AWS Bedrock, or Anthropic Claude API

2. **Replace in-memory vector store**:
   - Use Azure Cognitive Search, Pinecone, or pgvector (Postgres)

3. **Add observability**:
   - Log all function calls and LLM queries
   - Monitor token usage and costs
   - Add Application Insights for production diagnostics

4. **Implement caching**:
   - Cache embeddings to avoid recomputing
   - Cache LLM responses for identical questions

5. **Governance**:
   - Add prompt guards (e.g., Prompt Shields)
   - Implement content filtering for healthcare data
   - Audit all AI decisions (required in regulated industries)

---

## Next Steps After Learning

Once you've worked through all 5 phases:

1. **Apply to your codebase**: Add a RAG plugin to search your company's documentation
2. **Experiment with other models**: Try `mistral` or `neural-chat` instead of `llama3`
3. **Build a real agent**: Combine document processing + RAG + plugins for your specific domain
4. **Transition to Python + LangChain**: Now that you understand the concepts in .NET, learning them in Python becomes straightforward (the ideas are the same)

---

## Resources

- **Microsoft Semantic Kernel**: https://github.com/microsoft/semantic-kernel
- **Ollama**: https://ollama.ai
- **PdfPig**: https://github.com/UglyToad/PdfPig
- **RAG Patterns**: https://microsoft.github.io/semantic-kernel/features/retrieval-augmented-generation/
- **Healthcare Data Privacy (HIPAA)**: https://www.hhs.gov/hipaa/

---

## License

This learning solution uses open-source libraries under their respective licenses:
- **Semantic Kernel**: MIT
- **Ollama**: MIT
- **PdfPig**: Apache 2.0
- **UglyToad.PdfPig**: Apache 2.0

---

## Support & Contributions

For issues or questions:
1. Check the Troubleshooting section above
2. Verify Ollama is running and models are pulled
3. Confirm environment variables are set correctly
4. Check connection to `http://127.0.0.1:11434`

Good luck, and happy learning! 🚀
