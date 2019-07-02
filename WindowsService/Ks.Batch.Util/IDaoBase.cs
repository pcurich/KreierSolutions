namespace Ks.Batch.Util
{
    public interface IDaoBase
    {
        /// <summary>
        /// Conexion a la base de datos
        /// </summary>
        void Connect(); 

        /// <summary>
        /// Cierre de conexiones en la base de datos
        /// </summary>
        void Close();
    }
}