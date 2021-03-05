using System;

namespace SelfServiceLibrary.Service.DTO.Issue
{
    public class NfcIssueCreateDTO
    {
#pragma warning disable CS8618 // Enforced by validation
        public string BarCode { get; set; }
        public DateTime ExpiryDate { get; set; }
#pragma warning restore CS8618 // Enforced by validation
    }
}
