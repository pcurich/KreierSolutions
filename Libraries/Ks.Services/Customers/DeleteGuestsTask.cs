using System;
using Ks.Services.Tasks;

namespace Ks.Services.Customers
{
    /// <summary>
    /// Represents a task for deleting guest customers
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        private readonly ICustomerService _customerService;

        public DeleteGuestsTask(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            //60*24 = 1 day
            const int OLDER_THAN_MINUTES = 1440; //TODO move to settings
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _customerService.DeleteGuestCustomers(null, DateTime.UtcNow.AddMinutes(-OLDER_THAN_MINUTES));
        }
    }
}