using System;
using System.Collections.Generic;
using JsonParserOverride.Enumerations;

namespace JsonParserOverride.Models
{
    public class ContractDto : DtoBase
    {
        public int Id { get; internal set; }
        public string CustomerFirstname { get; set; }
        public string CustomerLastname { get; set; }

        public string SalesmanId { get; set; }
        public string SalemanName { get; set; }

        public int AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZipCode { get; set; }
        public string AddressPhoneNumber { get; set; }

        public int MailingAddressId { get; set; }
        public string MailingAddressLine1 { get; set; }
        public string MailingAddressLine2 { get; set; }
        public string MailingAddressCity { get; set; }
        public string MailingAddressState { get; set; }
        public string MailingAddressZipCode { get; set; }
        public string MailingAddressPhoneNumber { get; set; }

        public string EmailAddress { get; set; }
        public Job? Job { get; set; }
        public DateTime TurnedInDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool Warranty { get; set; }
        public bool PropertyHasMultipleStories { get; set; }
        public bool PropertyHasGutterColor { get; set; }
        public bool? IsArchived { get; set; }
        public decimal ContractPrice { get; set; }
        public decimal InsuranceCash { get; set; }
        public decimal Upgrades { get; set; }
        public decimal WnpSubtractions { get; set; }
        public decimal AdvAgree { get; set; }
        public decimal SupplementRequested { get; set; }
        public decimal SupplementApproved { get; set; }
        public decimal MoniesToBeCollected { get; set; }
        public decimal MoniesReceived { get; set; }
        public decimal CommissionDue { get; set; }
        public bool CommissionPaid { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Materials { get; set; }
        public decimal Labor { get; set; }
        public decimal GuttersMisc { get; set; }
        public List<Trade> Trades { get; set; }
        public decimal WarrantyAmount { get; set; }
        public decimal Profit => ContractPrice - (Materials + Labor + WarrantyAmount + CommissionDue);
    }
}