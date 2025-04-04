﻿using System;
using System.Collections.Generic;
using abremir.AllMyBricks.Data.Models;
using abremir.AllMyBricks.DataSynchronizer.Configuration;
using abremir.AllMyBricks.ThirdParty.Brickset.Models;

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
                if (item.Description.Equals(Constants.InstructionNoLongerListedInLego, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                yield return item.ToInstruction();
            }
        }
    }
}
