public interface IQuizService
{
    public Task<Quiz> CreateQuizAsync(Quiz newQuiz);
}
