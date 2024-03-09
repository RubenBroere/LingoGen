using System.Text;

namespace LingoGen.Generator;

public class SourceBuilder
{
    private readonly StringBuilder _sb = new();

    private int _indentLevel;

    public SourceBuilder AppendLine(string? line)
    {
        _sb.Append(new string(' ', _indentLevel * 4));
        _sb.AppendLine(line);
        return this;
    }

    public SourceBuilder AppendLine()
    {
        _sb.AppendLine();
        return this;
    }

    public SourceBuilder Append(string? line)
    {
        _sb.Append(new string(' ', _indentLevel * 4));
        _sb.Append(line);
        return this;
    }

    public SourceBuilder IncreaseIndent()
    {
        _indentLevel++;
        return this;
    }

    public SourceBuilder DecreaseIndent()
    {
        _indentLevel--;
        return this;
    }

    public Region EnterBlock() => EnterIndentedRegion("{", "}");

    public Region EnterIndentedRegion() => EnterIndentedRegion("", "");

    public Region EnterIndentedRegion(string open, string close)
    {
        AppendLine(open);
        IncreaseIndent();
        return new(this, close);
    }

    public override string ToString()
    {
        return _sb.ToString();
    }

    public readonly struct Region(SourceBuilder builder, string close) : IDisposable
    {
        public void Dispose()
        {
            builder.DecreaseIndent();
            builder.AppendLine(close);
        }
    }

    public SourceBuilder Clear()
    {
        _sb.Clear();
        _indentLevel = 0;
        return this;
    }
}