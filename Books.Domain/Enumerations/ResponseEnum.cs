using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Enumerations
{
    public enum ResponseEnum
    {
        [EnumDisplay(Name = "Approved Or Completed Successfully", Description = "Transacion or operation was successful")]
        ApprovedOrCompletedSuccesfully = 00,

        [EnumDisplay(Name = "Status Unknown", Description = "Status of transacion or operation is unknown")]
        StatusUnknown = 01,

        [EnumDisplay(Name = "Invalid Phone Number", Description = "Phone number is invalid")]
        InvalidPhoneNumner = 02,

        [EnumDisplay(Name = "Invalid Sender", Description = "Invalid sender bank code")]
        InvalidSender = 03,

        [EnumDisplay(Name = "Do Not Honor", Description = "Transaction or operation should not be honored")]
        DoNotHonor = 05,

        [EnumDisplay(Name = "Dormant Account", Description = "Customer account number is dormant")]
        DormantAccount = 06,

        [EnumDisplay(Name = "Invalid Account", Description = "Customer account number is invalid or cannot be used for the operation")]
        InvalidAccount = 07,

        [EnumDisplay(Name = "Account Name Mismatch", Description = "Name provided does not match the account name on the core banking or database")]
        AccountNameMismatch = 08,

        [EnumDisplay(Name = "Request Processing In Progress", Description = "The request is being processed")]
        RequestProcessingInProgress = 09,

        [EnumDisplay(Name = "Invalid Transaction", Description = "The request is being processed")]
        InvalidTransaction = 12,

        [EnumDisplay(Name = "Invalid Amount", Description = "Transaction amount is invalid")]
        InvalidAmount = 13,

        [EnumDisplay(Name = "Invalid Batch Number", Description = "Invalid batch number")]
        InvalidBatchNumber = 14,

        [EnumDisplay(Name = "Invalid Session Or RecordID", Description = "Invalid Session Or RecordID")]
        InvalidSessionOrRecordID = 15,

        [EnumDisplay(Name = "Unknown Bank Code", Description = "Bank code does not exist")]
        UnknownBankCode = 16,

        [EnumDisplay(Name = "Invalid Channel", Description = "Invalid Channel code")]
        InvalidChannel = 17,

        [EnumDisplay(Name = "Wrong Method Call", Description = "Wrong Method Call")]
        WrongMethodCall = 18,

        [EnumDisplay(Name = "No Action Taken", Description = "No Action Taken")]
        NoActionTaken = 21,

        [EnumDisplay(Name = "Unable To Locate Record", Description = "Unable To Locate Record")]
        UnableToLocateRecord = 25,

        [EnumDisplay(Name = "Duplicate Record", Description = "Duplicate Record")]
        DuplicateRecord = 26,

        [EnumDisplay(Name = "Format Error", Description = "Format Error")]
        FormatError = 30,

        [EnumDisplay(Name = "Suspected Fraud", Description = "Suspected Fraud")]
        SuspectedFraud = 34,

        [EnumDisplay(Name = "Contact Sending Bank", Description = "Contact Sending Bank")]
        ContactSendingBank = 35,

        [EnumDisplay(Name = "No Sufficent Funds", Description = "No Sufficent Funds")]
        NoSufficentFunds = 51,

        [EnumDisplay(Name = "Transaction Not Permitted To Sender", Description = "Transaction Not Permitted To Sender")]
        TransactionNotPermittedToSender = 57,

        [EnumDisplay(Name = "Transaction Not Permitted On Channel", Description = "Transaction Not Permitted To Sender")]
        TransactionNotPermittedOnChannel = 58,

        [EnumDisplay(Name = "Transfer Limit Exceeded", Description = "Transfer Limit Exceeded")]
        TransferLimitExceeded = 61,

        [EnumDisplay(Name = "Security Violation ", Description = "Security violation ")]
        SecurityViolation = 63,

        [EnumDisplay(Name = "Exceeds Withdrawal Frequency ", Description = "Exceeds withdrawal frequency ")]
        ExceedsWithdrawalFrequency = 65,

        [EnumDisplay(Name = "Response received too late", Description = "Response received too late")]
        ResponseReceivedTooLate = 68,

        [EnumDisplay(Name = "Unsuccessful Account Amount Block", Description = "Unsuccessful Account Amount block")]
        UnsuccessfulAccountAmountBlock = 69,

        [EnumDisplay(Name = "Unsuccessful Account Amount unblock", Description = "Unsuccessful Account Amount unblock")]
        UnsuccessfulAccountAmountUnblock = 70,

        [EnumDisplay(Name = "Empty Mandate Reference Number", Description = "Empty Mandate Reference Number")]
        EmptyMandateReferenceNumber = 71,

        [EnumDisplay(Name = "Beneficiary Bank not available", Description = "Beneficiary Bank not available")]
        BeneficiaryBanknotAvailable = 91,

        [EnumDisplay(Name = "Routing error", Description = "Routing error")]
        RoutingError = 92,

        [EnumDisplay(Name = "Duplicate transaction ", Description = "Duplicate transaction ")]
        DuplicateTransaction = 94,

        [EnumDisplay(Name = "System Malfunction ", Description = "System malfunction ")]
        SystemMalfunction = 96,

        [EnumDisplay(Name = "Timeout waiting for response from destination", Description = "Timeout waiting for response from destination")]
        TimeoutWaitingForResponseFromDestination = 97,

        [EnumDisplay(Name = "Time Out ", Description = "Time Out ")]
        TimeOut = 11,

        [EnumDisplay(Name = "Fcmb Transaction Status Unknown", Description = "Fcmb Transaction Status Unknown")]
        FcmbTransactionStatusUnknown = 22,

        [EnumDisplay(Name = "Fcmb Unable To Debit Customer", Description = "Fcmb Unable To Debit Customer")]
        FcmbUnableToDebitCustomer = 33,

        [EnumDisplay(Name = "Fcmb Invalid Nuban Account", Description = "Fcmb Invalid Nuban Account")]
        FcmbInvalidNubanAccount = 44,

        [EnumDisplay(Name = "Fcmb Invalid Account Name", Description = "Fcmb Invalid Account Name")]
        FcmbInvalidAccountName = 45

    }

    public class EnumDisplayAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

    }

}
