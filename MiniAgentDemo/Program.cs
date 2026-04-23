// ============================================================
// PHASE 5: Mini Agent – Capstone combining all previous phases
// ============================================================
// This project combines:
//   Phase 1a – Prompt templates and structured output
//   Phase 1b – ChatHistory for stateful conversation
//   Phase 2  – Document processing (text chunking)
//   Phase 3  – Plugins/native functions (live data access)
//   Phase 4  – RAG (semantic retrieval of policy documents)
//
// The agent can:
//   1. Answer policy questions using RAG (no hallucination)
//   2. Look up live claim data using plugins
//   3. Submit appeals using plugins
//   4. Remember the conversation context across turns
//   5. Generate a structured final report
//
// PRE-REQUISITE: Pull embedding model if not done yet:
//   podman exec -it ollama ollama pull nomic-embed-text
// ============================================================

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Embeddings;
using MiniAgentDemo.Plugins;

var endpoint   = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var chatModel  = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";
var embedModel = Environment.GetEnvironmentVariable("OLLAMA_EMBED_MODEL") ?? "nomic-embed-text";

// ── Build kernel with chat + embeddings + plugin ──
#pragma warning disable SKEXP0070
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: chatModel, endpoint: endpoint)
    .AddOllamaTextEmbeddingGeneration(modelId: embedModel, endpoint: endpoint)
    .Build();
#pragma warning restore SKEXP0070

kernel.Plugins.AddFromType<HealthcarePlugin>("Healthcare");

// ─────────────────────────────────────────────────────────────
// PHASE 4 PART: Build the RAG knowledge base from policy docs
// ─────────────────────────────────────────────────────────────
var policyDocuments = new[]
{
    "Pre-authorization policy: All elective surgeries, MRI scans, CT scans, and specialist referrals require prior authorization before the service date. Retroactive authorization is not accepted unless the procedure was an emergency.",
    "Denial reason code 001 – Missing pre-authorization: To appeal, submit form PA-2026 with the treating physician's clinical notes within 60 days of the denial date.",
    "Claim appeal process: Members and providers may appeal a denied claim within 180 days of the denial notice. Level 1 appeals are reviewed within 30 days for non-urgent claims and 72 hours for urgent medical situations.",
    "Emergency services policy: Claims for emergency room visits at out-of-network facilities are covered at in-network rates if the condition constitutes a medical emergency. Post-stabilization care requires authorization within 24 hours.",
    "Annual benefit limits 2026: Physical therapy is covered for up to 30 visits per calendar year. Mental health outpatient services are covered for up to 60 days per year.",
    "Required documents for appeal: Missing pre-authorization denials require form PA-2026, physician clinical notes, and a referral letter. Medical record denials require full records, discharge summary, and lab results.",
};

Console.WriteLine("=== PHASE 5: Mini Healthcare Agent ===");
Console.WriteLine("Building policy knowledge base...");

#pragma warning disable SKEXP0001
var embeddingService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
#pragma warning restore SKEXP0001

var vectorStore = new List<(string Text, ReadOnlyMemory<float> Vector)>();
foreach (var doc in policyDocuments)
{
    var vector = await embeddingService.GenerateEmbeddingAsync(doc);
    vectorStore.Add((doc, vector));
}

Console.WriteLine($"Knowledge base ready: {vectorStore.Count} policy documents indexed.");
Console.WriteLine("Agent capabilities: policy Q&A (RAG) + live claim lookup + appeal submission\n");
Console.WriteLine("Commands: type your question | 'report' for session summary | 'exit' to quit\n");

// ─────────────────────────────────────────────────────────────
// PHASE 1B PART: Stateful chat with system prompt
// ─────────────────────────────────────────────────────────────
var chatHistory = new ChatHistory("""
    You are a healthcare claims assistant agent with two capabilities:
    1. You can answer policy questions using the provided policy context.
    2. You have tools (Healthcare plugin functions) to look up live claim data and submit appeals.

    Rules:
    - For policy questions: use only the provided policy context, not general knowledge.
    - For claim data questions: use your tools (GetClaimStatus, SubmitAppeal, GetRequiredDocuments).
    - Be concise, professional, and factual.
    - If you cannot answer from context or tools, say so clearly.
    """);

var chat = kernel.GetRequiredService<IChatCompletionService>();

// Track session activity for the final report
var sessionLog = new List<(string Role, string Content)>();

#pragma warning disable SKEXP0070
var executionSettings = new OllamaPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
#pragma warning restore SKEXP0070

// ─────────────────────────────────────────────────────────────
// PHASE 4 PART: RAG retrieval helper
// ─────────────────────────────────────────────────────────────
async Task<string> RetrievePolicyContext(string query)
{
    var queryVector = await embeddingService.GenerateEmbeddingAsync(query);
    var topDocs = vectorStore
        .Select(e => (e.Text, Score: CosineSimilarity(queryVector, e.Vector)))
        .OrderByDescending(x => x.Score)
        .Take(2)
        .Where(x => x.Score > 0.3f) // Only include relevant docs
        .ToList();

    return topDocs.Any()
        ? string.Join("\n\n", topDocs.Select((d, i) => $"[Policy {i + 1}]\n{d.Text}"))
        : string.Empty;
}

// ─────────────────────────────────────────────────────────────
// Main agent loop
// ─────────────────────────────────────────────────────────────
while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

    // ── Generate session report ──
    if (input.Equals("report", StringComparison.OrdinalIgnoreCase))
    {
        await GenerateSessionReport(kernel, sessionLog);
        continue;
    }

    sessionLog.Add(("user", input));

    // ── RAG: retrieve relevant policy context for this query ──
    var policyContext = await RetrievePolicyContext(input);

    // ── Inject RAG context into the message if relevant docs found ──
    var userMessage = string.IsNullOrEmpty(policyContext)
        ? input
        : $"""
          Policy context (use this to answer policy questions):
          {policyContext}

          User question: {input}
          """;

    chatHistory.AddUserMessage(userMessage);

    try
    {
        // ── Let the agent respond (may call plugins automatically) ──
        var response = await chat.GetChatMessageContentAsync(
            chatHistory, executionSettings, kernel);

        var reply = response.Content ?? "(no response)";
        chatHistory.AddAssistantMessage(reply);
        sessionLog.Add(("assistant", reply));

        Console.WriteLine($"\nAgent: {reply}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[Error: {ex.Message}]");
        Console.WriteLine("[If auto function calling fails, the model may not support tool use. Try llama3.1]\n");

        // Remove the failed user message to keep history consistent
        chatHistory.RemoveAt(chatHistory.Count - 1);
    }
}

// ─────────────────────────────────────────────────────────────
// PHASE 1A PART: Structured session report
// ─────────────────────────────────────────────────────────────
async Task GenerateSessionReport(Kernel k, List<(string Role, string Content)> log)
{
    if (!log.Any())
    {
        Console.WriteLine("No session activity to report yet.\n");
        return;
    }

    Console.WriteLine("\n━━━ Generating Session Report ━━━");

    var transcript = string.Join("\n", log.Select(e => $"{e.Role.ToUpper()}: {e.Content}"));

    var reportFn = k.CreateFunctionFromPrompt("""
        You are a healthcare operations analyst.
        Based on the session transcript below, generate a structured session report.
        
        Format the report with these sections:
        SUMMARY: (1-2 sentences of what was discussed)
        CLAIMS REVIEWED: (list any claim IDs mentioned)
        ACTIONS TAKEN: (list any appeals submitted or documents requested)
        OPEN ITEMS: (list unresolved questions or next steps)

        Transcript:
        {{$transcript}}
        """);

    var report = await k.InvokeAsync(reportFn, new KernelArguments { ["transcript"] = transcript });
    Console.WriteLine($"\n{report}\n");
}

// ── Cosine similarity (same as Phase 4) ──
static float CosineSimilarity(ReadOnlyMemory<float> a, ReadOnlyMemory<float> b)
{
    var va = a.Span; var vb = b.Span;
    float dot = 0f, magA = 0f, magB = 0f;
    for (int i = 0; i < va.Length; i++) { dot += va[i] * vb[i]; magA += va[i] * va[i]; magB += vb[i] * vb[i]; }
    var mag = MathF.Sqrt(magA) * MathF.Sqrt(magB);
    return mag == 0f ? 0f : dot / mag;
}
