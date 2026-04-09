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
            if (string.IsNullOrEmpty(json)) return GetDefaultPlayers();
            var players = JsonSerializer.Deserialize<List<Player>>(json, _opts);
            if (players == null || players.Count == 0) return GetDefaultPlayers();
            return players;
        }
        catch { return GetDefaultPlayers(); }
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

    // --- Default seed data ---

    private static List<Player> GetDefaultPlayers()
    {
        var seed = new (string Name, string Nat, int Sp, int St, int De, int At, int Sr)[]
        {
            ("Roberto Nervosso", "IT", 8, 7, 5, 8, 6),
            ("Marco Zvodić",     "HR", 7, 8, 6, 7, 7),
            ("Radko Skodić",     "HR", 6, 6, 8, 5, 8),
            ("Nico Pantofelić",  "HR", 9, 7, 4, 9, 5),
            ("Sebastian Hojny",  "CZ", 7, 9, 6, 7, 6),
            ("Pavlo Glovković",  "HR", 6, 7, 7, 6, 7),
            ("Igoro Serata",     "IT", 8, 6, 6, 8, 7),
            ("Zoran Zalević",    "HR", 5, 6, 9, 4, 9),
            ("Dragan Zastavić",  "HR", 7, 7, 7, 7, 7),
            ("Davor Dryblović",  "HR", 9, 8, 3, 9, 4),
            ("Tomo Torpedić",    "HR", 8, 7, 5, 8, 6),
            ("David Krasnalić",  "HR", 6, 6, 7, 6, 8),
            ("Tin Elektrić",     "HR", 9, 9, 4, 8, 5),
            ("Paolo Portiere",   "IT", 5, 5, 9, 3, 8),
            ("Zeb Macahan",      "US", 7, 8, 6, 7, 8),
            ("Cvetan Gregorić",  "HR", 6, 7, 8, 5, 8),
            ("Tacco Luppo",      "IT", 6, 6, 6, 6, 6),
        };
        int id = 1;
        return seed.Select(s => new Player
        {
            Id          = id++,
            Name        = s.Name,
            Nationality = s.Nat,
            Speed       = s.Sp,
            Stamina     = s.St,
            Defense     = s.De,
            Attack      = s.At,
            Strength    = s.Sr
        }).ToList();
    }
}
