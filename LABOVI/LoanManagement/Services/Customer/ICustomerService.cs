using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Services.Customer
{
    public interface ICustomerService
    {
        int AddCustomer(string name, string email);
        string GetCustomerDetails(int customerId);
    }
}
