using Ks.Core.Configuration;

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
        /// Gets or sets the authorize loan.
        /// </summary>
        public int AuthorizeLoan { get; set; }

        /// <summary>
        /// Gets or sets the registration cash.
        /// </summary>
        public int RegistrationCash { get; set; }

        /// <summary>
        /// Gets or sets the registration form.
        /// </summary>
        public int RegistrationForm { get; set; }

        /// <summary>
        /// Gets or sets the number of liquidation.
        /// </summary>
        public int NumberOfLiquidation { get; set; }
    }
}