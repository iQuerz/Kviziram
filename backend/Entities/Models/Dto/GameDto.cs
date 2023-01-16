public class GameDto
{
    public DateTime Created { get; set; }

    public string InviteCode { get; set; } = string.Empty;
 
    public string HostName { get; set; } = string.Empty;

    public string QuizName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string TrophyName { get; set; } = string.Empty;

    public GameDto(Match game) {
        this.Created = (game.Created != null) ? game.Created.Value : DateTime.Now;
        this.InviteCode = (game.InviteCode != null) ? game.InviteCode : string.Empty;
    }

    // string RedisKeyGame = _util.RedisKeyGame(game.InviteCode);
    // GameDto gameDTO = new GameDto(game);
    // gameDTO.HostName = _util.CallerAccountExists().Username;
    // gameDTO.QuizName = (tempQuiz.Name != null) ? tempQuiz.Name : string.Empty;
    // gameDTO.CategoryName = (tempQuiz.Category != null && tempQuiz.Category.Name != null) ? tempQuiz.Category.Name : string.Empty;
    // gameDTO.QuizName = (tempQuiz.Achievement != null && tempQuiz.Achievement.Name != null) ? tempQuiz.Achievement.Name : string.Empty;


}