using System;
using System.Collections.Generic;
using System.Text;

namespace AtmCore
{

    public class AtmTransactionResult : TransactionResult
    {
        public AtmTransactionResult(decimal balance, TransactionOutcome result, string failureMessage) : base(result, failureMessage)
        {
            Balance = balance;
        }
        public decimal Balance { get;  set; }
    }


    public class Atm
    {
        public decimal Balance { get; private set; }
        public CustomerAccount CurrentCustomer { get; private set; }
        public AtmTransactionResult Withdraw(decimal amount)
        {
            if (CurrentCustomer == null)
            {
                return new AtmTransactionResult(Balance, TransactionOutcome.Failure, "ACCOUNT_ERR");
            }

            if (amount > Balance)
            {
                return new AtmTransactionResult(Balance, TransactionOutcome.Failure, "ATM_ERR");
            }

            

            Balance -= amount;
            return new AtmTransactionResult(Balance, TransactionOutcome.Success, "");
        }

        public void SetCurrentCustomer(CustomerAccount customer)
        {
            CurrentCustomer = customer;
        }

        public static Atm Load(decimal balance)
        {
            return new Atm { Balance = balance };
        }


    }
}
