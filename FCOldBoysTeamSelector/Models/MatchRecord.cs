namespace FCOldBoysTeamSelector.Models;

public class MatchRecord
{
    public int Id { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public string MatchDay { get; set; } = "";
    public string MatchTime { get; set; } = "";
    public string MatchVenue { get; set; } = "";
    public int? Score1 { get; set; }
    public int? Score2 { get; set; }

    public List<MatchPlayerEntry> PlayerEntries { get; set; } = new();

    public string ResultDisplay => (Score1.HasValue && Score2.HasValue)
        ? $"{Score1}:{Score2}"
        : "—";

    public int? WinningTeam => (Score1.HasValue && Score2.HasValue)
        ? (Score1 > Score2 ? 1 : Score1 < Score2 ? 2 : 0)
        : null;
}

public class MatchPlayerEntry
{
    public int Id { get; set; }
    public int MatchRecordId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public MatchRecord MatchRecord { get; set; } = null!;
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
    public string PlayerNationality { get; set; } = "";
    public int TeamNumber { get; set; } // 1 = Team 1, 2 = Team 2, 0 = rezerwa
}
