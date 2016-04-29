namespace TCell.Abstraction
{
    public interface IPlayable
    {
        string Id { get; }
        string Source { get; set; }

        bool ExecuteCommand(string cmmandText);
    }
}
