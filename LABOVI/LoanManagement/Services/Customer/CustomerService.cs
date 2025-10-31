using LoanManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerModel = LoanManagement.Models.Customer;

namespace LoanManagement.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly DbClass _db;

        public CustomerService(DbClass db)
        {
            _db = db;
        }

        public int AddCustomer(string name, string email)
        {
            Random random = new Random();
            int customerId = random.Next(1000, 9999); 

            bool isUnique = false;
            while (!isUnique)
            {
                isUnique = true; 

                
                for (int i = 0; i < _db.Customers.Count; i++)
                {
                    if (_db.Customers[i].Id == customerId)
                    {
                        isUnique = false; 
                        customerId = random.Next(1000, 9999); 
                        break; 
                    }
                }
            }

            var customer = new CustomerModel(customerId, name, email);
            _db.AddCustomer(customer);
            return customerId;
        }

        public string GetCustomerDetails(int customerId)
        {
            var customer = _db.GetCustomer(customerId);
            if (customer != null)
            {
                int id = customer.Id;
                string name = customer.Name;
                string email = customer.Email;
                string details = "ID: " + id + ", Name: " + name + ", Email: " + email;
                return details;
            }
            else
            {
                throw new InvalidOperationException("Customer with id " + customerId + " was not found");
            }
        }
    }
}
