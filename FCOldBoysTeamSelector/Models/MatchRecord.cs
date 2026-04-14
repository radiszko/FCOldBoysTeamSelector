using System.Text.Json.Serialization;

namespace FCOldBoysTeamSelector.Models;

public class MatchRecord
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("generated_at")]
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    [JsonPropertyName("match_day")]
    public string MatchDay { get; set; } = "";

    [JsonPropertyName("match_time")]
    public string MatchTime { get; set; } = "";

    [JsonPropertyName("match_venue")]
    public string MatchVenue { get; set; } = "";

    [JsonPropertyName("score1")]
    public int? Score1 { get; set; }

    [JsonPropertyName("score2")]
    public int? Score2 { get; set; }

    [JsonPropertyName("player_entries")]
    public List<MatchPlayerEntry> PlayerEntries { get; set; } = new();

    [JsonIgnore]
    public string ResultDisplay => (Score1.HasValue && Score2.HasValue)
        ? $"{Score1}:{Score2}"
        : "—";

    [JsonPropertyName("winning_team")]
    public int? WinningTeam => (Score1.HasValue && Score2.HasValue)
        ? (Score1 > Score2 ? 1 : Score1 < Score2 ? 2 : 0)
        : null;
}

public class MatchPlayerEntry
{
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = "";

    [JsonPropertyName("player_nationality")]
    public string PlayerNationality { get; set; } = "";

    [JsonPropertyName("team_number")]
    public int TeamNumber { get; set; } // 1 = Team 1, 2 = Team 2, 0 = rezerwa
}
