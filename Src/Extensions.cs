using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

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

        public static T[] RemoveIndexes<T>(this T[] array, int index, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException("index", "Index out of bounds.");
            if (length < 0 || index + length > array.Length)
                throw new ArgumentOutOfRangeException("length", "Length cannot be negative or out of bounds.");
            var newArray = new T[array.Length - length];
            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            if (index + length < array.Length)
                Array.Copy(array, index + length, newArray, index, array.Length - index - length);
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

        public static T[] InsertAtIndex<T>(this T[] array, int index, T element)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0 || index > array.Length)
                throw new ArgumentException("Index out of bounds.", "index");
            var newArray = new T[array.Length + 1];
            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            newArray[index] = element;
            if (index < array.Length)
                Array.Copy(array, index, newArray, index + 1, array.Length - index);
            return newArray;
        }

        public static TransitionResult With(this QuizStateBase state, string jsMethod, object jsParams = null)
        {
            return new TransitionResult(state, null, jsMethod, jsParams);
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

        public static ConsoleColoredString ToUsefulString(this object obj)
        {
            if (obj is IToConsoleColoredString)
                return ((IToConsoleColoredString) obj).ToConsoleColoredString();
            if (obj is string)
                return ((string) obj).Color(ConsoleColor.Yellow);
            if (ExactConvert.IsTrueIntegerType(obj.GetType()))
                return obj.ToString().Color(ConsoleColor.Red);
            if (obj is bool)
                return obj.ToString().Color(ConsoleColor.Green);

            var t = obj.GetType();
            if (t.IsArray)
                return "{0/White} × {1/DarkCyan}".Color(null).Fmt(((dynamic) obj).Length, t.GetElementType().Name);

            Type[] arguments;
            if (t.TryGetInterfaceGenericParameters(typeof(IDictionary<,>), out arguments))
                return "{0/White} × {1/DarkMagenta} → {2/DarkCyan}".Color(null).Fmt(((dynamic) obj).Count, arguments[0].Name, arguments[1].Name);
            if (t.TryGetInterfaceGenericParameters(typeof(ICollection<>), out arguments))
                return "{0/White} × {1/DarkCyan}".Color(null).Fmt(((dynamic) obj).Count, arguments[0].Name);

            return obj.ToConsoleColoredString(ConsoleColor.Magenta);
        }
    }
}
