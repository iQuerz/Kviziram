public interface IAchievementService
{
    public Task<Achievement> GetAchievementAsync(Guid? uID);
    public Task<List<Achievement>> GetAllAchievementsAsync();
    public Task<Achievement> CreateAchievementAsync(Achievement newAchievementy);
    public Task<Achievement> UpdateAchievementAsync(Achievement updatedAchievement);
    public Task<string> DeleteAchievementAsync(Guid uID);

    public Task<List<AchievedDto>?> GetAchievementScoreboardAsync(Guid acuID);

}
