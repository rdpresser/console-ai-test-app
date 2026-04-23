# DocumentProcessingDemo - Phase 2

## Overview
Phase 2 of the AI learning series, focusing on document processing including PDF/text extraction and chunked summarization using Semantic Kernel.

## Concepts Covered
- Reading PDF files locally with PdfPig (no Azure, no cloud)
- Text chunking: splitting long documents into LLM-sized pieces
- Per-chunk extraction prompt
- Consolidation: merging partial summaries into a final report
- Token budget awareness (why chunking matters)

## Features
- PDF text extraction
- Automatic text chunking
- Intelligent document summarization
- Support for multiple document formats
- Configurable chunk sizes

## Prerequisites
- Ollama running on `http://127.0.0.1:11434`
- Required model: `llama3` (default)
- PdfPig NuGet package for PDF processing

## Environment Variables
- `OLLAMA_ENDPOINT`: Ollama server endpoint (default: `http://127.0.0.1:11434`)
- `OLLAMA_MODEL`: Model to use (default: `llama3`)
- `DOCUMENT_PATH`: Path to document file (default: `Docs/sample.txt`)

## Usage
```bash
# Run with default sample document
cd DocumentProcessingDemo
dotnet run

# Run with custom PDF
DOCUMENT_PATH=/path/to/your/document.pdf dotnet run

# Run with custom configuration
OLLAMA_ENDPOINT=http://localhost:11434 OLLAMA_MODEL=llama3 dotnet run
```

## Examples

### PDF Text Extraction
```csharp
if (extension == ".pdf")
{
    fullText = ExtractTextFromPdf(documentPath);
    Console.WriteLine($"Extracted {fullText.Length} characters from PDF.");
}
```

### Text Chunking
```csharp
var chunks = ChunkText(fullText, maxTokens: 1000);
Console.WriteLine($"Split document into {chunks.Count} chunks.");
```

### Chunk Processing
```csharp
var summaries = new List<string>();
foreach (var chunk in chunks)
{
    var summary = await ProcessChunkAsync(kernel, chunk, chunkIndex++);
    summaries.Add(summary);
}
```

### Consolidation
```csharp
var finalSummary = await ConsolidateSummariesAsync(kernel, summaries);
```

## Test Cases

### 1. PDF Extraction
- **Objective**: Verify PDF text extraction works correctly
- **Test**: Process PDF document with known content
- **Expected**: Complete text extraction with proper formatting
- **Validation**: Compare extracted text with original content

### 2. Text Chunking
- **Objective**: Ensure proper text chunking strategy
- **Test**: Chunk documents of various lengths
- **Expected**: Appropriate chunk sizes and overlap
- **Validation**: Check chunk boundaries and content integrity

### 3. Summarization Quality
- **Objective**: Verify chunk summarization effectiveness
- **Test**: Summarize various document types
- **Expected**: Accurate and relevant summaries
- **Validation**: Manual review of summary quality

### 4. Consolidation Effectiveness
- **Objective**: Test final summary generation
- **Test**: Consolidate multiple chunk summaries
- **Expected**: Coherent final summary
- **Validation**: Check for consistency and completeness

### 5. Performance Testing
- **Objective**: Verify processing performance
- **Test**: Process documents of different sizes
- **Expected**: Reasonable processing times
- **Validation**: Monitor execution times and resource usage

## Sample Document Processing
```
=== PHASE 2: Document Processing ===

Reading document: /path/to/document.pdf
Extracted 15420 characters from PDF.
Split document into 8 chunks.

Processing chunk 1/8...
Summary: This document outlines the pre-authorization policy for elective medical procedures...

Processing chunk 2/8...
Summary: The denial reason code 001 applies to claims submitted without proper pre-authorization...

...

Final Consolidated Summary:
[Comprehensive summary combining all chunk summaries]
```

## Architecture
```
DocumentProcessingDemo/
├── Program.cs          # Main application logic
├── DocumentProcessingDemo.csproj  # Project configuration
├── Docs/               # Sample documents
│   └── sample.txt     # Default sample document
└── bin/               # Build output
```

## Dependencies
- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.Ollama
- UglyToad.PdfPig (for PDF processing)

## Configuration
- **Chunk Size**: Configurable maximum tokens per chunk (default: 1000)
- **Overlap**: Configurable overlap between chunks (default: 100)
- **Max Chunks**: Maximum number of chunks to process (default: 50)

## Best Practices
1. **Document Preparation**: Clean and preprocess documents before extraction
2. **Chunk Strategy**: Use appropriate chunk sizes based on content type
3. **Memory Management**: Handle large documents efficiently
4. **Error Handling**: Graceful handling of malformed documents
5. **Quality Control**: Implement validation for extracted text

## Production Considerations
- **Document Storage**: Implement proper document storage and retrieval
- **Processing Pipeline**: Build robust document processing pipeline
- **Scalability**: Handle large volumes of documents
- **Monitoring**: Track processing metrics and quality
- **Security**: Ensure document handling complies with regulations

## Troubleshooting
1. **PDF Issues**
   - Corrupted PDF files
   - Password-protected PDFs
   - Scanned PDFs (requires OCR)
   - Complex layouts with tables/images

2. **Chunking Problems**
   - Inappropriate chunk sizes
   - Content splitting at bad points
   - Missing context between chunks

3. **Performance Issues**
   - Large document processing times
   - Memory usage with large files
   - Timeout issues with complex documents

## Advanced Features
- **Multi-format Support**: Add support for Word, Excel, etc.
- **OCR Integration**: Add OCR for scanned documents
- **Content Classification**: Categorize document types
- **Metadata Extraction**: Extract structured metadata
- **Batch Processing**: Process multiple documents simultaneously

## Sample Documents
The `Docs/` directory contains:
- `sample.txt`: Sample text document for testing
- Add your own PDF files to test with different document types

## Development Notes
- PdfPig is used for PDF processing (no external dependencies)
- Text chunking strategy can be customized
- Summarization prompts can be adjusted for different document types
- Error handling covers various document corruption scenarios