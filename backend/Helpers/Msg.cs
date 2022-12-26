public static class Msg
{
    #region Generic messages 
    public const string Unknown = "Oops! An unknown error happened on the server";
    public const string NoPermission = "You don't have permission";
    public const string NoObject = "The object doesn't exist";
    public const string Deleted = " has been deleted";
    public const string NoAccess = "You don't have access";
    public const string NoAnything = "Could not find anything";
    #endregion

    #region Login/Register messages
    public const string NoAuth = "Authorization failed";
    public const string NoSession = "Session doesn't exist";
    public const string BadEmail = "Account with this email doesn't exist";
    public const string UsedEmail = "The email you enter is already registered";
    public const string BadPassword = "Bad password entered";
    public const string AlreadyLoggedIn = "You're already logged in";
    #endregion 

    #region Account messages
    public const string NoAccount = "Account doesn't exist";
    public const string NoGuest = "Your guest uid is wrong";
    public const string NoFriends = "You have no friends lmao";
    public const string RequestSent = "Friendship request has been sent";
    public const string RequestAnswer = "Relationship status set to: ";
    public const string RequestFailed = "Request failed";
    public const string RelationshipRemove = "The relationship has been removed";
    public const string WrongRelationshipState = "What fuckin' relationship did you ask for?";
    #endregion


}
