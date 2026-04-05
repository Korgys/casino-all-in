namespace casino.console.Games.Commons;

/// <summary>
/// Defines console operations required by <see cref="ConsoleFrameBuffer"/>.
/// </summary>
public interface IConsoleFrameTarget
{
    bool SupportsCursorPositioning { get; }

    int? WindowWidth { get; }

    int? WindowHeight { get; }

    void Clear();

    void SetCursorPosition(int left, int top);

    void Write(string value);

    void WriteLine();
}
