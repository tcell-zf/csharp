namespace TCell.Abstraction
{
    public interface IPlayable
    {
        string Id { get; }
        string SourcePath { get; set; }

        bool Start();
        bool Stop();
        bool ExecuteCommand(string commandText);
    }

    public interface IPlayerHostable
    {
    }
}
