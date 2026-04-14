using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FCOldBoysTeamSelector.Models;

public class Player
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię i nazwisko jest wymagane")]
    [StringLength(100, ErrorMessage = "Nazwa nie może być dłuższa niż 100 znaków")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Parametry zawodnika (1-10)
    [Range(1, 10, ErrorMessage = "Wartość musi być od 1 do 10")]
    [JsonPropertyName("speed")]
    public int Speed { get; set; } = 5;     // Szybkość

    [Range(1, 10, ErrorMessage = "Wartość musi być od 1 do 10")]
    [JsonPropertyName("stamina")]
    public int Stamina { get; set; } = 5;   // Wybieganie

    [Range(1, 10, ErrorMessage = "Wartość musi być od 1 do 10")]
    [JsonPropertyName("defense")]
    public int Defense { get; set; } = 5;   // Obrona

    [Range(1, 10, ErrorMessage = "Wartość musi być od 1 do 10")]
    [JsonPropertyName("attack")]
    public int Attack { get; set; } = 5;    // Atak

    [Range(1, 10, ErrorMessage = "Wartość musi być od 1 do 10")]
    [JsonPropertyName("strength")]
    public int Strength { get; set; } = 5;  // Siła

    // Grupy zawodników 1-4 (zawodnicy z tej samej grupy zawsze razem) - null = brak grupy
    [JsonPropertyName("group_id")]
    public int? GroupId { get; set; }

    // Bramkarz (domyślna flaga; nie jest liczony do siły składu)
    [JsonPropertyName("is_goalkeeper")]
    public bool IsGoalkeeper { get; set; } = false;

    // Narodowość (kod ISO 3166-1 alpha-2, np. "PL", "DE"; pusty = brak)
    [JsonPropertyName("nationality")]
    public string Nationality { get; set; } = string.Empty;

    // Sumaryczny rating
    [JsonIgnore]
    public int TotalRating => Speed + Stamina + Defense + Attack + Strength;

    // Średni rating
    [JsonIgnore]
    public double AverageRating => TotalRating / 5.0;
}
