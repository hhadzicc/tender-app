using LoanManagement.Data;
using LoanManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Services.Repayment
{
    public class RepaymentService : IRepaymentService
    {

        private readonly DbClass _db;

        public RepaymentService(DbClass db)
        {
            _db = db;
        }
        public double CalculateRemainingBalance(int loanId)
        {
            var loan = _db.Loans.FirstOrDefault(l => l.Id == loanId);

            if (loan == null)
            {

                throw new InvalidOperationException("Loan with id " + loanId + " was not found");
            }

            double remainingPrincipal = loan.Principal;

            double monthlyInterestRate = loan.InterestRate / 12 / 100;

            double paymentsMade = 0;
            DateTime lastPaymentDate = loan.StartDate; 

            for (int i = 0; i < _db.Payments.Count; i++)
            {
                if (_db.Payments[i].LoanId == loanId)
                {
                    var payment = _db.Payments[i];
                    int monthsBetweenPayments = ((payment.PaymentDate.Year - lastPaymentDate.Year) * 12) + payment.PaymentDate.Month - lastPaymentDate.Month;

                    for (int m = 0; m < monthsBetweenPayments; m++)
                    {
                        remainingPrincipal += remainingPrincipal * monthlyInterestRate;
                    }

                    paymentsMade += payment.Amount;
                    remainingPrincipal -= payment.Amount;
                    lastPaymentDate = payment.PaymentDate; 
                }
            }

            DateTime currentDate = DateTime.Now;

            // add interest 
            var year = ((currentDate.Year - lastPaymentDate.Year) * 12);
            var months = currentDate.Month - lastPaymentDate.Month;
            int monthsSinceLastPayment = ((currentDate.Year - lastPaymentDate.Year) * 12) + currentDate.Month - lastPaymentDate.Month;

            for (int m = 0; m < monthsSinceLastPayment; m++)
            {
                remainingPrincipal += remainingPrincipal * monthlyInterestRate;
            }

            Console.WriteLine("Total payments made: " + paymentsMade);
            Console.WriteLine("Remaining balance as of " + currentDate.ToShortDateString() + ": " + remainingPrincipal);

            return remainingPrincipal;
        }

        public void ProcessPayment(int customerId, double amount, DateTime paymentDate)
        {
            var loan = _db.Loans.FirstOrDefault(l => l.CustomerId == customerId);
            if (loan != null)
            {
               

                Random random = new Random();
                int paymentId = random.Next(1000, 9999); 

                bool isUnique = false;
                while (!isUnique)
                {
                    isUnique = true; 

                   
                    for (int j = 0; j < _db.Payments.Count; j++)
                    {
                        if (_db.Payments[j].Id == paymentId)
                        {
                            isUnique = false; 
                            paymentId = random.Next(1000, 9999); 
                            break; 
                        }
                    }
                }


                var payment = new Payment(paymentId, loan.Id, amount, paymentDate);
                _db.AddPayment(payment);
                Console.WriteLine("Payment of " + amount + "received for customer " + customerId);
            }
            else
            {
                throw new InvalidOperationException("Loan with was not found for customer " + customerId);
            }
        }

     
    }
}
