using System.Collections.Generic;
using Ks.Core.Domain.Logging;
using Ks.Core.Domain.Security;

namespace Ks.Services.Logging
{
    public static class DefaultActivityLogType
    {
        public static readonly ActivityLogType ActivityLogEditQuevedEmail = new ActivityLogType { SystemKeyword = "EditQuevedEmail", Name = "Edición de email", Enabled = true };
        public static readonly ActivityLogType ActivityLogEditScheduleTask = new ActivityLogType { SystemKeyword = "EditScheduleTask", Name = "Edición del servicio de tareas",  Enabled = true };
        public static readonly ActivityLogType ActivityLogEditScheduleBatch = new ActivityLogType { SystemKeyword = "EditScheduleBatch", Name = "Edición de los servicios de Windows", Enabled = true };

        public static readonly ActivityLogType ActivityLogNewCustomer = new ActivityLogType { SystemKeyword = "NewCustomer", Name = "Nuevo asociado", Enabled = true };
        public static readonly ActivityLogType ActivityLogEditCustomer = new ActivityLogType { SystemKeyword = "EditCustomer", Name = "Modificar a los asociados", Enabled = true };

        public static IEnumerable<ActivityLogType> GetMenuSystem()
        {
            return new[]
            {
                ActivityLogEditScheduleTask,
                ActivityLogEditScheduleBatch
            };
        }
    }
}