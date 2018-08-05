using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;
using System.Collections.Generic;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class InstructionsExtensions
    {
        public static Instruction ToInstruction(this Instructions source)
        {
            return new Instruction
            {
                Description = source.Description,
                Url = source.Url
            };
        }

        public static IEnumerable<Instruction> ToInstructionEnumerable(this IEnumerable<Instructions> source)
        {
            foreach (var item in source)
            {
                yield return item.ToInstruction();
            }
        }
    }
}