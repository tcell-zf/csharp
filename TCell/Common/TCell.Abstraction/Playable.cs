namespace TCell.Abstraction
{
    public interface IPlayable
    {
        string Id { get; }
        string SourcePath { get; set; }

        bool ExecuteCommand(string commandText);
    }
}
