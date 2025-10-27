// See https://aka.ms/new-console-template for more information
using LoanManagement.Data;
using LoanManagement.Services.Customer;
using LoanManagement.Services.Loan;
using LoanManagement.Services.Repayment;

try
{
    DbClass mockDb = new DbClass();


    ICustomerService customerService = new CustomerService(mockDb);
    ILoanService loanService = new LoanService(mockDb);
    IRepaymentService repaymentService = new RepaymentService(mockDb);

    int customerId = customerService.AddCustomer("John Doe", "john@example.com");
    Console.WriteLine(customerService.GetCustomerDetails(customerId)); 

    List<double> principalAmounts = new List<double> { 10000, 15000 }; 
    List<double> interestRates = new List<double> { 5, 7 }; 
    List<int> loanTerms = new List<int> { 5, 10 }; 
    int numberOfLoans = principalAmounts.Count; 


    var results = loanService.CalculateMonthlyPayment(principalAmounts, interestRates, loanTerms, numberOfLoans);
    double totalInterest = loanService.CalculateTotalInterest(principalAmounts[0], results[0].MonthlyPayment, loanTerms[0]);
    int loanId = loanService.AddLoan(customerId, results, new DateTime(2024, 5, 5));

    Console.WriteLine($"Monthly Payment for first loan: {results[0].ToString():C}");
    Console.WriteLine($"Total Interest for first loan: {totalInterest:C}");

    repaymentService.ProcessPayment(customerId, 500, new DateTime(2024, 8, 8));
    double remainingBalance = repaymentService.CalculateRemainingBalance(loanId); 
    Console.WriteLine($"Remaining Balance after payment: {remainingBalance:C}");
}
catch (Exception e){
    Console.WriteLine(e.Message);
}
      
