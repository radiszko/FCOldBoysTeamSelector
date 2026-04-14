using System.Text.Json.Serialization;

namespace FCOldBoysTeamSelector.Models;

public class PlayerStats
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    [JsonPropertyName("matches")]
    public int Matches { get; set; }     // Liczba meczów z wynikiem

    [JsonPropertyName("wins")]
    public int Wins { get; set; }

    [JsonPropertyName("draws")]
    public int Draws { get; set; }

    [JsonPropertyName("losses")]
    public int Losses { get; set; }

    [JsonPropertyName("appearances")]
    public int Appearances { get; set; } // Liczba występów (wszystkie mecze)

    [JsonPropertyName("total_score")]
    public int TotalScore { get; set; }  // Suma bramek strzelonych

    [JsonPropertyName("total_conceded")]
    public int TotalConceded { get; set; } // Suma bramek straconych

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}