namespace EDIProcessingApp.Core.Enums;

public enum EdiFileStatus
{
    Pending,
    Processing,
    Processed,
    Error,
    Archived
}

public enum TransactionStatus
{
    Pending,
    Validated,
    Error,
    Sent,
    Acknowledged
}

public enum FileSource
{
    SFTP,
    API,
    Manual
}

public enum EdiFormat
{
    X12,
    EDIFACT,
    Custom
}

public enum TransactionType
{
    // X12 Transaction Types
    PurchaseOrder_850,
    Invoice_810,
    AdvanceShipNotice_856,
    PurchaseOrderAcknowledgment_855,
    PaymentOrderRemittance_820,
    ShippingSchedule_862,
    FunctionalAcknowledgment_997,
    
    // EDIFACT Message Types
    ORDERS,      // Purchase Order
    INVOIC,      // Invoice
    DESADV,      // Advance Ship Notice
    ORDRSP,      // Purchase Order Acknowledgment
    PAYMUL,      // Payment/Remittance
    REMADV,      // Payment/Remittance
    DELFOR,      // Shipping Schedule
    CONTRL       // Functional Acknowledgment
}

public enum ConfigurationType
{
    General,
    EDI,
    SFTP,
    Validation,
    API,
    Security,
    Notification
}
