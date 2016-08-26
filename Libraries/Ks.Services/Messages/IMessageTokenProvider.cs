using System.Collections.Generic;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Messages;
using Ks.Core.Domain.System;

namespace Ks.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddSystemTokens(IList<Token> tokens, KsSystem system, EmailAccount emailAccount);
        void AddCustomerTokens(IList<Token> tokens, Customer customer);
        string[] GetListOfAllowedTokens();
    }
}