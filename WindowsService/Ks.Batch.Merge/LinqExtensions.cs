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
            int i = 1;
            var root = new List<IList<T>>();
            IList<T> child = new List<T>();
            
            foreach (var l in list)
            {
                if (i == parts)
                {
                    child.Add(l);
                    root.Add(child);
                    child = new List<T>();
                    i = 1;
                }
                else
                {
                    child.Add(l);
                    i++;
                }
            }
            root.Add(child);
            //var splits = from item in list
            //             group item by i++ % parts into part
            //             select part.AsEnumerable();
            return root;
        }
    }
}