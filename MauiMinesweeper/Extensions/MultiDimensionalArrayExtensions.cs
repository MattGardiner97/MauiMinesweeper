using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMinesweeper.Extensions
{
    internal static class MultiDimensionalArrayExtensions
    {
        internal static IEnumerable<T> Flatten<T>(this T[,] array)
        {
            for(var y = 0; y < array.GetLength(1); y++)
            {
                for(var x = 0;x < array.GetLength(0); x++)
                {
                    yield return array[x, y];
                }
            }
        }
    }
}
