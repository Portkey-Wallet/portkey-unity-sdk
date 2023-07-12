using NUnit.Framework;
using Portkey.Core;
using Portkey.DID;

namespace Portkey.Test
{
    public class AccountProviderTest
    {
        [Test]
        public void CreateTest()
        {
            IAccountProvider<AccountBase> accountProvider = new AccountProvider();
            var account = accountProvider.CreateAccount();
        }
    }
}