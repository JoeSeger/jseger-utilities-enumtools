using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSeger.Utilities.Enumtools.Runtime
{
    public static class EnumExtensions
    {
        public class DescriptionAttribute : Attribute
        {
            public string Description { get; }

            public DescriptionAttribute(string description)
            {
                Description = description;
            }
        }
        // public enum TestEnum
        // {
        //     [Description("Value1 description")]
        //     Value1,
        //     [Description("Value2 description")]
        //     Value2,
        //     [Description("Value3 description")]
        //     Value3,
        // }

        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : Enum
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (item.GetDescription().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return defaultValue;
        }

        public static T Next<T>(this T src) where T : Enum
        {
            var arr = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf(arr, src) + 1;
            return (arr.Length == j) ? arr[0] : arr[j];
        }

        public static int GetEnumCount<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;

        public static IEnumerable<T> GetAllValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>();

        private static bool IsFlagSet<T>(T flags, T value) where T : Enum
        {
            var flagValue = Convert.ToInt32(value);
            var flagsValue = Convert.ToInt32(flags);

            return (flagsValue & flagValue) == flagValue && flagValue != 0;
        }

        public static List<string> GetFlagNames<T>(T flags) where T : Enum =>
            (from T flag in Enum.GetValues(typeof(T)) where flags.HasFlag(flag) select flag.ToString()).ToList();

        public static List<T> GetFlagValues<T>(T flags) where T : Enum =>
            (from T flag in Enum.GetValues(typeof(T)) where flags.HasFlag(flag) select flag).ToList();

        public static string GetFirstSetFlagName<T>(T flags) where T : Enum
        {
            TestIsFlagSet<T>(flags);
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (IsFlagSet(flags, value))
                {
                    return value.ToString();
                }
            }

            return
                default(T)?.ToString(); // Return the default value (usually the 'None' equivalent) if no flags are set
        }
        
        public static TEnum FromInt<TEnum>(int value) where TEnum : Enum
        {
            if (Enum.IsDefined(typeof(TEnum), value))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), value);
            }

            throw new ArgumentException("Invalid enum value");
        }
        public static T GetFirstSetFlag<T>(T flags) where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (IsFlagSet(flags, value))
                {
                    return value;
                }
            }

            return default; // Return the default value (usually the 'None' equivalent) if no flags are set
        }

        public static string[] GetNames(this Enum checkingEnum) => Enum.GetNames(checkingEnum.GetType());

        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable.GetType() != value.GetType())
                throw new ArgumentException(
                    "The checked flag is not from the same type as the checked variable.");

            var uInt64 = Convert.ToUInt64(value);
            var num2 = Convert.ToUInt64(variable);

            return (num2 & uInt64) == uInt64;
        }

        public static IEnumerable<T> GetValues<T>()
        {
            var values = (T[])typeof(T).GetEnumValues();
            return values?.Where(v => v.ToString() != "Invalid" && v.ToString() != "Amount").ToArray();
        }

        public static int GetCount(this Enum checkingEnum) => checkingEnum.GetNames().Length;

        public static T GetByID<T>(this Enum checkingEnum, int id) where T : struct, Enum =>
            Enum.Parse<T>(checkingEnum.GetNames()[id]);

        
        public static void TestIsFlagSet<T>(T flags) where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                Console.WriteLine($"{value}: {IsFlagSet(flags, value)}");
            }
        }

        public static string ToFormattedString<T>(this T flags) where T : Enum
        {
            var sb = new StringBuilder();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (IsFlagSet(flags, value))
                {
                    sb.AppendLine(value.ToString());
                }
            }

            return sb.ToString().TrimEnd(); // Remove the trailing newline
        }
    }
}