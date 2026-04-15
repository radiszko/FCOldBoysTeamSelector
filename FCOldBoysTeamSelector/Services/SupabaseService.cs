using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FCOldBoysTeamSelector.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FCOldBoysTeamSelector.Services;

public class SupabaseServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class SupabaseService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SupabaseService> _logger;
    private readonly SupabaseServiceOptions _options;
    private const string TableName = "players";
    private const string StatsTableName = "player_stats";
    private const string MatchesTableName = "matches";

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SupabaseService(HttpClient httpClient, IOptions<SupabaseServiceOptions> options, ILogger<SupabaseService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;

        if (string.IsNullOrEmpty(_options.BaseUrl) || string.IsNullOrEmpty(_options.ApiKey))
        {
            _logger.LogError("Supabase konfiguracja jest niekompletna. Sprawdź appsettings.json.");
            return;
        }

        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _logger.LogInformation("SupabaseService zainicjowany dla {BaseUrl}", _options.BaseUrl);
    }

    // ── Players ──

    public async Task<List<Player>> GetPlayersAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("GetPlayersAsync: BaseUrl nie jest skonfigurowany");
                return new List<Player>();
            }

            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/rest/v1/{TableName}?select=*");
            response.EnsureSuccessStatusCode();
            var players = await response.Content.ReadFromJsonAsync<List<Player>>(_jsonOpts);
            _logger.LogInformation("Pobrano {Count} zawodników z Supabase", players?.Count ?? 0);
            return players ?? new List<Player>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania zawodników z Supabase");
            return new List<Player>();
        }
    }

    public async Task<bool> SavePlayersAsync(List<Player> players)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("SavePlayersAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            foreach (var player in players)
            {
                var json = JsonSerializer.Serialize(player, _jsonOpts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/rest/v1/{TableName}")
                {
                    Content = content
                };
                request.Headers.Add("Prefer", "resolution=merge-duplicates");
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Nie udało się zapisać zawodnika {PlayerId}: {StatusCode}", player.Id, response.StatusCode);
                    return false;
                }
            }
            _logger.LogInformation("Zapisano {Count} zawodników do Supabase", players.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas zapisu zawodników do Supabase");
            return false;
        }
    }

    public async Task<bool> UpdatePlayerAsync(Player player)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("UpdatePlayerAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            var json = JsonSerializer.Serialize(player, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_options.BaseUrl}/rest/v1/{TableName}?id=eq.{player.Id}")
            {
                Content = content
            };
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Zaktualizowano zawodnika {PlayerId}", player.Id);
            else
                _logger.LogWarning("Nie udało się zaktualizować zawodnika {PlayerId}: {StatusCode}", player.Id, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji zawodnika {PlayerId}", player.Id);
            return false;
        }
    }

    public async Task<bool> InsertPlayerAsync(Player player)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("InsertPlayerAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            // Don't send id — let the DB generate it via SERIAL
            var dto = new
            {
                name = player.Name,
                nationality = player.Nationality,
                group_id = player.GroupId,
                is_goalkeeper = player.IsGoalkeeper,
                speed = player.Speed,
                stamina = player.Stamina,
                defense = player.Defense,
                gra_bez_pilki = player.GraBezPilki,
                gra_z_pilka = player.GraZPilka,
                strength = player.Strength
            };
            var json = JsonSerializer.Serialize(dto, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_options.BaseUrl}/rest/v1/{TableName}", content);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Dodano nowego zawodnika {PlayerName}", player.Name);
            else
                _logger.LogWarning("Nie udało się dodać zawodnika {PlayerName}: {StatusCode}", player.Name, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dodawania zawodnika {PlayerName}", player.Name);
            return false;
        }
    }

    public async Task<bool> DeletePlayerAsync(int playerId)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("DeletePlayerAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            var response = await _httpClient.DeleteAsync($"{_options.BaseUrl}/rest/v1/{TableName}?id=eq.{playerId}");

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Usunięto zawodnika {PlayerId}", playerId);
            else
                _logger.LogWarning("Nie udało się usunąć zawodnika {PlayerId}: {StatusCode}", playerId, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania zawodnika {PlayerId}", playerId);
            return false;
        }
    }

    // ── Player stats ──

    public async Task<PlayerStats?> GetPlayerStatsAsync(int playerId)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("GetPlayerStatsAsync: BaseUrl nie jest skonfigurowany");
                return null;
            }

            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/rest/v1/{StatsTableName}?player_id=eq.{playerId}&select=*");
            if (!response.IsSuccessStatusCode) return null;
            var stats = await response.Content.ReadFromJsonAsync<List<PlayerStats>>(_jsonOpts);
            return stats?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania statystyk zawodnika {PlayerId}", playerId);
            return null;
        }
    }

    public async Task<bool> SavePlayerStatsAsync(PlayerStats stats)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("SavePlayerStatsAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            var json = JsonSerializer.Serialize(stats, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_options.BaseUrl}/rest/v1/{StatsTableName}", content);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Zapisano statystyki zawodnika {PlayerId}", stats.PlayerId);
            else
                _logger.LogWarning("Nie udało się zapisać statystyk zawodnika {PlayerId}: {StatusCode}", stats.PlayerId, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas zapisu statystyk zawodnika {PlayerId}", stats.PlayerId);
            return false;
        }
    }

    public async Task<bool> DeletePlayerStatsAsync(int playerId)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("DeletePlayerStatsAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            var response = await _httpClient.DeleteAsync($"{_options.BaseUrl}/rest/v1/{StatsTableName}?player_id=eq.{playerId}");

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Usunięto statystyki zawodnika {PlayerId}", playerId);
            else
                _logger.LogWarning("Nie udało się usunąć statystyk zawodnika {PlayerId}: {StatusCode}", playerId, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania statystyk zawodnika {PlayerId}", playerId);
            return false;
        }
    }

    // ── Matches ──

    public async Task<List<MatchRecord>> GetMatchRecordsAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("GetMatchRecordsAsync: BaseUrl nie jest skonfigurowany");
                return new List<MatchRecord>();
            }

            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/rest/v1/{MatchesTableName}?select=*&order=generated_at.desc");
            response.EnsureSuccessStatusCode();
            var matches = await response.Content.ReadFromJsonAsync<List<MatchRecord>>(_jsonOpts);
            _logger.LogInformation("Pobrano {Count} rekordów meczowych z Supabase", matches?.Count ?? 0);
            return matches ?? new List<MatchRecord>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania rekordów meczowych z Supabase");
            return new List<MatchRecord>();
        }
    }

    public async Task<bool> SaveMatchRecordAsync(MatchRecord record)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("SaveMatchRecordAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

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
            var response = await _httpClient.PostAsync($"{_options.BaseUrl}/rest/v1/{MatchesTableName}", content);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Zapisano rekord meczu dla dnia {MatchDay}", record.MatchDay);
            else
                _logger.LogWarning("Nie udało się zapisać rekordu meczu: {StatusCode}", response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas zapisu rekordu meczowego");
            return false;
        }
    }

    public async Task<bool> UpdateMatchScoreAsync(int matchId, int? score1, int? score2)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("UpdateMatchScoreAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            int? winningTeam = (score1.HasValue && score2.HasValue)
                ? (score1 > score2 ? 1 : score1 < score2 ? 2 : 0)
                : null;
            var dto = new { score1, score2, winning_team = winningTeam };
            var json = JsonSerializer.Serialize(dto, _jsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_options.BaseUrl}/rest/v1/{MatchesTableName}?id=eq.{matchId}")
            {
                Content = content
            };
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Zaktualizowano wynik meczu {MatchId}: {Score1}-{Score2}", matchId, score1, score2);
            else
                _logger.LogWarning("Nie udało się zaktualizować wyniku meczu {MatchId}: {StatusCode}", matchId, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji wyniku meczu {MatchId}", matchId);
            return false;
        }
    }

    public async Task<bool> DeleteMatchRecordAsync(int matchId)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                _logger.LogError("DeleteMatchRecordAsync: BaseUrl nie jest skonfigurowany");
                return false;
            }

            var response = await _httpClient.DeleteAsync($"{_options.BaseUrl}/rest/v1/{MatchesTableName}?id=eq.{matchId}");

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Usunięto rekord meczu {MatchId}", matchId);
            else
                _logger.LogWarning("Nie udało się usunąć rekordu meczu {MatchId}: {StatusCode}", matchId, response.StatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania rekordu meczu {MatchId}", matchId);
            return false;
        }
    }

    public void Dispose()
    {
        _logger.LogInformation("SupabaseService zamknięty");
        _httpClient.Dispose();
    }
}