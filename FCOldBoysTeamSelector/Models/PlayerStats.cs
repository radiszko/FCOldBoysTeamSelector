namespace FCOldBoysTeamSelector.Models;

public class PlayerStats
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int Matches { get; set; }     // Liczba meczów z wynikiem
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int Appearances { get; set; } // Liczba występów (wszystkie mecze)
    public int TotalScore { get; set; }  // Suma bramek strzelonych
    public int TotalConceded { get; set; } // Suma bramek straconych
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}