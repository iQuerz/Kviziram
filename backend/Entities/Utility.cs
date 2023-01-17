using StackExchange.Redis;
using Neo4jClient;
using System.Text.Json;


public class Utility
{
    private IDatabase _redis;
    private IGraphClient _neo;
    private KviziramContext _context;

    public Utility(KviziramContext context)
    {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
    }

    #region Caller Functions
    public bool isStringSidValid(string sID)
    {
        return sID.Contains("account") || sID.Contains("guest");
    }

    public void CallerExists() {
        if (_context.SID == null)
            throw new KviziramException(Msg.NoSession);

        if (_context.AccountCaller == null && _context.GuestCaller == null)
            throw new KviziramException(Msg.NoAuth);
    }

    public AccountPoco CallerAccountExists() {
        if (_context.AccountCaller == null)
            throw new KviziramException(Msg.NoAuth);
        return _context.AccountCaller;
    }

    public string GetRedisSID() {
        if (_context.SID != null) return _context.SID;
        return string.Empty;
    }

    public bool IsCallerAccountAdmin() {
        if (!CallerAccountExists().isAdmin) 
            throw new KviziramException(Msg.NoAccess);
        else return true;
    }

    public Guest CallerGuestExists() {
        if (_context.GuestCaller == null)
            throw new KviziramException(Msg.NoAuth);
        return _context.GuestCaller;
    }

    public (bool account, bool guest) IsCaller() {
        bool account = (_context.AccountCaller == null) ? false : true;
        bool guest = (_context.GuestCaller == null) ? false : true;
        return (account, guest);
    }

    public bool AlreadyLogged() {
        if (_context.SID != null)
            throw new KviziramException(Msg.AlreadyLoggedIn);
        return false;
    }

    public string CreateInviteCode() {
        return Guid.NewGuid().ToString("n").Substring(0, 6);
    }
    #endregion

    #region Redis keys

    public string RedisKeyPublicMatches = "list:games:public";

    //Korisnici, invite kljuc za korisnika tj lista.
    public string RedisKeyGuest(string key) { return "guest:" + key + ":id"; }
    public string RedisKeyAccount(string key) { return "account:" + key + ":id"; }
    public string RedisKeyInvite(string key) { return "invite:" + key + ":id"; }

    //Igra, kviz id-ime-kategorija-trofej
    public string RedisKeyGame(string key) { return "game:" + key + ":id"; }
    public string RedisKeyChat(string key) { return "chat:" + key + ":id"; }

    //Pitanja od kviza u redis, jer ce konstantno da se igraju pa ih cuvamo u redis
    //Broj igraca za igru koji je odg na pitanje pa refresh za sledece pitanje
    public string RedisKeyQuestions(string key) { return "questions:" + key + ":id"; }
    public string RedisKeyQuestions(Guid? key) { if(key == null) throw new KviziramException(Msg.NoQuiz); else return "questions:" + key + ":id"; }
    public string RedisKeyPlayersAnswered(string key) { return "answered:" + key + ":id"; }

    //Lobby prati broj igraca koji su trenutno u game, scores cuva rezultat i uporedjujemo ako se igrac disconnect/connect-uje
    public string RedisKeyLobby(string key) { return "lobby:" + key + ":id"; }
    public string RedisKeyScores(string key) { return "score:" + key + ":id"; }

    //Cuvaj poslednje game-ove ako se diskonektuje pa mora reconnect
    public string RedisKeyPlayedGames(string? key) { return "played:" + key + ":id"; }
    #endregion

    #region Convert Functions
    public string ListOfGuidsToString (List<Guid> listGuids) {
        return "['" + string.Join("','", listGuids) + "']";
    }

    public string ListOfIntToString (List<int> listInt) {
        return "['" + string.Join("','", listInt) + "']";
    }
    #endregion

    #region JSON Formatting 
    public string SerializeQuestion(QuestionDto question) { return JsonSerializer.Serialize<QuestionDto>(question); }
    public QuestionDto? DeserializeQuestion(string question) { return JsonSerializer.Deserialize<QuestionDto?>(question); }
    
    public string SerializeGameDto(GameDto gameDTO) { return JsonSerializer.Serialize<GameDto>(gameDTO); }
    public GameDto? DeserializeGameDto(string? gameDTO) { if (gameDTO != null) return JsonSerializer.Deserialize<GameDto?>(gameDTO); else return null; }

    public string SerializeMatch(Match match) { return JsonSerializer.Serialize<Match>(match); }
    public Match? DeserializeMatch(string? match) { if (match != null) return JsonSerializer.Deserialize<Match?>(match); else return null; }
    
    public string SerializeAccountPoco(AccountPoco acc) { return JsonSerializer.Serialize<AccountPoco>(acc); }
    public AccountPoco? DeserializeAccountPoco(string? acc) { if (acc != null) return JsonSerializer.Deserialize<AccountPoco?>(acc); else return null; }

    #endregion
}
