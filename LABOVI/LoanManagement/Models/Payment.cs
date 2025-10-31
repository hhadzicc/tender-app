using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public Payment(int id, int loanId, double amount, DateTime paymentDate)
        {
            Id = id;
            LoanId = loanId;
            Amount = amount;
            PaymentDate = paymentDate;
        }
    }

}
