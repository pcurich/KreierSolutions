using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Tokenizer;
using Ks.Core;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Messages;
using Ks.Services.Customers;
using Ks.Services.Events;
using Ks.Services.KsSystems;
using Ks.Services.Localization;

namespace Ks.Services.Messages
{
    public partial class FakeWorkflowMessageService : IWorkflowMessageService
    {
        public int SendCustomerRegisteredNotificationMessage(Customer customer, int languageId)
        {
            return 0;
        }

        public int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            return 0;
        }

        public int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            return 0;
        }

        public int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            return 0;
        }

        public int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            return 0;
        }
    }
}