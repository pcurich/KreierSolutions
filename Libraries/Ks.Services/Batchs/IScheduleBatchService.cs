using System.Collections.Generic;
using Ks.Core.Domain.Batchs;

namespace Ks.Services.Batchs
{
    public interface IScheduleBatchService
    {
        /// <summary>
        /// Deletes a batch
        /// </summary>
        /// <param name="batch">batch</param>
        void DeleteBatch(ScheduleBatch batch);

        /// <summary>
        /// Gets a Batch
        /// </summary>
        /// <param name="batchId">Batch identifier</param>
        /// <returns>Batch</returns>
        ScheduleBatch GetBatchById(int batchId); 

        /// <summary>
        /// Gets all Batchs
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Batchs</returns>
        IList<ScheduleBatch> GetAllBatchs(bool showHidden = false);

        /// <summary>
        /// Inserts a Batch
        /// </summary>
        /// <param name="batch">Batch</param>
        void InsertBatch(ScheduleBatch batch);

        /// <summary>
        /// Updates the Batch
        /// </summary>
        /// <param name="batch">Batch</param>
        void UpdateBatch(ScheduleBatch batch);
    }
}