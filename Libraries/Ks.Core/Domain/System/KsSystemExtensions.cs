using System;
using System.Collections.Generic;
using System.Linq;

namespace Ks.Core.Domain.System
{
    public static class KsSystemExtensions
    {
        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="ksSystem">Store</param>
        /// <returns>Comma-separated hosts</returns>
        public static string[] ParseHostValues(this KsSystem ksSystem)
        {
            if (ksSystem == null)
                throw new ArgumentNullException("ksSystem");

            var parsedValues = new List<string>();
            if (!String.IsNullOrEmpty(ksSystem.Hosts))
            {
                string[] hosts = ksSystem.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string host in hosts)
                {
                    var tmp = host.Trim();
                    if (!String.IsNullOrEmpty(tmp))
                        parsedValues.Add(tmp);
                }
            }
            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a store contains a specified host
        /// </summary>
        /// <param name="ksSystem">Store</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public static bool ContainsHostValue(this KsSystem ksSystem, string host)
        {
            if (ksSystem == null)
                throw new ArgumentNullException("ksSystem");

            if (String.IsNullOrEmpty(host))
                return false;

            var contains = ksSystem.ParseHostValues()
                                .FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
            return contains;
        }
         
    }
}