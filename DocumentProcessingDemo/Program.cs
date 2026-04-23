// ============================================================
// PHASE 2: Document Processing – PDF/Text → Chunked Summary
// ============================================================
// Concepts covered:
//   - Reading a PDF file locally with PdfPig (no Azure, no cloud)
//   - Text chunking: splitting long documents into LLM-sized pieces
//   - Per-chunk extraction prompt
//   - Consolidation: merging partial summaries into a final report
//   - Token budget awareness (why chunking matters)
// ============================================================
//
// HOW TO TEST:
//   Option A – Automatic: runs with the bundled Docs/sample.txt
//   Option B – Your own PDF: copy any PDF to Docs/ and set the
//              environment variable DOCUMENT_PATH to its path.
// ============================================================

using System.Text;
using Microsoft.SemanticKernel;
using UglyToad.PdfPig;

var endpoint     = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var model        = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";
var documentPath = Environment.GetEnvironmentVariable("DOCUMENT_PATH")
    ?? Path.Combine(AppContext.BaseDirectory, "Docs", "sample.txt");

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: model, endpoint: endpoint)
    .Build();

Console.WriteLine("=== PHASE 2: Document Processing ===\n");

// ─────────────────────────────────────────────────────────────
// STEP 1 – Extract text from the document
// WHY: LLMs work on text. Before we can summarize, we must
//      extract raw text from the file format (PDF, Word, etc.)
// ─────────────────────────────────────────────────────────────
Console.WriteLine($"Reading document: {documentPath}");

string fullText;
var extension = Path.GetExtension(documentPath).ToLowerInvariant();

if (extension == ".pdf")
{
    fullText = ExtractTextFromPdf(documentPath);
    Console.WriteLine($"Extracted {fullText.Length} characters from PDF.");
}
else
{
    // Fallback: plain text (also useful for pre-processed OCR output)
    fullText = await File.ReadAllTextAsync(documentPath);
    Console.WriteLine($"Read {fullText.Length} characters from text file.");
}

Console.WriteLine();

// ─────────────────────────────────────────────────────────────
// STEP 2 – Chunk the text
// WHY: LLMs have a context window limit (e.g., 8K or 32K tokens).
//      A long document must be split into chunks that fit.
//      Rule of thumb: 1 token ≈ 4 characters for English text.
//      At chunkSize=4000 chars ≈ ~1000 tokens – safe for any model.
// ─────────────────────────────────────────────────────────────
const int ChunkSize    = 4000; // characters per chunk
const int ChunkOverlap = 200;  // overlap to avoid cutting mid-sentence

var chunks = ChunkText(fullText, ChunkSize, ChunkOverlap);
Console.WriteLine($"Split into {chunks.Count} chunks (chunkSize={ChunkSize}, overlap={ChunkOverlap}).\n");

// ─────────────────────────────────────────────────────────────
// STEP 3 – Summarize each chunk individually
// WHY: Each chunk fits within the model's context window.
//      We extract the key points from each piece separately.
// ─────────────────────────────────────────────────────────────
var chunkSummaries = new List<string>();

for (int i = 0; i < chunks.Count; i++)
{
    Console.WriteLine($"Processing chunk {i + 1}/{chunks.Count}...");

    var extractFn = kernel.CreateFunctionFromPrompt("""
        You are a healthcare document analyst.
        Extract the 3 most important facts or findings from the text below.
        Be concise. Use bullet points. No preamble.

        Text:
        {{$chunk}}
        """);

    var chunkResult = await kernel.InvokeAsync(extractFn, new KernelArguments
    {
        ["chunk"] = chunks[i]
    });

    chunkSummaries.Add($"[Chunk {i + 1}]\n{chunkResult}");
    Console.WriteLine($"  Done.\n");
}

// ─────────────────────────────────────────────────────────────
// STEP 4 – Consolidate chunk summaries into a final report
// WHY: After per-chunk extraction, we merge everything into one
//      coherent summary. This is the "Map-Reduce" pattern for docs.
// ─────────────────────────────────────────────────────────────
Console.WriteLine("━━━ Final consolidated report ━━━\n");

var combinedSummaries = string.Join("\n\n", chunkSummaries);

var consolidateFn = kernel.CreateFunctionFromPrompt("""
    You are a senior healthcare auditor writing an executive summary.
    Below are key points extracted from different sections of a document.
    Write a single coherent executive summary (5-8 bullet points max).
    Focus on: financial impact, key risks, and recommended actions.
    Do not repeat the same point twice.

    Extracted points:
    {{$summaries}}
    """);

var finalReport = await kernel.InvokeAsync(consolidateFn, new KernelArguments
{
    ["summaries"] = combinedSummaries
});

Console.WriteLine(finalReport);
Console.WriteLine("\n=== Done ===");

// ──────────────────────────────────────────────────────────────
// Helper: extract all text from a PDF file using PdfPig
// ──────────────────────────────────────────────────────────────
static string ExtractTextFromPdf(string path)
{
    var sb = new StringBuilder();
    using var pdf = PdfDocument.Open(path);
    foreach (var page in pdf.GetPages())
        sb.AppendLine(page.Text);
    return sb.ToString();
}

// ──────────────────────────────────────────────────────────────
// Helper: split text into overlapping chunks
// Overlap ensures a sentence split across chunk boundaries
// still appears in full in at least one chunk.
// ──────────────────────────────────────────────────────────────
static List<string> ChunkText(string text, int chunkSize, int overlap)
{
    var chunks = new List<string>();
    int start  = 0;

    while (start < text.Length)
    {
        int end    = Math.Min(start + chunkSize, text.Length);
        var chunk  = text[start..end];
        chunks.Add(chunk);

        if (end == text.Length) break;
        start += chunkSize - overlap;
    }

    return chunks;
}
