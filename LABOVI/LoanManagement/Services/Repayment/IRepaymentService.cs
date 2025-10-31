using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Services.Repayment
{
    public interface IRepaymentService
    {
        double CalculateRemainingBalance(int loanId);
        void ProcessPayment(int customerId, double amount, DateTime paymentDate);
    }
}
