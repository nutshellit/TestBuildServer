using System;
using Xunit;
using AtmCore;
namespace AtmTests
{
    public class CustomerAccountFixtures
    {
        CustomerAccount DefaultAccount()
        {
            return CustomerAccount.Load("12345678", "1234", 500.0m, 100.0m);
        }
        [Fact]
        public void ValidatePIN_ValidPin_ReturnsValidResult()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.ValidatePin("1234");
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
        }

        [Fact]
        public void GetBalance_WhenRequested_ReturnsCustomerBalance()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.GetBalance();
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
            Assert.Equal(500.0m, result.CustomerBalance);
        }

        [Fact]
        public void ValidatePIN_InvalidPin_ReturnsInvalidResult()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.ValidatePin("4321");
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal("ACCOUNT_ERR", result.FailureMessage);
        }

        [Fact]
        public void Withdraw_ValidAmount_ReturnsValidResult()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.Withdraw(200.0m);
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
            Assert.Equal(300.0m, result.CustomerBalance);
            Assert.Equal(300.0m, cust.AccountBalance);
            Assert.Equal(200.0m, result.Amount);
        }
        [Fact]
        public void Withdraw_TwoValidAmounts_ReturnsValidResult()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result1 = cust.Withdraw(200.0m);
            var result2 = cust.Withdraw(100.0m);
            //assert
            Assert.Equal(TransactionOutcome.Success, result2.Result);
            Assert.Equal(200.0m, result2.CustomerBalance);
            Assert.Equal(200.0m, cust.AccountBalance);
            Assert.Equal(100.0m, result2.Amount);

        }

        [Fact]
        public void Withdraw_ValidAmountWithinOverdraft_ReturnsValidResult()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.Withdraw(599.0m);
            //assert
            Assert.Equal(TransactionOutcome.Success, result.Result);
            Assert.Equal(-99.0m, result.CustomerBalance);
            Assert.Equal(-99.0m, cust.AccountBalance);
        }

        [Fact]
        public void Withdraw_InValidAmount_ReturnsInvalidResult()
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.Withdraw(601.0m);
            //assert
            Assert.Equal(TransactionOutcome.Failure, result.Result);
            Assert.Equal("FUNDS_ERR", result.FailureMessage);
            Assert.Equal(500.0m, result.CustomerBalance);
            Assert.Equal(500.0m, cust.AccountBalance);
            Assert.Equal(0, result.Amount);
        }

        [Theory]
        [InlineData(600.0, TransactionOutcome.Success, -100, -100, 600, "")]
        [InlineData(598.0, TransactionOutcome.Success, -98, -98, 598, "")]
        [InlineData(600.1, TransactionOutcome.Failure, 500, 500, 0,"FUNDS_ERR")]
        [InlineData(599.9999, TransactionOutcome.Success, -99.9999, -99.9999, 599.9999, "")]
        public void Withdraw_DiffAmounts_Result(decimal amount, TransactionOutcome expectOutcome, 
            decimal expectedResult, decimal expectedAccBal, decimal expectedResultAmount,
            string expectedFailureMessage)
        {
            //arrange
            var cust = DefaultAccount();
            //act
            var result = cust.Withdraw(amount);
            //assert
            Assert.Equal(expectOutcome, result.Result);
            Assert.Equal(expectedResult, result.CustomerBalance);
            Assert.Equal(expectedAccBal, cust.AccountBalance);
            Assert.Equal(expectedResultAmount, result.Amount);
            Assert.Equal(expectedFailureMessage, result.FailureMessage);
            
        }

    }
}
