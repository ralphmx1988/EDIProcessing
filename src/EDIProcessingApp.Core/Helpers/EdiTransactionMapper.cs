using System;
using System.Collections.Generic;
using System.Linq;

namespace EDIProcessingApp.Core.Helpers;

public static class EdiTransactionMapper
{
    public static readonly Dictionary<string, EdiTransactionInfo> TransactionMappings = new()
    {
        // X12 Transaction Sets
        ["850"] = new EdiTransactionInfo("850", "ORDERS", "Purchase Order", "Order request"),
        ["810"] = new EdiTransactionInfo("810", "INVOIC", "Invoice", "Billing information"),
        ["856"] = new EdiTransactionInfo("856", "DESADV", "Advance Ship Notice", "Shipping notification"),
        ["855"] = new EdiTransactionInfo("855", "ORDRSP", "Purchase Order Acknowledgement", "PO confirmation"),
        ["820"] = new EdiTransactionInfo("820", "PAYMUL,REMADV", "Payment Order/Remittance", "Payment/remittance information"),
        ["862"] = new EdiTransactionInfo("862", "DELFOR", "Shipping Schedule", "Delivery schedule"),
        ["997"] = new EdiTransactionInfo("997", "CONTRL", "Functional Acknowledgement", "Receipt confirmation of EDI message"),
        
        // EDIFACT Message Types (reverse mapping)
        ["ORDERS"] = new EdiTransactionInfo("850", "ORDERS", "Purchase Order", "Order request"),
        ["INVOIC"] = new EdiTransactionInfo("810", "INVOIC", "Invoice", "Billing information"),
        ["DESADV"] = new EdiTransactionInfo("856", "DESADV", "Advance Ship Notice", "Shipping notification"),
        ["ORDRSP"] = new EdiTransactionInfo("855", "ORDRSP", "Purchase Order Acknowledgement", "PO confirmation"),
        ["PAYMUL"] = new EdiTransactionInfo("820", "PAYMUL", "Payment Order/Remittance", "Payment/remittance information"),
        ["REMADV"] = new EdiTransactionInfo("820", "REMADV", "Payment Order/Remittance", "Payment/remittance information"),
        ["DELFOR"] = new EdiTransactionInfo("862", "DELFOR", "Shipping Schedule", "Delivery schedule"),
        ["CONTRL"] = new EdiTransactionInfo("997", "CONTRL", "Functional Acknowledgement", "Receipt confirmation of EDI message")
    };

    public static EdiTransactionInfo? GetTransactionInfo(string code)
    {
        return TransactionMappings.TryGetValue(code.ToUpper(), out var info) ? info : null;
    }

    public static string GetDocumentName(string code)
    {
        var info = GetTransactionInfo(code);
        return info?.DocumentName ?? "Unknown";
    }

    public static string GetDescription(string code)
    {
        var info = GetTransactionInfo(code);
        return info?.Description ?? "Unknown transaction type";
    }

    public static string GetX12Code(string edifactName)
    {
        var info = TransactionMappings.Values.FirstOrDefault(t => 
            t.EdifactName.Split(',').Contains(edifactName.ToUpper()));
        return info?.X12Code ?? edifactName;
    }

    public static string GetEdifactName(string x12Code)
    {
        var info = GetTransactionInfo(x12Code);
        return info?.EdifactName.Split(',')[0] ?? x12Code;
    }

    public static bool IsValidTransactionType(string code)
    {
        return TransactionMappings.ContainsKey(code.ToUpper());
    }

    public static List<EdiTransactionInfo> GetAllTransactionTypes()
    {
        return TransactionMappings.Values
            .GroupBy(t => t.X12Code)
            .Select(g => g.First())
            .OrderBy(t => t.X12Code)
            .ToList();
    }

    public static List<string> GetSupportedX12Codes()
    {
        return new List<string> { "850", "810", "856", "855", "820", "862", "997" };
    }

    public static List<string> GetSupportedEdifactCodes()
    {
        return new List<string> { "ORDERS", "INVOIC", "DESADV", "ORDRSP", "PAYMUL", "REMADV", "DELFOR", "CONTRL" };
    }
}

public record EdiTransactionInfo(
    string X12Code,
    string EdifactName,
    string DocumentName,
    string Description
);
