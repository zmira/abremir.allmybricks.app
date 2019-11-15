using System;
using System.ComponentModel;

namespace abremir.AllMyBricks.ThirdParty.Brickset.Extensions
{
    public static class TypeExtension
    {
        public static string GetDescription(this Type type)
        {
            var descriptions = (DescriptionAttribute[])
                type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }
            return descriptions[0].Description;
        }
    }
}
