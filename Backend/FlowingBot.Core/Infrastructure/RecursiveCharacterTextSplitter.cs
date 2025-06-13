using System.Text;

namespace FlowingBot.Core.Infrastructure;
internal class RecursiveCharacterTextSplitter
{
    private readonly List<string> _separators;
    private readonly int _chunkSize;
    private readonly int _chunkOverlap;

    public RecursiveCharacterTextSplitter(
        int chunkSize = 1000,
        int chunkOverlap = 200,
        List<string>? separators = null)
    {
        _chunkSize = chunkSize;
        _chunkOverlap = chunkOverlap;
        _separators = separators ?? new List<string> { "\n\n", "\n", " ", "" };
    }

    public List<string> SplitText(string text)
    {
        var chunks = new List<string>();
        SplitRecursive(text, chunks);
        return chunks;
    }

    private void SplitRecursive(string text, List<string> chunks)
    {
        if (text.Length <= _chunkSize)
        {
            chunks.Add(text);
            return;
        }

        var separator = _separators.FirstOrDefault(s => text.Contains(s));
        if (separator == null)
        {
            chunks.Add(text.Substring(0, _chunkSize));
            SplitRecursive(text.Substring(_chunkSize - _chunkOverlap), chunks);
            return;
        }

        var parts = text.Split(new[] { separator }, StringSplitOptions.None);
        var currentChunk = new StringBuilder();

        foreach (var part in parts)
        {
            if (currentChunk.Length + part.Length + separator.Length > _chunkSize)
            {
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                    currentChunk.Append(part);
                }
            }
            else
            {
                if (currentChunk.Length > 0)
                    currentChunk.Append(separator);
                currentChunk.Append(part);
            }
        }

        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());
    }
}