using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FCOldBoysTeamSelector.Models;

namespace FCOldBoysTeamSelector.Services;

public class SupabaseService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://mmybmfoynuilwksuluxw.supabase.co";
    private const string ApiKey = "sb_publishable_hUjuii0FiXkLmUOL5qynSw__8ZVWXNW";
    private const string TableName = "players";
    private const string StatsTableName = "player_stats";
    private const string MatchesTableName = "matches";

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SupabaseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
    }

    // ── Players ──

    public async Task<List<Player>> GetPlayersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/rest/v1/{TableName}?select=*");
            response.EnsureSuccessStatusCode();
            var players = await response.Content.ReadFromJsonAsync<List<Player>>(_jsonOpts);
            return players ?? new List<Player>();
        }
        catch (Exception)
        {
            return new List<Player>();
        }
    }

    public async Task<bool> SavePlayersAsync(List<Player> players)
    {
        try
        {
            foreach (var player in players)
            {
                var json = JsonSerializer.Serialize(player, _jsonOpts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/rest/v1/{TableName}")
                {
                    Content = content
                };
                request.Headers.Add("Prefer", "resolution=merge-duplicates");
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // ── Player stats ──

    public async Task<PlayerStats?> GetPlayerStatsAsync(int playerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/rest/v1/{StatsTableName}?player_id=eq.{playerId}&select=*");
            if (!response.IsSuccessStatusCode) return null;
            var stats = await response.Content.ReadFromJsonAsync<List<PlayerStats>>(_jsonOpts);
            return stats?.FirstOrDefault();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> SavePlayerStatsAsync(PlayerStats stats)
    {
        try
        {
            var json = JsonSerializer.Serialize(stats, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/rest/v1/{StatsTableName}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeletePlayerStatsAsync(int playerId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/rest/v1/{StatsTableName}?player_id=eq.{playerId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // ── Matches ──

    public async Task<List<MatchRecord>> GetMatchRecordsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/rest/v1/{MatchesTableName}?select=*&order=generated_at.desc");
            response.EnsureSuccessStatusCode();
            var matches = await response.Content.ReadFromJsonAsync<List<MatchRecord>>(_jsonOpts);
            return matches ?? new List<MatchRecord>();
        }
        catch (Exception)
        {
            return new List<MatchRecord>();
        }
    }

    public async Task<bool> SaveMatchRecordAsync(MatchRecord record)
    {
        try
        {
            var dto = new
            {
                match_day = record.MatchDay,
                match_time = record.MatchTime,
                match_venue = record.MatchVenue,
                score1 = record.Score1,
                score2 = record.Score2,
                winning_team = record.WinningTeam,
                player_entries = record.PlayerEntries,
                generated_at = record.GeneratedAt.ToUniversalTime()
            };
            var json = JsonSerializer.Serialize(dto, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/rest/v1/{MatchesTableName}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateMatchScoreAsync(int matchId, int? score1, int? score2)
    {
        try
        {
            int? winningTeam = (score1.HasValue && score2.HasValue)
                ? (score1 > score2 ? 1 : score1 < score2 ? 2 : 0)
                : null;
            var dto = new { score1, score2, winning_team = winningTeam };
            var json = JsonSerializer.Serialize(dto, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{BaseUrl}/rest/v1/{MatchesTableName}?id=eq.{matchId}")
            {
                Content = content
            };
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteMatchRecordAsync(int matchId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/rest/v1/{MatchesTableName}?id=eq.{matchId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}