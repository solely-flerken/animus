namespace Packages.Animus.Unity.Runtime.Core.Memory
{
    public struct DialogLine
    {
        public string Speaker { get; }
        public string Text { get; }

        public DialogLine(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
        }
    }
}