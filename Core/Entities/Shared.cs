using System;
using System.Collections.Generic;
using System.Text;

namespace AtmCore
{
    public enum TransactionOutcome { Success, Failure }
    public abstract class TransactionResult
    {
        public TransactionResult(TransactionOutcome result, string failureMessage)
        {
            Result = result;
            FailureMessage = failureMessage;
        }

        public TransactionOutcome Result { get; set; }
        public string FailureMessage { get; set; }
    }
}
