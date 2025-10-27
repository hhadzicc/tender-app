using LoanManagement.Data;
using LoanManagement.Models;
using LoanManagement.Services.Repayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using LoanModel = LoanManagement.Models.Loan;

namespace LoanManagement.Services.Loan
{
    public class LoanService : ILoanService
    {
        private readonly DbClass _db;

        public LoanService(DbClass db)
        {
            _db = db;
        }

        public List<LoanCalculationResult> CalculateMonthlyPayment(List<double> principalAmounts, List<double> interestRates, List<int> loanTerms, int numberOfLoans)
        {
            
            if (principalAmounts.Count != numberOfLoans || interestRates.Count != numberOfLoans || loanTerms.Count != numberOfLoans)
            {
                throw new ArgumentException("The number of loans and list sizes must match.");
               
            }

            List<LoanCalculationResult> loanResults = new List<LoanCalculationResult>();
         
            for (int i = 0; i < numberOfLoans; i++)
            {
                double principal = principalAmounts[i]; 
                double interestRate = interestRates[i];  
                int termInYears = loanTerms[i];  

                if (principal <= 0)
                {
                    int loanNumber = i + 1;
                    throw new InvalidOperationException("Invalid principal for loan " + loanNumber + ". It must be greater than zero.");
                    
                }

                else if (interestRate <= 0)
                {
                    int loanNumber = i + 1;
                    throw new InvalidOperationException($"Invalid interest rate for loan " + loanNumber + ". It must be greater than zero.");
                    
                }

                else if (termInYears <= 0)
                {
                    int loanNumber = i + 1;
                    throw new InvalidOperationException("Invalid loan term for loan " + loanNumber + ". It must be greater than zero.");
                    
                }

                double monthlyInterestRate = interestRate / 12 / 100;  
                int numberOfMonths = termInYears * 12;

                int numberOfMonthsMonthly = numberOfMonths * 12;

                double baseValue = 1 + monthlyInterestRate;

                double compoundedInterest = Math.Pow(baseValue, numberOfMonths);

                double numerator = principal * monthlyInterestRate * compoundedInterest;

                double denominator = compoundedInterest - 1;

                double monthlyPayment = numerator / denominator;

                LoanCalculationResult loanResult = new LoanCalculationResult
                {
                    Principal = principal,
                    InterestRate = interestRate,
                    TermInYears = termInYears,
                    MonthlyPayment = monthlyPayment,
                    NumberOfMonths = numberOfMonths,
                    MonthlyInterestRate = monthlyInterestRate
                };

                loanResults.Add(loanResult);
            }

            return loanResults;

        }

        public double CalculateTotalInterest(double principal, double monthlyPayment, int termInYears)
        {
            return (monthlyPayment * termInYears * 12) - principal;
        }

        public int AddLoan(int customerId, List<LoanCalculationResult> loanResults, DateTime startDate)
        {
            
            if (loanResults != null)
            {
                for (int i = 0; i < loanResults.Count; i++)
                {

                    Random random = new Random();
                    int loanId = random.Next(1000, 9999); 

                    bool isUnique = false;
                    while (!isUnique)
                    {
                        isUnique = true; 

                    
                        for (int j = 0; j < _db.Loans.Count; j++)
                        {
                            if (_db.Loans[j].Id == loanId)
                            {
                                isUnique = false; 
                                loanId = random.Next(1000, 9999); 
                                break; 
                            }
                        }
                    }


                    var loan = new LoanModel(loanId, customerId, loanResults[i].Principal, loanResults[i].InterestRate, loanResults[i].TermInYears, loanResults[i].MonthlyPayment, startDate);

                    _db.AddLoan(loan);

                    IRepaymentService repaymentService = new RepaymentService(_db);
                    repaymentService.ProcessPayment(customerId, loanResults[i].MonthlyPayment, startDate);
                    return loanId;
                }
            }
            else
            {
                throw new ArgumentException("Loan calculation failed due to invalid inputs.");
                


            }
            return 0;
        }

       

    }


}
