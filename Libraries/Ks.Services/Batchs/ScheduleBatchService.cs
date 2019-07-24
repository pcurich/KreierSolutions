using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core.Data;
using Ks.Core.Domain.Batchs;

namespace Ks.Services.Batchs
{
    /// <summary>
    /// Schedule Batch Service
    /// </summary>
    public class ScheduleBatchService : IScheduleBatchService
    {
         #region Fields

        private readonly IRepository<ScheduleBatch> _batchRepository;

        #endregion

        #region Ctor

        public ScheduleBatchService(IRepository<ScheduleBatch> batchRepository)
        {
            this._batchRepository = batchRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a Batch
        /// </summary>
        /// <param name="batch">Batch</param>
        public virtual void DeleteBatch(ScheduleBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException("batch");

            _batchRepository.Delete(batch);
        }

        /// <summary>
        /// Gets a Batch
        /// </summary>
        /// <param name="batchId">Batch identifier</param>
        /// <returns>Batch</returns>
        public virtual ScheduleBatch GetBatchById(int batchId)
        {
            if (batchId == 0)
                return null;

            return _batchRepository.GetById(batchId);
        } 

        /// <summary>
        /// Gets all Batchs
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Batchs</returns>
        public virtual IList<ScheduleBatch> GetAllBatchs(bool showHidden = false)
        {
            var query = _batchRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.FrecuencyId);

            var batchs = query.ToList();
            return batchs;
        }

        /// <summary>
        /// Inserts a Batch
        /// </summary>
        /// <param name="batch">Batch</param>
        public virtual void InsertBatch(ScheduleBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException("batch");

            _batchRepository.Insert(batch);
        }

        /// <summary>
        /// Updates the Batch
        /// </summary>
        /// <param name="batch">Batch</param>
        public virtual void UpdateBatch(ScheduleBatch batch)
        {
            if (batch == null)
                throw new ArgumentNullException("batch");

            _batchRepository.Update(batch);
        }

        #endregion
    }
}