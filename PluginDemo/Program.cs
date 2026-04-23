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
using System.Text.RegularExpressions;

var endpoint = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var model    = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";
var showFallbackNotice = string.Equals(
    Environment.GetEnvironmentVariable("PLUGINDEMO_SHOW_FALLBACK_NOTICE"),
    "true",
    StringComparison.OrdinalIgnoreCase);

// ─────────────────────────────────────────────────────────────
// Register the plugin with the kernel.
// The kernel exposes it to the LLM as a set of callable tools.
// ─────────────────────────────────────────────────────────────
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: model, endpoint: endpoint)
    .Build();

kernel.Plugins.AddFromType<ClaimPlugin>("Claims");
var claimsPlugin = kernel.Plugins["Claims"];

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

var toolCallingEnabled = true;
var fallbackNoticePrinted = false;

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
        string result;
        if (toolCallingEnabled)
        {
            // ── The kernel will automatically call the right plugin functions ──
            var autoResult = await kernel.InvokePromptAsync(question, new(settings));
            result = autoResult.ToString();
        }
        else
        {
            result = await InvokeManualFallbackAsync(kernel, claimsPlugin, question);
        }

        Console.WriteLine($"Answer: {result}");
    }
    catch (Exception ex)
    {
        if (toolCallingEnabled && IsToolNotSupportedError(ex))
        {
            toolCallingEnabled = false;

            if (showFallbackNotice && !fallbackNoticePrinted)
            {
                Console.WriteLine("[Notice] Model does not support tools. Using manual fallback mode.");
                Console.WriteLine("[Tip] For native tool calling, try: ollama pull llama3.1 or mistral");
                fallbackNoticePrinted = true;
            }

            var fallback = await InvokeManualFallbackAsync(kernel, claimsPlugin, question);
            Console.WriteLine($"Answer: {fallback}");
        }
        else
        {
            Console.WriteLine($"Answer: Could not process this question. Details: {ex.Message}");
        }
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

// Direct invocation – bypass LLM, call function directly
var directResult = await kernel.InvokeAsync(claimsPlugin["GetClaimById"], new KernelArguments
{
    ["claimId"] = "CLM-2026-115"
});
Console.WriteLine($"Direct GetClaimById: {directResult}");

var summaryResult = await kernel.InvokeAsync(claimsPlugin["GetClaimsSummary"]);
Console.WriteLine($"\nDirect GetClaimsSummary:\n{summaryResult}");

Console.WriteLine("\n=== Done ===");

static bool IsToolNotSupportedError(Exception ex)
{
    var message = ex.ToString().ToLowerInvariant();
    return message.Contains("does not support tools") ||
           (message.Contains("support") && message.Contains("tools"));
}

static async Task<string> InvokeManualFallbackAsync(Kernel kernel, KernelPlugin claimsPlugin, string question)
{
    // Try to map common intents to explicit plugin calls when tools are unavailable.
    var normalized = question.ToLowerInvariant();

    // Case 1: specific claim detail/status
    var claimId = ExtractClaimId(question);
    if (claimId is not null && (normalized.Contains("status") || normalized.Contains("details") || normalized.Contains("claim")))
    {
        var result = await kernel.InvokeAsync(claimsPlugin["GetClaimById"], new KernelArguments
        {
            ["claimId"] = claimId
        });
        return result.ToString();
    }

    // Case 2: claims by patient
    if (normalized.Contains("patient") || normalized.Contains("named"))
    {
        var patient = ExtractPatientName(question) ?? "Maria Silva";
        var result = await kernel.InvokeAsync(claimsPlugin["GetClaimsByPatient"], new KernelArguments
        {
            ["patientName"] = patient
        });
        return result.ToString();
    }

    // Case 3: denied claims list
    if (normalized.Contains("denied"))
    {
        var result = await kernel.InvokeAsync(claimsPlugin["GetClaimsByStatus"], new KernelArguments
        {
            ["status"] = "Denied"
        });
        return result.ToString();
    }

    // Case 4: grouped summary
    if (normalized.Contains("summary") || normalized.Contains("grouped by status"))
    {
        var result = await kernel.InvokeAsync(claimsPlugin["GetClaimsSummary"]);
        return result.ToString();
    }

    // Default: summary gives a useful deterministic answer.
    var fallbackSummary = await kernel.InvokeAsync(claimsPlugin["GetClaimsSummary"]);
    return fallbackSummary.ToString();
}

static string? ExtractClaimId(string input)
{
    var match = Regex.Match(input, @"CLM-\d{4}-\d{3}", RegexOptions.IgnoreCase);
    return match.Success ? match.Value.ToUpperInvariant() : null;
}

static string? ExtractPatientName(string question)
{
    var marker = "named ";
    var idx = question.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
    if (idx < 0) return null;

    var name = question[(idx + marker.Length)..].Trim().TrimEnd('.', '?', '!');
    return string.IsNullOrWhiteSpace(name) ? null : name;
}
