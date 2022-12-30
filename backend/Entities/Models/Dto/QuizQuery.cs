public class QuizQuery
{
    public Guid? CreatorID { get; set; }
    public Guid? AchievementID { get; set; }
    public Guid? CategoryID { get; set; }
    public bool GetQuestions { get; set; } = false;
    public bool OrderAsc { get; set; } = true;
    
}