namespace Ks.Core.Domain.Directory
{
    /// <summary>
    /// Represents a City
    /// </summary>
    public partial class City:BaseEntity
    {
        /// <summary>
        /// Gets or sets the stateprovince identifier
        /// </summary>
        public int StateProvinceId { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the stateprovince
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }
    }
}