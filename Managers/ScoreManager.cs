using System;
using System.IO;
using System.Threading.Tasks;

namespace Arcade2D.Managers;

public class ScoreManager
{
    // Я змінив назву файлу збереження, щоб старий рекорд (200) скинувся, 
    // і гра почала записувати історію з чистого аркуша
    private readonly string _lastScoreFilename = "lastscore.txt"; 
    
    public int Score { get; private set; }
    public int LastScore { get; private set; }

    public void Reset() => Score = 0;

    public void AddScore(int points) => Score += points;

    public void LoadHighScore()
    {
        if (File.Exists(_lastScoreFilename))
        {
            try
            {
                string content = File.ReadAllText(_lastScoreFilename);
                if (int.TryParse(content, out int loadedScore))
                {
                    LastScore = loadedScore;
                }
            }
            catch (Exception) { }
        }
    }

    public async Task SaveHighScoreAsync()
    {
        // РЕФАКТОРИНГ: Ми ПРИБРАЛИ перевірку if (Score > LastScore).
        // Тепер, щойно гра закінчується (перемога або програш), 
        // ваш поточний рахунок гарантовано записується як "Останній рекорд".
        
        LastScore = Score;
        try
        {
            await File.WriteAllTextAsync(_lastScoreFilename, LastScore.ToString());
        }
        catch (Exception) { }
    }
}