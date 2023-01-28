using Microsoft.AspNetCore.SignalR;

public class NotificationHub: Hub
{
    KviziramContext _context;
    Utility _util;

    public NotificationHub(KviziramContext context, Utility utility) {
        _context = context;
        _util = utility;
    }

    public override Task OnConnectedAsync() {
        Groups.AddToGroupAsync(Context.ConnectionId, _util.CallerAccountExists().ID.ToString()).GetAwaiter().GetResult();
        return base.OnConnectedAsync();
    }
}
