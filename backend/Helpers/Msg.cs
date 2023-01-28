public static class Msg
{
    #region Generic messages 
    public const string Unknown = "Oops! An unknown error happened on the server";
    public const string NoPermission = "You don't have permission";
    public const string NoObject = "The object doesn't exist";
    public const string Deleted = " has been deleted";
    public const string NoAccess = "You don't have access";
    public const string NoAnything = "Could not find anything";
    public const string OperationDone = "Operation done";
    public const string Empty = "Null as parameter wtf";
    public const string IndexOutOfRange = "Index out of range";
    #endregion

    #region Login/Register messages
    public const string NoAuth = "Authorization failed";
    public const string NoSession = "Session doesn't exist";
    public const string BadEmail = "Account with this email doesn't exist";
    public const string UsedEmail = "The email you enter is already registered";
    public const string BadPassword = "Bad password entered";
    public const string AlreadyLoggedIn = "You're already logged in";
    public const string LoggedOut = "You have been logged out";
    #endregion 

    #region Account messages
    public const string NoAccount = "Account doesn't exist";
    public const string NoGuest = "Your guest uid is wrong";
    public const string NoFriends = "You have no friends lmao";
    public const string RequestSent = "Friendship request has been sent";
    public const string RequestAnswer = "Relationship status set to: ";
    public const string AccountBlocked = "Account has been blocked";
    public const string RequestFailed = "Request failed";
    public const string RelationshipRemove = "The relationship has been removed";
    public const string WrongRelationshipState = "What fuckin' relationship did you ask for?";
    #endregion

    #region Category messages
    public const string NoCategory = "Category not found";
    public const string PreferredCategoriesSet = "Preferred categories updated";
    public const string PreferredCategoryRemove = "Preferred categories remove";
    #endregion

    #region Achievement messages
    public const string NoAchievement = "Achievement not found";
    public const string ConnectedQuizAchievement = "Connect has been established";
    public const string DisconnectedQuizAchievement = "Achievement was removed from the quiz";
    public const string AchievementSetUpdated = "Achievement given";
    #endregion

    #region Quiz messages
    public const string QuizNoCategory = "Category for this quiz has not been set";
    public const string NoQuiz = "Quiz not found";
    public const string NoQuestions = "You didn't enter any questions for your quiz";
    #endregion

    #region Ad messages
    public const string NoAd = "Category not found";
    public const string AdBlocked = "The ad has been blocked";
    #endregion

    #region Match messages
    public const string NoMatch = "Match not found";
    public const string SavedMatch = "Match has been saved";
    #endregion

    #region Game messages
    public const string NoGame = "Game not found";
    public const string ChatWelcome = "Welcome to the chat boiii";
    public const string NoStartGame = "Can't start the game";
    public const string NoLobby = "Lobby is empty?";
    public const string GameStarted = "The game has started";
    public const string Invited = "Invitation sent";
    public const string InviteOpened = "Invite has been opened";
    #endregion
    


}
