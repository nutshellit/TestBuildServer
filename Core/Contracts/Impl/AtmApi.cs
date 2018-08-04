using AtmCore.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtmCore.Contracts.Impl
{
    public class AtmApi : IAtmApi
    {
        public AtmApi(IAtmRepository atmRepo,IAccountRepository accountRepo )
        {
            _accountRepo = accountRepo;
            _atm = atmRepo.Get();
        }

        IAccountRepository _accountRepo;
        Atm _atm;


        public CustomerBalanceResult GetBalance(string accountCode)
        {
            if (_atm.CurrentCustomer == null || _atm.CurrentCustomer.AccountNumber != accountCode )
                return new CustomerBalanceResult(0, TransactionOutcome.Failure, "ACCOUNT_ERR");
            return _atm.CurrentCustomer.GetBalance();
        }

        public CustomerPinValidationResult ValidatePin(string accountCode, string pin)
        {
            var customer = _accountRepo.Get(accountCode);
            if (customer == null)
                return new CustomerPinValidationResult(TransactionOutcome.Failure, "ACCOUNT_ERR");
            var r =  customer.ValidatePin(pin);
            if (r.Result == TransactionOutcome.Success)
                _atm.SetCurrentCustomer(customer);
            return r;
        }

        public CustomerTransactionResult Withdraw(string accountCode, decimal amount)
        {
            var r = _atm.Withdraw(amount);
            if(r.Result == TransactionOutcome.Failure)
                return new CustomerTransactionResult(0, 0, TransactionOutcome.Failure, r.FailureMessage);
            if (_atm.CurrentCustomer == null || _atm.CurrentCustomer.AccountNumber != accountCode)
                return new CustomerTransactionResult(0,0, TransactionOutcome.Failure, "ACCOUNT_ERR");
            return _atm.CurrentCustomer.Withdraw(amount);
        }
    }
}
