namespace Ks.Batch.Util
{
    public interface IBatchContainer
    {
        /// <summary>
        /// Inicia el servicio 
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Detiene el servicio
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// Pausa el servicio
        /// </summary>
        /// <returns></returns>
        bool Pause();

        /// <summary>
        /// Reanuda el servicio
        /// </summary>
        /// <returns></returns>
        bool Continue();

        /// <summary>
        /// Comandos presonalizados para actividades especificas
        /// </summary>
        /// <param name="commandNumber">128-255</param>
        void CustomCommand(int commandNumber);
    }
}