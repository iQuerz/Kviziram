public interface IQuizService
{
    public Task<Quiz?> GetQuizAsync(Guid quID);
    public Task<Quiz?> CreateQuizAsync(Quiz newQuiz);
    public Task<Quiz?> UpdateQuizAsync(Quiz updatedQuiz);
    public Task<string> DeleteQuizAsync(Guid quID);
    public Task<List<AccountQuizRatingDto>?> GetQuizRatingsAsync(Guid quID);
    public Task<List<QuizDto>?> SearchQuizzesAsync(QuizQuery quizQuery);
    public Task<string> ConnectQuizAchievementAsync(Guid quID, Guid? acuID);
    public Task<string> DisconnectQuizAchievementAsync(Guid quID, Guid? acuID);
}
