public class GameInviteDto
{
    public AccountPoco? FromUser { get; set; }

    public string InviteCode { get; set; } = string.Empty;
}