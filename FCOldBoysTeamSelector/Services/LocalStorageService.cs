using System.Text.Json;
using FCOldBoysTeamSelector.Models;
using Microsoft.JSInterop;

namespace FCOldBoysTeamSelector.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _js;
    private const string PlayersKey = "fcobs_players";
    private const string MatchesKey  = "fcobs_matches";

    private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

    public LocalStorageService(IJSRuntime js) => _js = js;

    // --- Players ---

    public async Task<List<Player>> GetPlayersAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("storage.load", PlayersKey);
            if (string.IsNullOrEmpty(json)) return new List<Player>();
            var players = JsonSerializer.Deserialize<List<Player>>(json, _opts);
            return players ?? new List<Player>();
        }
        catch { return new List<Player>(); }
    }

    public async Task SavePlayersAsync(List<Player> players)
    {
        var json = JsonSerializer.Serialize(players);
        await _js.InvokeVoidAsync("storage.save", PlayersKey, json);
    }

    // --- Match records ---

    public async Task<List<MatchRecord>> GetMatchRecordsAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("storage.load", MatchesKey);
            if (string.IsNullOrEmpty(json)) return [];
            return JsonSerializer.Deserialize<List<MatchRecord>>(json, _opts) ?? [];
        }
        catch { return []; }
    }

    public async Task SaveMatchRecordsAsync(List<MatchRecord> records)
    {
        var json = JsonSerializer.Serialize(records);
        await _js.InvokeVoidAsync("storage.save", MatchesKey, json);
    }
}