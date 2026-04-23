// ============================================================
// PHASE 1A: Text Generation – Prompt Templates & Structured Output
// ============================================================
// Concepts covered:
//   - KernelArguments and prompt templates with named variables
//   - Few-shot prompting to enforce consistent output format
//   - Forcing JSON output and parsing it with System.Text.Json
// ============================================================

using System.Text.Json;
using Microsoft.SemanticKernel;

var endpoint = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://127.0.0.1:11434");
var model    = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3";

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId: model, endpoint: endpoint)
    .Build();

// ── Raw data simulating a record pulled from a SQL Server healthcare system ──
var claim = new
{
    ClaimId   = "CLM-2026-102",
    Patient   = "John Doe",
    Date      = "2026-04-20",
    Amount    = 1250.00m,
    Status    = "Denied",
    Reason    = "Missing pre-authorization documentation",
    Provider  = "City General Hospital",
    Procedure = "MRI – Lumbar Spine"
};

Console.WriteLine("=== PHASE 1A: Text Generation ===");
Console.WriteLine($"\nRaw input: {JsonSerializer.Serialize(claim)}\n");

// ─────────────────────────────────────────────────────────────
// DEMO 1 – Prompt template with named variables (KernelArguments)
// WHY: Separating templates from data is the production pattern.
//      Never concatenate user data directly into prompts.
// ─────────────────────────────────────────────────────────────
Console.WriteLine("━━━ Demo 1: Prompt template with named variables ━━━");

var summaryTemplate = """
    You are a healthcare claim analyst.
    Write a concise 2-sentence professional summary for an auditor.

    Claim ID:  {{$claim_id}}
    Patient:   {{$patient}}
    Date:      {{$date}}
    Amount:    ${{$amount}}
    Status:    {{$status}}
    Reason:    {{$reason}}
    Provider:  {{$provider}}
    Procedure: {{$procedure}}
    """;

var summaryFn = kernel.CreateFunctionFromPrompt(summaryTemplate);
var summary   = await kernel.InvokeAsync(summaryFn, new KernelArguments
{
    ["claim_id"]  = claim.ClaimId,
    ["patient"]   = claim.Patient,
    ["date"]      = claim.Date,
    ["amount"]    = claim.Amount.ToString("F2"),
    ["status"]    = claim.Status,
    ["reason"]    = claim.Reason,
    ["provider"]  = claim.Provider,
    ["procedure"] = claim.Procedure
});

Console.WriteLine(summary);
Console.WriteLine();

// ─────────────────────────────────────────────────────────────
// DEMO 2 – Few-shot prompting to enforce a strict output format
// WHY: Without examples the LLM produces different formats each run.
//      Few-shot examples teach the exact format you need.
// ─────────────────────────────────────────────────────────────
Console.WriteLine("━━━ Demo 2: Few-shot prompting (consistent format) ━━━");

var fewShotTemplate = """
    Summarize healthcare claims. Always use EXACTLY this format:
    CLAIM [id] | [STATUS] | Action: [one action sentence]

    Examples:
    CLAIM CLM-001 | APPROVED | Action: Process payment of $500 to City Clinic within 5 days.
    CLAIM CLM-002 | DENIED   | Action: Send denial letter requesting missing surgical notes from Dr. Smith.
    CLAIM CLM-003 | PENDING  | Action: Awaiting lab report; follow up on 2026-04-30.

    Now summarize:
    Claim ID: {{$claim_id}}, Status: {{$status}}, Reason: {{$reason}}, Amount: ${{$amount}}
    """;

var fewShotFn  = kernel.CreateFunctionFromPrompt(fewShotTemplate);
var fewShotOut = await kernel.InvokeAsync(fewShotFn, new KernelArguments
{
    ["claim_id"] = claim.ClaimId,
    ["status"]   = claim.Status,
    ["reason"]   = claim.Reason,
    ["amount"]   = claim.Amount.ToString("F2")
});

Console.WriteLine(fewShotOut);
Console.WriteLine();

// ─────────────────────────────────────────────────────────────
// DEMO 3 – Structured JSON output + deserialization
// WHY: In production you need typed data, not free text.
//      Instructing the LLM to return JSON lets you deserialize
//      the result and feed it into the next pipeline stage.
// ─────────────────────────────────────────────────────────────
Console.WriteLine("━━━ Demo 3: Force JSON output and parse it ━━━");

var jsonTemplate = """
    Analyze this healthcare claim and respond ONLY with a valid JSON object.
    No markdown fences, no explanation – just raw JSON.

    Required fields:
    {
      "risk_level": "Low" | "Medium" | "High",
      "action_required": "string",
      "estimated_resolution_days": number,
      "priority": "Routine" | "Urgent" | "Critical"
    }

    Claim: {{$claim_id}} | Status: {{$status}} | Reason: {{$reason}} | Amount: ${{$amount}}
    """;

var jsonFn  = kernel.CreateFunctionFromPrompt(jsonTemplate);
var jsonOut = await kernel.InvokeAsync(jsonFn, new KernelArguments
{
    ["claim_id"] = claim.ClaimId,
    ["status"]   = claim.Status,
    ["reason"]   = claim.Reason,
    ["amount"]   = claim.Amount.ToString("F2")
});

var jsonRaw = jsonOut.ToString().Trim();
Console.WriteLine("LLM output:");
Console.WriteLine(jsonRaw);

try
{
    using var doc = JsonDocument.Parse(jsonRaw);
    Console.WriteLine("\nParsed fields:");
    foreach (var prop in doc.RootElement.EnumerateObject())
        Console.WriteLine($"  {prop.Name}: {prop.Value}");
}
catch
{
    // LLMs occasionally wrap output in markdown ``` blocks.
    // Production tip: strip ```json ... ``` before parsing.
    Console.WriteLine("\n[Tip: model returned non-pure JSON. Strip markdown fences and retry.]");
}

Console.WriteLine("\n=== Done ===");
