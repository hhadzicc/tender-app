using LoanManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Data
{
    public class DbClass
    {
        public List<Customer> Customers { get; set; }
        public List<Loan> Loans { get; set; }
        public List<Payment> Payments { get; set; }

        public DbClass()
        {
            Customers = new List<Customer>();
            Loans = new List<Loan>();
            Payments = new List<Payment>();
        }

        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer);
        }

        public void AddLoan(Loan loan)
        {
            Loans.Add(loan);
        }

        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }

        public Customer GetCustomer(int customerId)
        {
            return Customers.FirstOrDefault(c => c.Id == customerId);
        }

        public Loan GetLoan(int loanId)
        {
            return Loans.FirstOrDefault(l => l.Id == loanId);
        }

        public List<Payment> GetPaymentsForLoan(int loanId)
        {
            return Payments.Where(p => p.Id == loanId).ToList();
        }
    }

}
