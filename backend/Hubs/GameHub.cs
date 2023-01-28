using Microsoft.AspNetCore.SignalR;
using Neo4jClient;
using StackExchange.Redis;

public class GameHub: Hub
{
    KviziramContext _context;
    IDatabase _redis;
    IGraphClient _neo;
    Utility _util;
    ISubscriber _matchPubSub;
    string InviteCode { get; set; } = string.Empty;
    (Guid Id, string Sid, string Name) _user;

    public GameHub(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _matchPubSub = context.Redis.GetSubscriber();
    }

    public override Task OnConnectedAsync() {
        var con = Context.GetHttpContext();
        if(con != null) {
            string? sid = con.Request.Headers["SessionID"];               
            if (sid != null) {
                string? account = _redis.StringGetAsync(_util.RK_Account(sid)).GetAwaiter().GetResult().ToString();
                if (account != null) {
                    var accountExists = _util.DeserializeAccountPoco(account);
                    if (accountExists != null )
                        _user = (accountExists.ID, _util.RK_Account(sid), accountExists.Username);
                }
            }
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception) {

        _matchPubSub.PublishAsync(_util.RK_GameWatcher(InviteCode), $"Disconnected:{_user.Sid}|{_user.Id}").GetAwaiter().GetResult();
        _matchPubSub.PublishAsync(_util.RK_GameWatcher(InviteCode), $"Chat:{_user.Name} has left the game.").GetAwaiter().GetResult();

        _matchPubSub.UnsubscribeAsync(_util.RK_GameWatcher(InviteCode)).GetAwaiter().GetResult();

        return base.OnDisconnectedAsync(exception);
    }

    
    //Ako uspe konekcija na klijentu, ova funkcija se odma zove u .then() delu
    public async Task OnJoinGame(string inviteCode) {
        this.InviteCode = inviteCode;
        await SetupMatchPubSub(this.InviteCode);

        await _matchPubSub.PublishAsync(_util.RK_GameWatcher(inviteCode), $"Connected:{_user.Sid}|{_user.Id}");
        if (await _redis.HashExistsAsync(_util.RK_Lobby(inviteCode), _user.Id.ToString()))
            await _matchPubSub.PublishAsync(_util.RK_GameWatcher(inviteCode), $"Chat:{_user.Name} joined the game.");
    }

    //Chat poziv
    public async Task SendChatMessage(string message) {
        if (await _redis.HashExistsAsync(_util.RK_Lobby(InviteCode), _user.Id.ToString()))
            await _matchPubSub.PublishAsync(_util.RK_GameWatcher(InviteCode), $"Chat:{_user.Name}: {message}");
    }

    //Odgovor poziv, ne znam dal moze INT pri pozivu pa bolje samo parse da odradimo u funkciju
    public async Task SendAnswer(string answer) {
        if (await _redis.HashExistsAsync(_util.RK_Lobby(InviteCode), _user.Id.ToString())) {
            await _matchPubSub.PublishAsync(_util.RK_GameWatcher(InviteCode), $"Answered:{_user.Id}|{answer}");
            await _matchPubSub.PublishAsync(_util.RK_GameWatcher(InviteCode), $"Chat:{_user.Name} answered the question.");
        }
    }

    public async Task SetupMatchPubSub(string inviteCode) {
        await _matchPubSub.SubscribeAsync(_util.RK_GameActions(inviteCode), async (channel, message) => {
            string[] msgOperation = message.ToString().Split(":", 2);;
            Console.Write(msgOperation[0] + " ----- " + msgOperation[1]);

            string gameInviteCode = channel.ToString().Split(':')[1];
            Console.WriteLine(inviteCode);

            //Start - Chat - NextQuestion - Finished - Answered - Disconnected - Reconnected - Connected
            switch(msgOperation[0]) {
                case "Start": {
                    //msgOperation[1] je inviteCode
                    await Clients.Caller.SendAsync("receiveGameStarted", msgOperation[1]);
                    break;
                }

                case "Chat": {
                    //msgOperation[1] je poruka
                    await Clients.Caller.SendAsync("receiveChatMessage", msgOperation[1]);
                    break;
                }
                
                case "NextQuestion": {
                    //Ucitaj sledece pitanje preko game kontrolera -> [HttpGet("{inviteCode}/question/{quizID}")]
                    await Clients.Caller.SendAsync("receiveNextQuestion");
                    break;
                }

                case "Finished": {
                    //Game je zavrsen, povuci podatke o igri preko match kontrolera [HttpGet]
                    //msgOperation[1] je GUID od game-a koji je sacuvan u neo4j i to se koristi da se povuce
                    await Clients.Caller.SendAsync("receiveFinishedGame", msgOperation[1]);
                    break;
                }

                case "Answered": {
                    //msgOperation[1] je player id tj. GUID (nije session id)
                    await Clients.Caller.SendAsync("receiveAnswered", msgOperation[1]);
                    break;
                }

                case "Connected": {
                    //msgOperation[1] je player id tj. GUID (nije session id)
                    await Clients.Caller.SendAsync("receiveConnected", msgOperation[1]);
                    break;
                }

                case "Reconnected": {
                    //msgOperation[1] je player id tj. GUID (nije session id)
                    await Clients.Caller.SendAsync("receiveReconnected", msgOperation[1]);
                    break;
                }

                case "Disconnected": {
                    //msgOperation[1] je player id tj. GUID (nije session id)
                    await Clients.Caller.SendAsync("receiveDisconnected", msgOperation[1]);
                    break;
                }

                case "Kickout": {
                    //msgOperation[1] je inviteCode kome igrac nije mogao da pristupi
                    await Clients.Caller.SendAsync("receiveKickout", msgOperation[1]);
                    break;
                }

                default: {
                    Console.WriteLine("How did we end up here?");
                    break;
                }
            }
        });
    }


    
}