using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGameEngine
{
    public static class Extensions
    {
        public static T[] RemoveIndex<T>(this T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index >= array.Length)
                throw new ArgumentException("Index out of bounds.", "index");
            var newArray = new T[array.Length - 1];
            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            if (index < array.Length - 1)
                Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
            return newArray;
        }
    }
}
