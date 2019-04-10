using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace botTesting
{
    public class Errors : RuntimeResult
    {
        public Errors(CommandError? error, string reason) : base(error, reason)
        {
            
        }
        public static Errors FromError(string reason) =>
            new Errors(CommandError.Unsuccessful, reason);
        public static Errors FromSuccess(string reason = null) =>
            new Errors(null, reason);
    }
}
