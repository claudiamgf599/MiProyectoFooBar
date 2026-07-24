using FooBar.Domain.Exceptions;

namespace FooBar.Domain.Common
{
    public static class ArgumentValidator
    {
        public static object ValidateNull(this object value, string message)
        {
            if (value is null)
            {
                throw new RequiredException(message);
            }
            return value;
        }

        public static string ValidateRequired(this string value, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new RequiredException(message);
            }
            return value;
        }

        public static string ValidateLength(this string value, int minimun, int maximun, string message)
        {
            if (value.Length < minimun || value.Length > maximun)
            {
                throw new RequiredException(message);
            }
            return value;
        }

        public static decimal ValidateGreaterThanZero(this decimal value, string message)
        {
            if (value <= 0)
            {
                throw new RequiredException(message);
            }
            return value;
        }

        public static TEnum ValidateEnum<TEnum>(this TEnum value, string message) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(value))
            {
                throw new RequiredException(message);
            }
            return value;
        }

        public static IEnumerable<T> ValidateNotEmpty<T>(this IEnumerable<T> list, string message)
        {
            if (!list.Any())
            {
                throw new RequiredException(message);
            }
            return list;
        }

        public static Guid ValidateNotEmpty(this Guid value, string message)
        {
            if (value == Guid.Empty)
            {
                throw new RequiredException(message);
            }
            return value;
        }
    }
}
