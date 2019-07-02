using System.Collections.Generic;
using System.Linq;

namespace Ks.Batch.Merge
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Divide la lista en sublistas 
        /// </summary>
        /// <typeparam name="T">Tipo de dato</typeparam>
        /// <param name="list">Lista de datos</param>
        /// <param name="parts">Partes en la que se quiere dividir</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            List<int> ll = new List<int>{ 1,2,3};

            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }
    }
}