using System;
using System.IO;
using System.Threading.Tasks;

namespace Arcade2D.Managers;

public class ScoreManager
{
    private readonly string _lastScoreFilename = "lastscore.txt"; 
    
    public int Score { get; private set; }
    
    public int LastScore { get; private set; } 

    public void Reset() => Score = 0;

    public void AddScore(int points) => Score += points;

    public void LoadLastScore()
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

    public async Task SaveLastScoreAsync()     // async!!!
    {
        try
        {
            // файл асинхронно, не блокуючи гру
            await File.WriteAllTextAsync(_lastScoreFilename, Score.ToString());
        }
        catch (Exception) { }
    }

    public void UpdateLastScoreInMemory()     
    {
        LastScore = Score;
    }
}