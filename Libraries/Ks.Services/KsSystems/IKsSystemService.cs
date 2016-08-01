using System.Collections.Generic;
using Ks.Core.Domain.System;

namespace Ks.Services.KsSystems
{
    /// <summary>
    /// KsSystem service interface
    /// </summary>
    public partial interface IKsSystemService
    {
        /// <summary>
        /// Deletes a ksSystem
        /// </summary>
        /// <param name="ksSystem">KsSystem</param>
        void DeleteKsSystem(KsSystem ksSystem);

        /// <summary>
        /// Gets all ksSystem
        /// </summary>
        /// <returns>ksSystem</returns>
        IList<KsSystem> GetAllKsSystems();

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="ksSystemId">ksSystem identifier</param>
        /// <returns>KsSystem</returns>
        KsSystem GetKsSystemById(int ksSystemId);

        /// <summary>
        /// Inserts a ksSystem
        /// </summary>
        /// <param name="ksSystem">KsSystem</param>
        void InsertKsSystem(KsSystem ksSystem);

        /// <summary>
        /// Updates the ksSystem
        /// </summary>
        /// <param name="ksSystem">KsSystem</param>
        void UpdateKsSystem(KsSystem ksSystem);
    }
}