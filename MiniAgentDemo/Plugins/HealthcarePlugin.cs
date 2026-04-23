using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace MiniAgentDemo.Plugins;

// ─────────────────────────────────────────────────────────────
// HealthcarePlugin – the agent's tools for accessing live data
//
// This plugin combines what Phase 3 taught about KernelFunction
// with what the capstone agent needs: live data access alongside
// the RAG knowledge base built from policy documents.
// ─────────────────────────────────────────────────────────────
public class HealthcarePlugin
{
    private static readonly Dictionary<string, ClaimRecord> Claims = new()
    {
        ["CLM-2026-102"] = new("CLM-2026-102", "John Doe",   "MRI – Lumbar Spine",    1250.00m, "Denied",   "Missing pre-authorization"),
        ["CLM-2026-108"] = new("CLM-2026-108", "Maria Silva","Knee Replacement",      14800.00m, "Approved", "N/A"),
        ["CLM-2026-115"] = new("CLM-2026-115", "Robert Chen","Emergency Room Visit",   3200.00m, "Pending",  "Awaiting medical records"),
    };

    [KernelFunction]
    [Description("Gets the current status, amount, and denial reason for a specific insurance claim by claim ID.")]
    public string GetClaimStatus(
        [Description("The claim ID, e.g. CLM-2026-102")] string claimId)
    {
        if (Claims.TryGetValue(claimId.Trim().ToUpperInvariant(), out var c))
            return $"{c.Id}: {c.Procedure} | ${c.Amount:F2} | Status: {c.Status} | Reason: {c.Reason}";
        return $"Claim '{claimId}' not found.";
    }

    [KernelFunction]
    [Description("Submits a formal appeal for a denied claim. Returns a confirmation number.")]
    public string SubmitAppeal(
        [Description("The claim ID to appeal")] string claimId,
        [Description("Brief reason for the appeal")] string reason)
    {
        var confirmationNumber = $"APL-{DateTime.Now:yyyyMMdd}-{claimId.Replace("CLM-", "")}";
        return $"Appeal submitted for {claimId}. Confirmation: {confirmationNumber}. Reason recorded: '{reason}'. Review within 30 business days.";
    }

    [KernelFunction]
    [Description("Gets the list of required documents needed to resolve a denied claim.")]
    public string GetRequiredDocuments(
        [Description("The denial reason code or description")] string denialReason)
    {
        return denialReason.ToLowerInvariant() switch
        {
            var r when r.Contains("pre-authorization") =>
                "Required: (1) Completed form PA-2026, (2) Treating physician's clinical notes, (3) Referral letter from PCP.",
            var r when r.Contains("medical records") =>
                "Required: (1) Complete medical records from the treating facility, (2) Discharge summary, (3) Lab results.",
            var r when r.Contains("duplicate") =>
                "Required: (1) Proof this is a distinct service date, (2) Updated claim with corrected service codes.",
            _ =>
                "Required: (1) Completed appeal form, (2) Supporting clinical documentation, (3) Provider statement."
        };
    }
}

public record ClaimRecord(string Id, string Patient, string Procedure,
    decimal Amount, string Status, string Reason);
