namespace DingTopUp.Integration.Models;

public sealed class SendTransferRequest
{
    public required string SkuCode { get; set; }
    public required decimal SendValue { get; set; }

    public string? SendCurrencyIso { get; set; }

    public required string AccountNumber { get; set; }
    public required string DistributorRef { get; set; }
    public required bool ValidateOnly { get; set; }

    public List<SettingItem>? Settings { get; set; }
    public string? BillRef { get; set; }
}

public sealed class SettingItem
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}

public sealed class SendTransferResponse
{
    public TransferRecord? TransferRecord { get; set; }
    public int ResultCode { get; set; }
    public List<TransferError>? ErrorCodes { get; set; }
}

public sealed class TransferRecord
{
    public string? TransferRef { get; set; }
    public string? DistributorRef { get; set; }

    public string? SkuCode { get; set; }
    public string? AccountNumber { get; set; }

    public string? ProcessingState { get; set; } // Submitted/Processing/Complete/Failed

    public decimal? ReceiveValue { get; set; }
    public string? ReceiveCurrencyIso { get; set; }

    public decimal? SendValue { get; set; }
    public string? SendCurrencyIso { get; set; }

    public decimal? CommissionApplied { get; set; }

    public string? ReceiptText { get; set; }
    public Dictionary<string, string>? ReceiptParams { get; set; }
}

public sealed class TransferError
{
    public string Code { get; set; } = "";
    public string? Context { get; set; }
}
