// ============================================================
// PHASE 3: SK Plugins – Native Functions (Tool Use / Function Calling)
// ============================================================
// Concepts covered:
//   - [KernelFunction] + [Description] to expose C# methods as AI tools
//   - FunctionChoiceBehavior.Auto() to let the LLM call functions
//   - The LLM autonomously decides which function to invoke
//   - In production: functions call SQL Server, REST APIs, message queues
//
// THIS IS THE MOST IMPORTANT CONCEPT FOR THE JOB:
//   The job asks to "implement AI solutions within a legacy healthcare product".
//   Plugins are how you bridge the AI to your existing backend systems.
// ============================================================

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using PluginDemo.Plugins;

var endpoint = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var model    = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";

// ─────────────────────────────────────────────────────────────
// Register the plugin with the kernel.
// The kernel exposes it to the LLM as a set of callable tools.
// ─────────────────────────────────────────────────────────────
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: model, endpoint: endpoint)
    .Build();

kernel.Plugins.AddFromType<ClaimPlugin>("Claims");

Console.WriteLine("=== PHASE 3: Native Functions / Plugins ===");
Console.WriteLine("The LLM has access to these tools:");
foreach (var plugin in kernel.Plugins)
    foreach (var fn in plugin)
        Console.WriteLine($"  {plugin.Name}.{fn.Name}: {fn.Description}");
Console.WriteLine();

// ─────────────────────────────────────────────────────────────
// FunctionChoiceBehavior.Auto() – the key setting
// WHY: With Auto, when the LLM receives a question it cannot answer
//      from its training data, it will emit a "tool call" request.
//      The kernel intercepts it, executes the C# method, injects
//      the result back into the conversation, and re-invokes the LLM
//      to generate the final answer. All transparent to you.
// ─────────────────────────────────────────────────────────────
#pragma warning disable SKEXP0070
var settings = new OllamaPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
#pragma warning restore SKEXP0070

// Test questions – the LLM will decide which function to call for each
var questions = new[]
{
    "What is the status of claim CLM-2026-102? Give me all the details.",
    "Show me all claims for the patient named Maria Silva.",
    "How many denied claims do we have? List them.",
    "Give me a summary of all claims grouped by status.",
};

foreach (var question in questions)
{
    Console.WriteLine($"━━━ Question: {question}");

    try
    {
        // ── The kernel will automatically call the right plugin functions ──
        var result = await kernel.InvokePromptAsync(question, new(settings));
        Console.WriteLine($"Answer: {result}");
    }
    catch (Exception ex)
    {
        // If the model doesn't support tool calling natively, fall back to manual
        Console.WriteLine($"[Auto function calling not supported by this model build]");
        Console.WriteLine($"[Error: {ex.Message}]");
        Console.WriteLine("[Tip: Try a function-calling capable model: ollama pull llama3.1 or mistral]");
        Console.WriteLine("[Fallback: see Demo 2 below for manual invocation]");
        break;
    }

    Console.WriteLine();
}

// ─────────────────────────────────────────────────────────────
// DEMO 2 – Manual plugin invocation (always works, any model)
// WHY: If your local model doesn't support tool calling, or you want
//      deterministic control, you can call plugins directly.
//      This shows the same functions work without LLM orchestration.
// ─────────────────────────────────────────────────────────────
Console.WriteLine("━━━ Demo 2: Manual plugin invocation (no LLM needed) ━━━");

var claimsPlugin = kernel.Plugins["Claims"];

// Direct invocation – bypass LLM, call function directly
var directResult = await kernel.InvokeAsync(claimsPlugin["GetClaimById"], new KernelArguments
{
    ["claimId"] = "CLM-2026-115"
});
Console.WriteLine($"Direct GetClaimById: {directResult}");

var summaryResult = await kernel.InvokeAsync(claimsPlugin["GetClaimsSummary"]);
Console.WriteLine($"\nDirect GetClaimsSummary:\n{summaryResult}");

Console.WriteLine("\n=== Done ===");
