public interface IQuizService
{
    public Task<Quiz> CreateQuizAsync(Quiz newQuiz);
    public Task<Quiz?> GetQuizAsync(Guid quID);
    public Task<List<AccountQuizRatingDto>?> GetQuizRatingsAsync(Guid quID);
    public Task<List<QuizDto>?> SearchQuizzesAsync(QuizQuery quizQuery);
}
