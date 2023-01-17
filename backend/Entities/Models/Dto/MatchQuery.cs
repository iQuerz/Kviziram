public class MatchQuery
{
    public Guid? HostID { get; set; }
    public Guid? WinnerID { get; set; }
    public bool IsSearchable { get; set; } = true;
}