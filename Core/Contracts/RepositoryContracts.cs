using System;
using System.Collections.Generic;
using System.Text;

namespace AtmCore
{
    public interface IAtmRepository
    {
        Atm Get();
    }

    public interface IAccountRepository
    {
        CustomerAccount Get(string accountNo);
    }
}
