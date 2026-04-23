// ============================================================
// PHASE 4: RAG – Retrieval-Augmented Generation (100% localhost)
// ============================================================
// Concepts covered:
//   - Text embeddings: converting text into float[] vectors
//   - Cosine similarity: measuring semantic closeness between vectors
//   - Vector store: storing and querying document embeddings
//   - RAG pipeline: retrieve relevant context → inject into prompt
//
// PRE-REQUISITE:
//   Pull the embedding model before running:
//   podman exec -it ollama ollama pull nomic-embed-text
//
// WHY RAG matters for the job:
//   The healthcare platform has thousands of policy documents, claim
//   notes, and medical records. RAG lets the LLM answer questions
//   about SPECIFIC company data it was never trained on — without
//   sending that data to any external API.
// ============================================================

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

var endpoint   = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var chatModel  = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";
var embedModel = Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text";

#pragma warning disable SKEXP0070
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: chatModel, endpoint: endpoint)
    .AddOllamaTextEmbeddingGeneration(modelId: embedModel, endpoint: endpoint)
    .Build();
#pragma warning restore SKEXP0070

Console.WriteLine("=== PHASE 4: RAG – Retrieval-Augmented Generation ===\n");

// ─────────────────────────────────────────────────────────────
// STEP 1 – Define the "knowledge base" (documents to index)
// WHY: In production, these come from a database, S3 bucket, or
//      the output of Phase 2 (document processing pipeline).
//      Here we use in-memory strings to focus on the RAG concept.
// ─────────────────────────────────────────────────────────────
var documents = new List<string>
{
    "Pre-authorization policy: All elective surgeries, MRI scans, CT scans, and specialist referrals require prior authorization. Authorization must be obtained before the service date. Retroactive authorization is not accepted unless the procedure was an emergency.",

    "Denial reason code 001 – Missing pre-authorization: The claim was submitted for a service that required prior approval, but no authorization number was provided. To appeal, submit form PA-2026 with the treating physician's clinical notes within 60 days of the denial date.",

    "Duplicate claim policy: Claims submitted more than once for the same patient, same date of service, and same procedure code will be automatically denied as duplicates. Providers must verify claim status in the portal before resubmission. Intentional duplicate billing may result in contract termination.",

    "Emergency services policy: Claims for emergency room visits at out-of-network facilities are covered at in-network rates if the condition constitutes a medical emergency as defined by the prudent layperson standard. Post-stabilization care requires authorization within 24 hours.",

    "Annual benefit limits 2026: Physical therapy is covered for up to 30 visits per calendar year. Mental health outpatient services are covered for up to 60 days per year. Chiropractic care is limited to 20 visits per year. Exceeding these limits requires a medical necessity review.",

    "Claim appeal process: Members and providers may appeal a denied claim within 180 days of the denial notice. Level 1 appeals are reviewed within 30 days for non-urgent claims and 72 hours for urgent medical situations. Level 2 appeals are reviewed by an independent external reviewer.",

    "Coordination of benefits: When a patient is covered by two or more insurance plans, the primary payer is determined by the birthday rule for dependents. The secondary payer covers costs not paid by the primary up to 100% of the allowed amount. Both claims must be submitted simultaneously.",
};

// ─────────────────────────────────────────────────────────────
// STEP 2 – Generate embeddings for all documents
// WHY: An embedding is a float[] vector (e.g., 768 dimensions) that
//      captures the semantic meaning of a text. Similar texts have
//      similar vectors. This allows semantic search, not just keyword search.
// ─────────────────────────────────────────────────────────────
Console.WriteLine("Indexing knowledge base...");

#pragma warning disable SKEXP0001
var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
#pragma warning restore SKEXP0001

var vectorStore = new List<(string Text, ReadOnlyMemory<float> Vector)>();

foreach (var (doc, i) in documents.Select((d, i) => (d, i)))
{
    var vector = await embeddingService.GenerateEmbeddingAsync(doc);
    vectorStore.Add((doc, vector));
    Console.WriteLine($"  Indexed document {i + 1}/{documents.Count}");
}

Console.WriteLine($"\nKnowledge base ready: {vectorStore.Count} documents indexed.\n");

// ─────────────────────────────────────────────────────────────
// STEP 3 – Query loop: RAG in action
// For each question:
//   a) Embed the question
//   b) Find the most similar documents (cosine similarity)
//   c) Inject the retrieved context into the prompt
//   d) The LLM answers using only that context
// ─────────────────────────────────────────────────────────────
var questions = new[]
{
    "My MRI claim was denied. What should I do?",
    "How many physical therapy visits are covered per year?",
    "What happens when two insurance plans cover the same patient?",
    "Can I submit the same claim twice if I made an error?",
};

foreach (var question in questions)
{
    Console.WriteLine($"━━━ Question: {question}");

    // a) Embed the question
    var questionVector = await embeddingService.GenerateEmbeddingAsync(question);

    // b) Rank documents by cosine similarity and take the top 2
    var ranked = vectorStore
        .Select(entry => (entry.Text, Score: CosineSimilarity(questionVector, entry.Vector)))
        .OrderByDescending(x => x.Score)
        .Take(2)
        .ToList();

    Console.WriteLine($"   Top match score: {ranked[0].Score:F4}");

    // c) Build context from retrieved documents
    var context = string.Join("\n\n", ranked.Select((r, i) => $"[Policy {i + 1}]\n{r.Text}"));

    // d) Prompt with injected context (this is the "Augmented" in RAG)
    var ragFn = kernel.CreateFunctionFromPrompt("""
        You are a helpful healthcare insurance assistant.
        Answer the question using ONLY the policy information provided below.
        If the answer is not in the provided context, say "I don't have that information in my current knowledge base."
        Do not use your general training knowledge.

        Policy Context:
        {{$context}}

        Question: {{$question}}
        """);

    var answer = await kernel.InvokeAsync(ragFn, new KernelArguments
    {
        ["context"]  = context,
        ["question"] = question
    });

    Console.WriteLine($"   Answer: {answer}\n");
}

Console.WriteLine("=== Done ===");

// ──────────────────────────────────────────────────────────────
// Cosine similarity: measures angle between two vectors.
// Returns 1.0 = identical meaning, 0.0 = unrelated, -1.0 = opposite.
// No external library needed — pure math on Span<float>.
// ──────────────────────────────────────────────────────────────
static float CosineSimilarity(ReadOnlyMemory<float> a, ReadOnlyMemory<float> b)
{
    var va   = a.Span;
    var vb   = b.Span;
    float dot = 0f, magA = 0f, magB = 0f;

    for (int i = 0; i < va.Length; i++)
    {
        dot  += va[i] * vb[i];
        magA += va[i] * va[i];
        magB += vb[i] * vb[i];
    }

    var magnitude = MathF.Sqrt(magA) * MathF.Sqrt(magB);
    return magnitude == 0f ? 0f : dot / magnitude;
}
