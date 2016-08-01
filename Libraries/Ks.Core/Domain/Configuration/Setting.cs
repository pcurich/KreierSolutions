﻿using Ks.Core.Domain.Localization;

namespace Ks.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity, ILocalizedEntity
    {
        public Setting() { }

        public Setting(string name, string value, int ksSystemId = 0)
        {
            this.Name = name;
            this.Value = value;
            this.KsSystemId = ksSystemId;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the KsSystem for which this setting is valid. 0 is set when the setting is for all KsSystem
        /// </summary>
        public int KsSystemId { get; set; }

        public override string ToString()
        {
            return Name;
        }
         
    }
}