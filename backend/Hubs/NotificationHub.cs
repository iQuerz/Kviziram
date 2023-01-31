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
        var con = Context.GetHttpContext();
        if(con != null) {
            string? neoID = con.Request.Query["neoID"].ToString();               
            if (neoID != null) {
                Groups.AddToGroupAsync(Context.ConnectionId, neoID).GetAwaiter().GetResult();
            }
        }
        return base.OnConnectedAsync();
    }
}
