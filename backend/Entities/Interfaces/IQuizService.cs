public interface IQuizService
{
    public Task<Quiz> CreateQuizAsync(Quiz newQuiz);
    public Task<Quiz?> GetQuizAsync(Guid uID);
    public Task<List<QuizPoco>?> SearchQuizzesAsync(QuizQuery quizQuery);
}
