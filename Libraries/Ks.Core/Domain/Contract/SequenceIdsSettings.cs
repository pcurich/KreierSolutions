﻿using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class SequenceIdsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the last number. The actual number +1
        /// </summary>
        public int DeclaratoryLetter { get; set; }

        /// <summary>
        /// Gets or sets the last number. The actual number +1
        /// </summary>
        public int AuthorizeDiscount { get; set; }

        /// <summary>
        /// Gets or sets the registration form.
        /// </summary>
        public int RegistrationForm { get; set; }

    }
}