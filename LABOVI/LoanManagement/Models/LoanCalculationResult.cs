using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Models
{
    public class LoanCalculationResult
    {
        public double Principal { get; set; }
        public double InterestRate { get; set; }
        public int TermInYears { get; set; }
        public double MonthlyPayment { get; set; }
        public int NumberOfMonths { get; set; }
        public double MonthlyInterestRate { get; set; }

        public override string ToString()
        {
            return $"Principal: {Principal:C}, Interest Rate: {InterestRate}%, Term: {TermInYears} years, " +
                   $"Monthly Payment: {MonthlyPayment:C}, Monthly Interest Rate: {MonthlyInterestRate}, " +
                   $"Number of Months: {NumberOfMonths}";
        }
    }
}
