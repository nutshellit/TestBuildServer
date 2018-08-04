using System;

namespace AtmCore
{
    

    public class CustomerTransactionResult : TransactionResult
    {
        public CustomerTransactionResult(decimal customerBalance, decimal amount, TransactionOutcome result, string failureMessage) : base(result, failureMessage)
        {
            CustomerBalance = customerBalance;
            Amount = amount;
        }
        public decimal CustomerBalance { get; set; }
        public decimal Amount { get; set; }
    }

    public class CustomerPinValidationResult : TransactionResult {
        public CustomerPinValidationResult(TransactionOutcome result, string failureMessage) : base(result, failureMessage)
        {
        }
    }
    public class CustomerBalanceResult : TransactionResult
    {
        public CustomerBalanceResult(decimal customerBalance, TransactionOutcome result, string failureMessage) : base(result, failureMessage)
        {
            CustomerBalance = customerBalance;
        }
        public decimal CustomerBalance { get; set; }
    }


    public class CustomerAccount
    {
        public string AccountNumber { get; private set; }
        string Pin { get; set; }
        public decimal AccountBalance { get; private set; }
        public decimal OverdraftFacility { get; private set; }

        public CustomerTransactionResult Deposit(decimal amount)
        {
            AccountBalance += amount;
            return new CustomerTransactionResult(AccountBalance, amount, TransactionOutcome.Success, "");
        }

        public CustomerTransactionResult Withdraw(decimal amount)
        {
            if (amount > (AccountBalance + OverdraftFacility))
            {
                return new CustomerTransactionResult(AccountBalance, 0, TransactionOutcome.Failure, "FUNDS_ERR");
            }

            AccountBalance -= amount;
            return new CustomerTransactionResult(AccountBalance, amount, TransactionOutcome.Success, "");
        }

        public CustomerPinValidationResult ValidatePin(string pin)
        {
            bool ok =  (Pin == pin);
            return new CustomerPinValidationResult(ok ? TransactionOutcome.Success : TransactionOutcome.Failure, ok ? "" : "ACCOUNT_ERR");
        }

        public CustomerBalanceResult GetBalance()
        {
            return new CustomerBalanceResult(AccountBalance, TransactionOutcome.Success, "");
        }


        public static CustomerAccount Load(string accNo, string pin, decimal accBal, decimal overdraftFacility)
        {
            return new CustomerAccount { AccountNumber = accNo,
                                         Pin = pin,
                                         AccountBalance = accBal,
                                         OverdraftFacility = overdraftFacility};
        }

    }
}
