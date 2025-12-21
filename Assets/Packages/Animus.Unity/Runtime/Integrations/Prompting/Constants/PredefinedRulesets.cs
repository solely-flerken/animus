namespace Packages.Animus.Unity.Runtime.Integrations.Prompting.Constants
{
    public static class PredefinedRulesets
    {
        public static readonly Ruleset CommonAgent = new(
            Rule.DoNotBreakCharacter,
            Rule.NoAssumptions
        );
    }
}