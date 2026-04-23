using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PluginDemo.Plugins;

// ─────────────────────────────────────────────────────────────
// ClaimPlugin – simulates a healthcare backend data service
//
// WHY [KernelFunction]: This attribute tells Semantic Kernel
//   that this method is a "tool" the LLM can invoke.
//   The LLM reads the Description attribute to decide when to call it.
//
// WHY [Description]: The LLM uses these descriptions to understand
//   WHAT the function does and WHAT each parameter means.
//   Good descriptions = accurate function selection by the model.
//
// In production: these methods would call SQL Server, a REST API,
//   or a message queue — the LLM just decides WHEN to call them.
// ─────────────────────────────────────────────────────────────
public class ClaimPlugin
{
    // Simulated in-memory database (replace with SQL Server in production)
    private static readonly Dictionary<string, ClaimRecord> Claims = new()
    {
        ["CLM-2026-102"] = new("CLM-2026-102", "John Doe",   "MRI – Lumbar Spine",   1250.00m, "Denied",   "Missing pre-authorization", "City General Hospital",  "2026-04-20"),
        ["CLM-2026-108"] = new("CLM-2026-108", "Maria Silva","Knee Replacement",     14800.00m, "Approved", "N/A",                       "Regional Orthopedic",    "2026-04-15"),
        ["CLM-2026-115"] = new("CLM-2026-115", "Robert Chen","Emergency Room Visit",  3200.00m, "Pending",  "Awaiting medical records",  "Metro ER",               "2026-04-22"),
        ["CLM-2026-121"] = new("CLM-2026-121", "Alice Wang", "Lab Work – CBC Panel",   480.00m, "Approved", "N/A",                       "Lakeside Diagnostics",   "2026-04-18"),
    };

    [KernelFunction]
    [Description("Retrieves the full details of a healthcare insurance claim by its claim ID. Returns status, amount, denial reason, provider, and patient name.")]
    public string GetClaimById(
        [Description("The claim ID to look up, e.g. CLM-2026-102")] string claimId)
    {
        if (Claims.TryGetValue(claimId.Trim().ToUpperInvariant(), out var claim))
            return $"Claim {claim.Id}: Patient={claim.Patient}, Procedure={claim.Procedure}, Amount=${claim.Amount:F2}, Status={claim.Status}, Reason={claim.DenialReason}, Provider={claim.Provider}, Date={claim.Date}";

        return $"Claim '{claimId}' not found in the system.";
    }

    [KernelFunction]
    [Description("Searches for all claims belonging to a patient by their full name. Returns a list of claim IDs and their statuses.")]
    public string GetClaimsByPatient(
        [Description("Full name of the patient, e.g. John Doe")] string patientName)
    {
        var matches = Claims.Values
            .Where(c => c.Patient.Contains(patientName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matches.Any())
            return $"No claims found for patient '{patientName}'.";

        return string.Join("\n", matches.Select(c =>
            $"  {c.Id}: {c.Status} | ${c.Amount:F2} | {c.Procedure}"));
    }

    [KernelFunction]
    [Description("Returns all claims with a specific status: Approved, Denied, or Pending.")]
    public string GetClaimsByStatus(
        [Description("Claim status to filter by: Approved, Denied, or Pending")] string status)
    {
        var matches = Claims.Values
            .Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matches.Any())
            return $"No claims with status '{status}'.";

        return string.Join("\n", matches.Select(c =>
            $"  {c.Id} | {c.Patient} | ${c.Amount:F2} | {c.Provider}"));
    }

    [KernelFunction]
    [Description("Returns a summary count of claims grouped by status (Approved, Denied, Pending) and total dollar amounts.")]
    public string GetClaimsSummary()
    {
        var grouped = Claims.Values.GroupBy(c => c.Status);
        var lines   = grouped.Select(g =>
            $"  {g.Key}: {g.Count()} claims | Total: ${g.Sum(c => c.Amount):F2}");
        return "Claims Summary:\n" + string.Join("\n", lines);
    }
}

public record ClaimRecord(
    string Id, string Patient, string Procedure,
    decimal Amount, string Status, string DenialReason,
    string Provider, string Date);
