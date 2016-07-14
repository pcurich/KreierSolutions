
using Ks.Core.Configuration;

namespace Ks.Core.Domain.Media
{
    public class MediaSettings : ISettings
    { 
        public int AutoCompleteSearchThumbPictureSize { get; set; }

        public bool DefaultPictureZoomEnabled { get; set; }

        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Geta or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }
        
        /// <summary>
        /// Geta or sets a vaue indicating whether single (/content/images/thumbs/) or multiple (/content/images/thumbs/001/ and /content/images/thumbs/002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }
    }
}