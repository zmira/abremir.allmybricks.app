using abremir.AllMyBricks.Data.Models;
using System.Collections.Generic;
using Managed = abremir.AllMyBricks.Data.Models.Realm;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class InstructionExtensions
    {
        public static Managed.Instruction ToRealmObject(this Instruction source)
        {
            return new Managed.Instruction
            {
                Description = source.Description,
                Url = source.Url
            };
        }

        public static Instruction ToPlainObject(this Managed.Instruction source)
        {
            return new Instruction
            {
                Description = source.Description,
                Url = source.Url
            };
        }

        public static IEnumerable<Managed.Instruction> ToRealmObject(this IEnumerable<Instruction> source)
        {
            foreach (var item in source)
            {
                yield return item.ToRealmObject();
            }
        }

        public static IEnumerable<Instruction> ToPlainObject(this IEnumerable<Managed.Instruction> source)
        {
            foreach (var item in source)
            {
                yield return item.ToPlainObject();
            }
        }
    }
}