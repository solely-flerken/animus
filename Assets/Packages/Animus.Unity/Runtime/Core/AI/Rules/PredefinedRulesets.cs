namespace Packages.Animus.Unity.Runtime.Core.AI.Rules
{
    public static class PredefinedRulesets
    {
        public static readonly Ruleset CommonAgent = new(
            Rule.DoNotBreakCharacter
        );
    }
}