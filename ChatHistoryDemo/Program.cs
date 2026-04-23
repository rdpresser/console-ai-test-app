// ============================================================
// PHASE 1B: Chat History – Multi-Turn Conversation with Memory
// ============================================================
// Concepts covered:
//   - IChatCompletionService (lower-level than InvokePromptAsync)
//   - ChatHistory to maintain conversation context across turns
//   - System prompt to define AI persona and scope
//   - Stateful sessions: why context matters for complex Q&A
// ============================================================

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var endpoint = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var model    = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: model, endpoint: endpoint)
    .Build();

// ── IChatCompletionService is the lower-level API.
//    InvokePromptAsync (Phase 1a) is a convenience wrapper around this.
//    Use this API when you need full control over chat history. ──
var chat = kernel.GetRequiredService<IChatCompletionService>();

// ─────────────────────────────────────────────────────────────
// System Prompt
// WHY: The system prompt sets the persona, rules, and domain scope
//      for the entire conversation. It is the first message in
//      ChatHistory and persists across all turns.
// ─────────────────────────────────────────────────────────────
var history = new ChatHistory("""
    You are a healthcare claim assistant for a U.S. insurance company.
    You help auditors and claim processors understand claim data.
    You are concise, factual, and professional.
    If you don't know something, say so clearly rather than guessing.
    You remember everything said in this conversation.
    """);

Console.WriteLine("=== PHASE 1B: Multi-Turn Chat with Memory ===");
Console.WriteLine("Type your question and press Enter. Type 'exit' to quit.");
Console.WriteLine("Tip: Ask follow-up questions — the AI remembers the whole conversation.");
Console.WriteLine("Example: Ask about claim CLM-2026-102, then ask 'What was that amount again?'\n");

// ─────────────────────────────────────────────────────────────
// Seed: inject a context message so the AI "knows" about claims
// WHY: In production, you'd inject retrieved records from SQL here.
//      This simulates feeding real data into the conversation.
// ─────────────────────────────────────────────────────────────
history.AddUserMessage("""
    Here is the claim context for this session. Use this data to answer my questions:

    Claim CLM-2026-102: Patient John Doe, MRI Lumbar Spine, $1,250, DENIED – Missing pre-auth. Provider: City General Hospital. Date: 2026-04-20.
    Claim CLM-2026-108: Patient Maria Silva, Knee Replacement, $14,800, APPROVED. Provider: Regional Orthopedic. Date: 2026-04-15.
    Claim CLM-2026-115: Patient Robert Chen, Emergency Room Visit, $3,200, PENDING – Awaiting medical records. Provider: Metro ER. Date: 2026-04-22.
    """);

history.AddAssistantMessage("Understood. I have loaded 3 claim records for this session. How can I help you?");

Console.WriteLine("Assistant: I have loaded 3 claim records. How can I help you?\n");

// ─────────────────────────────────────────────────────────────
// Conversation loop
// WHY: Each turn adds both the user message and the assistant reply
//      to ChatHistory. This is what gives the AI its "memory".
//      Without this, each question would be independent.
// ─────────────────────────────────────────────────────────────
while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    // Add user message to history before calling the model
    history.AddUserMessage(input);

    try
    {
        var response = await chat.GetChatMessageContentAsync(history);
        var reply    = response.Content ?? "(no response)";

        // Add assistant reply to history so the next turn remembers it
        history.AddAssistantMessage(reply);

        Console.WriteLine($"\nAssistant: {reply}\n");

        // ── Observability tip: in production log token count here ──
        // Console.WriteLine($"[Debug] History turns: {history.Count}");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"[Connection error] {ex.Message}");
        Console.WriteLine("Ensure Ollama is running at http://127.0.0.1:11434");
        break;
    }
}

Console.WriteLine($"\n=== Session ended. Total conversation turns: {history.Count} ===");
