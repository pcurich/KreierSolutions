using System.Data.Entity.ModelConfiguration;

namespace Ks.Data.Mapping
{
    public abstract class KsEntityTypeConfiguration<T> : EntityTypeConfiguration<T> where T : class
    {
        protected KsEntityTypeConfiguration()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
    }
}