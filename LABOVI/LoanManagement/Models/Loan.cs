using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public double Principal { get; set; }
        public double InterestRate { get; set; }
        public int TermInYears { get; set; }
        public double MonthlyPayment { get; set; }
        public DateTime StartDate {  get; set; }

        public Loan(int id, int customerId, double principal, double interestRate, int termInYears, double monthlyPayment, DateTime startDate)
        {
            Id = id;
            CustomerId = customerId;
            Principal = principal;
            InterestRate = interestRate;
            TermInYears = termInYears;
            MonthlyPayment = monthlyPayment;
            StartDate = startDate;
        }
    }

}
