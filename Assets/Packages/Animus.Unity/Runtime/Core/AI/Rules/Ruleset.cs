using System.Collections.Generic;
using System.Linq;

namespace Packages.Animus.Unity.Runtime.Core.AI.Rules
{
    public class Ruleset
    {
        private IReadOnlyList<string> Rules { get; }

        public static Ruleset Empty { get; } = new();

        public Ruleset(params string[] rules)
        {
            Rules = rules?.ToList() ?? new List<string>();
        }

        public static Ruleset Combine(params Ruleset[] rulesets)
        {
            var combinedRules = rulesets
                .SelectMany(rs => rs.Rules)
                .Distinct()
                .ToArray();

            return new Ruleset(combinedRules);
        }

        public static implicit operator List<string>(Ruleset ruleset)
        {
            return ruleset?.Rules.ToList() ?? new List<string>();
        }
    }
}