using AtmCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtmCore.Contracts
{
    public interface IAtmApi
    {
        CustomerPinValidationResult ValidatePin(string accountCode, string pin);
        CustomerBalanceResult GetBalance(string accountCode);
        CustomerTransactionResult Withdraw(string accountCode, decimal amount);
    }
}
