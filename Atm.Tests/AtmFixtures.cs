using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using AtmCore;

namespace AtmTests
{
    public class AtmFixtures
    {
        CustomerAccount DefaultAccount()
        {
            return CustomerAccount.Load("12345678", "1234", 500.0m, 100.0m);
        }

        [Fact]
        public void Withdraw_ValidAmount_ReturnsValidResult()
        {
            //arrange
            var atm = Atm.Load(8000.0m);
            atm.SetCurrentCustomer(DefaultAccount());
            //act
            var result = atm.Withdraw(200.0m);
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
            Assert.Equal(7800.0m, result.Balance);
            Assert.Equal(7800.0m, atm.Balance);
        }
        [Fact]
        public void Withdraw_NoValidatedCustomer_ReturnsInvalidResult()
        {
            //arrange
            var atm = Atm.Load(8000.0m);
            //act
            var result = atm.Withdraw(200.0m);
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal("ACCOUNT_ERR", result.FailureMessage);
        }
        [Fact]
        public void Withdraw_ExceedBalance_ReturnsInvalidResult()
        {
            //arrange
            var atm = Atm.Load(8000.0m);
            atm.SetCurrentCustomer(DefaultAccount());
            //act
            var result = atm.Withdraw(8001.0m);
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal(8000.0m, result.Balance);
            Assert.Equal(8000.0m, atm.Balance);
            Assert.Equal("ATM_ERR", result.FailureMessage);
        }

        [Fact]
        public void TestBuild()
        {
           
            Assert.Equal(1, 1);
            Assert.Equal(1, 2);
        }

    }
}
