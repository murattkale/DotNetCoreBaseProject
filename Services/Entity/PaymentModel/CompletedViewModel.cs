using System;


    public class CompletedViewModel
    {
        public string OrderNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string CardHolderName { get; set; }
        public int Installment { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
