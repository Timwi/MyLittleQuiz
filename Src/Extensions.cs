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

        public static T[] ReplaceIndex<T>(this T[] array, int index, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index >= array.Length)
                throw new ArgumentException("Index out of bounds.", "index");
            var newArray = new T[array.Length];
            Array.Copy(array, 0, newArray, 0, array.Length);
            newArray[index] = element;
            return newArray;
        }

        public static T[] ReplaceIndex<T>(this T[] array, int index, Func<T, T> elementSelector)
        {
            return array.ReplaceIndex(index, elementSelector(array[index]));
        }

        public static TransitionResult With(this QuizStateBase state, string jsMethod, object jsParams = null)
        {
            return new TransitionResult(state, jsMethod, jsParams);
        }

        public static TransitionResult NoTransition(this QuizStateBase state)
        {
            return new TransitionResult(state, null, null);
        }

        public static IEnumerable<T> Repeat<T>(this T obj, int times)
        {
            return Enumerable.Repeat(obj, times);
        }

        public static T ApplyToClone<T>(this T obj, Action<T> action) where T : ICloneable
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (action == null)
                throw new ArgumentNullException("action");
            var newObj = (T) obj.Clone();
            action(newObj);
            return newObj;
        }
    }
}
