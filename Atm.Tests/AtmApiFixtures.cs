using AtmCore;
using AtmCore.Contracts;
using AtmCore.Contracts.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AtmTests
{
    class MockAtmRepo : IAtmRepository
    {
        public Atm Get()
        {
            return Atm.Load(8000);
        }
    }

    class MockCustomerAccRepo : IAccountRepository
    {
        private readonly decimal accBal;
        private readonly decimal overdraft;
        private readonly string accNo;

        public MockCustomerAccRepo(decimal accBal, decimal overdraft, string accNo)
        {
            this.accBal = accBal;
            this.overdraft = overdraft;
            this.accNo = accNo;
        }

        public CustomerAccount Get(string accountNo)
        {
            if (accountNo == accNo)
                return CustomerAccount.Load(accountNo, "1234", accBal, overdraft);
            return null;
        }
    }

    public class AtmApiFixtures
    {
        IAtmApi _defaultAtmApi;
        public AtmApiFixtures()
        {
            _defaultAtmApi = new AtmApi(new MockAtmRepo(), new MockCustomerAccRepo(500, 100, "12345678"));
        }

        [Fact]
        public void ValidatePin_ValidPin_ValidResult()
        {
            //act
            var result = _defaultAtmApi.ValidatePin("12345678", "1234");
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
        }

        [Fact]
        public void ValidatePin_InvalidPin_InvalidResult()
        {
            //act
            var result = _defaultAtmApi.ValidatePin("12345678", "4321");
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal("ACCOUNT_ERR", result.FailureMessage);
        }

        [Fact]
        public void CheckBalance_ValidPin_ValidResult()
        {
            //arrange
            var _ = _defaultAtmApi.ValidatePin("12345678", "1234");
            //act
            var result = _defaultAtmApi.GetBalance("12345678");
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
            Assert.Equal(500, result.CustomerBalance);
        }

        [Fact]
        public void CheckBalance_InvalidPin_InvalidResult()
        {
            //arrange
            var _ = _defaultAtmApi.ValidatePin("12345678", "4321");
            //act
            var result = _defaultAtmApi.GetBalance("12345678");
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal(0, result.CustomerBalance);
            Assert.Equal("ACCOUNT_ERR", result.FailureMessage);
        }

        [Fact]
        public void DispenseFunds_ValidPinAndAmount_ValidResult()
        {
            //arrange
            var _ = _defaultAtmApi.ValidatePin("12345678", "1234");
            //act
            var result = _defaultAtmApi.Withdraw("12345678", 200.0m);
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
            Assert.Equal(300, result.CustomerBalance);
            Assert.Equal(200.0m, result.Amount);
        }

        [Fact]
        public void DispenseFunds_InvalidPin_ValidResult()
        {
            //arrange
            var _ = _defaultAtmApi.ValidatePin("12345678", "4321");
            //act
            var result = _defaultAtmApi.Withdraw("12345678", 200.0m);
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal(0, result.CustomerBalance);
            Assert.Equal(0, result.Amount);
            Assert.Equal("ACCOUNT_ERR", result.FailureMessage);
        }

        [Fact]
        public void DispenseFunds_InvalidAmount_ValidResult()
        {
            //arrange
            var _ = _defaultAtmApi.ValidatePin("12345678", "1234");
            //act
            var result = _defaultAtmApi.Withdraw("12345678", 601.0m);
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal(0, result.Amount);
            Assert.Equal("FUNDS_ERR", result.FailureMessage);
        }

        [Fact]
        public void DispenseFunds_AtmInvalidBalance_InvalidResult()
        {
            IAtmApi atmApi = new AtmApi(new MockAtmRepo(), new MockCustomerAccRepo(9000, 100, "12345678"));
            var _ = atmApi.ValidatePin("12345678", "1234");
            //act
            var result = atmApi.Withdraw("12345678", 8001.0m);
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.CustomerBalance);
            Assert.Equal("ATM_ERR", result.FailureMessage);
        }


    }
}
