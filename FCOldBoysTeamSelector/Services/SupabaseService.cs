using System.Net.Http.Json;
using System.Text;
using FCOldBoysTeamSelector.Models;

namespace FCOldBoysTeamSelector.Services;

public class SupabaseService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://mmybmfoynuilwksuluxw.supabase.co";
    private const string ApiKey = "sb_publishable_hUjuii0FiXkLmUOL5qynSw__8ZVWXNW";
    private const string TableName = "players";
    private const string StatsTableName = "player_stats";

    public SupabaseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
    }

    public async Task<List<Player>> GetPlayersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/rest/v1/{TableName}?select=*");
            response.EnsureSuccessStatusCode();
            var players = await response.Content.ReadFromJsonAsync<List<Player>>();
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
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/rest/v1/{TableName}", player);
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

    // Player stats

    public async Task<PlayerStats?> GetPlayerStatsAsync(int playerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/rest/v1/{StatsTableName}?player_id=eq.{playerId}&select=*");
            if (!response.IsSuccessStatusCode) return null;
            var stats = await response.Content.ReadFromJsonAsync<List<PlayerStats>>();
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
            // Upsert - update or insert
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/rest/v1/{StatsTableName}", stats);
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
}