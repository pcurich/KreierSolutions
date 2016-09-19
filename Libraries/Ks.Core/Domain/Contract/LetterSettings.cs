using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class LetterSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the number of letter is autogenerate.
        /// </summary>
        public bool IsAutogenerate { get; set; }

        /// <summary>
        /// Gets or sets the from number. Only for begin
        /// </summary>
        public int FromNumber { get; set; }

        /// <summary>
        /// Gets or sets the last number. The actual number +1
        /// </summary>
        public int LastNumber { get; set; }

        /// <summary>
        /// Gets or sets the step number.
        /// </summary>
        public int StepNumber { get; set; }

    }
}