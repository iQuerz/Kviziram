public static class Msg
{
    #region Generic messages 
    public const string Unknown = "Oops! An unknown error happened on the server";
    public const string NoPermission = "You don't have permission";
    public const string NoObject = "The object doesn't exist";
    public const string Deleted = " has been deleted";
    public const string NoAccess = "You don't have access";
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

    #endregion


}
