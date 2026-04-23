# RAGDemo - Phase 4

## Overview
Phase 4 of the AI learning series, focusing on Retrieval-Augmented Generation (RAG) using semantic search and vector embeddings with Semantic Kernel.

## Concepts Covered
- Text embeddings: converting text into float[] vectors
- Cosine similarity: measuring semantic closeness between vectors
- Vector store: storing and querying document embeddings
- RAG pipeline: retrieve relevant context → inject into prompt
- Semantic search without external APIs

## Features
- Local vector embeddings using nomic-embed-text
- Semantic document retrieval
- Context-aware question answering
- Healthcare policy knowledge base
- Configurable similarity thresholds

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required models: `llama3` (chat), `nomic-embed-text` (embeddings)

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Chat model to use (default: `llama3`)
- `OLLAMA_EMBED_MODEL`: Embedding model to use (default: `nomic-embed-text`)

## Usage
```bash
# Run with default settings
cd RAGDemo
dotnet run

# Run with custom models
OLLAMA_MODEL=llama3 OLLAMA_EMBED_MODEL=nomic-embed-text dotnet run

# Run with custom endpoint
OLLAMA_ENDPOINT=http://localhost:11434 dotnet run
```

## Examples

### Knowledge Base Setup
```csharp
var documents = new List<string>
{
    "Pre-authorization policy: All elective surgeries, MRI scans, CT scans, and specialist referrals require prior authorization...",
    "Denial reason code 001 – Missing pre-authorization: The claim was submitted for a service that required prior approval...",
    "Duplicate claim policy: Claims submitted more than once for the same patient, same date of service..."
};
```

### Vector Store Creation
```csharp
var embeddings = new List<float[]>();
foreach (var doc in documents)
{
    var embedding = await embeddingService.GenerateEmbeddingAsync(doc);
    embeddings.Add(embedding);
}
```

### Semantic Search
```csharp
var query = "What do I need for pre-authorization?";
var relevantDocs = FindRelevantDocuments(query, documents, embeddings, topK: 2);
```

### RAG Prompt Construction
```csharp
var ragPrompt = $"""
    You are a healthcare policy expert.
    Use ONLY the following context to answer the question.
    If the context doesn't contain the answer, say "I don't know based on the provided context."

    Context:
    {context}

    Question: {query}
    """;
```

## Test Cases

### 1. Embedding Generation
- **Objective**: Verify text embeddings are generated correctly
- **Test**: Generate embeddings for known documents
- **Expected**: Proper vector representations
- **Validation**: Check embedding dimensions and values

### 2. Semantic Search Accuracy
- **Objective**: Test semantic search effectiveness
- **Test**: Search for various queries with known relevant documents
- **Expected**: Correct retrieval of semantically relevant documents
- **Validation**: Measure retrieval precision and recall

### 3. Context Injection
- **Objective**: Verify context injection in prompts
- **Test**: Ask questions requiring specific document knowledge
- **Expected**: AI uses provided context accurately
- **Validation**: Check responses reference injected context

### 4. RAG Response Quality
- **Objective**: Test overall RAG response quality
- **Test**: Ask complex questions requiring document knowledge
- **Expected**: Accurate, context-aware responses
- **Validation**: Manual review of response quality

### 5. Performance Testing
- **Objective**: Verify RAG pipeline performance
- **Test**: Process multiple queries with varying complexity
- **Expected**: Reasonable response times
- **Validation**: Monitor embedding generation and search times

## Sample RAG Interaction
```
=== PHASE 4: RAG – Retrieval-Augmented Generation ===

Knowledge Base:
1. Pre-authorization policy: All elective surgeries, MRI scans, CT scans...
2. Denial reason code 001 – Missing pre-authorization: Claims submitted...
3. Duplicate claim policy: Claims submitted more than once...

User: What do I need for pre-authorization?
Retrieved context: Pre-authorization policy: All elective surgeries, MRI scans...
AI: Based on the provided context, all elective surgeries, MRI scans, CT scans,
    and specialist referrals require prior authorization before the service date.

User: What happens if I don't get pre-authorization?
Retrieved context: Denial reason code 001 – Missing pre-authorization...
AI: According to the context, claims submitted for services requiring prior
    approval without authorization will be denied. Retroactive authorization
    is not accepted unless the procedure was an emergency.
```

## Architecture
```
RAGDemo/
├── Program.cs          # Main application logic
├── RAGDemo.csproj      # Project configuration
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama
- nomic-embed-text model

## Configuration
- **Top K**: Number of relevant documents to retrieve (default: 2)
- **Similarity Threshold**: Minimum similarity score for relevance (default: 0.5)
- **Max Context Length**: Maximum tokens for context injection (default: 2000)

## Best Practices
1. **Document Quality**: Ensure knowledge base documents are clean and relevant
2. **Chunk Size**: Use appropriate document chunking for embeddings
3. **Query Processing**: Pre-process queries for better semantic matching
4. **Context Management**: Balance context length with relevance
5. **Validation**: Implement validation for retrieved documents

## Production Considerations
- **Vector Database**: Consider dedicated vector databases for large scale
- **Document Management**: Implement proper document lifecycle management
- **Performance Optimization**: Optimize embedding generation and search
- **Monitoring**: Track search accuracy and response quality
- **Scalability**: Handle large document collections efficiently

## Troubleshooting
1. **Embedding Issues**
   - Model availability problems
   - Embedding dimension mismatches
   - Performance issues with large documents

2. **Search Problems**
   - Poor semantic matching
   - Irrelevant document retrieval
   - Similarity threshold issues

3. **Context Issues**
   - Context length limitations
   - Irrelevant context injection
   - Prompt formatting problems

## Advanced Features
- **Hybrid Search**: Combine semantic and keyword search
- **Document Ranking**: Implement relevance scoring
- **Query Expansion**: Expand queries for better results
- **Feedback Loop**: Implement user feedback for improvement
- **Multi-modal**: Add image support for document search

## Knowledge Base Examples
The demo includes healthcare policy documents:
- Pre-authorization policies
- Denial reason codes
- Claim processing procedures
- Appeal processes

## Development Notes
- Uses local embeddings for privacy and performance
- Implements cosine similarity for semantic search
- Context injection prevents hallucination
- Configurable for different document types
- Error handling for various edge cases