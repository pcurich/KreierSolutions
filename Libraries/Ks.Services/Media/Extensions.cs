﻿using System;
using System.IO;
using System.Linq;
using System.Web;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Media;


namespace Ks.Services.Media
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the download binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>Download binary array</returns>
        public static byte[] GetDownloadBits(this HttpPostedFileBase postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            var binary = new byte[size];
            fs.Read(binary, 0, size);
            return binary;
        }

        /// <summary>
        /// Gets the picture binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>Picture binary array</returns>
        public static byte[] GetPictureBits(this HttpPostedFileBase postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            var img = new byte[size];
            fs.Read(img, 0, size);
            return img;
        }
        
    }
}
