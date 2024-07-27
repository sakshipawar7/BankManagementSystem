using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankManagementSystem.Helpers
{
    public class EnumToStringConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : Enum
    {
        public EnumToStringConverter()
            : base(
                v => v.ToString(),
                v => (TEnum)Enum.Parse(typeof(TEnum), v))
        { }
    }
}
