namespace FCOldBoysTeamSelector.Models;

public class Team
{
    public string Name { get; set; } = string.Empty;
    public List<Player> Players { get; set; } = new();
    
    // Parametry drużyny
    public int TotalSpeed => Players.Sum(p => p.Speed);
    public int TotalStamina => Players.Sum(p => p.Stamina);
    public int TotalDefense => Players.Sum(p => p.Defense);
    public int TotalGraBezPilki => Players.Sum(p => p.GraBezPilki);
    public int TotalGraZPilka => Players.Sum(p => p.GraZPilka);
    public int TotalStrength => Players.Sum(p => p.Strength);
    public int TotalRating => Players.Sum(p => p.TotalRating);
    
    public int Count => Players.Count;
    
    // Średnie parametry drużyny
    public double AvgSpeed => Players.Any() ? TotalSpeed / (double)Players.Count : 0;
    public double AvgStamina => Players.Any() ? TotalStamina / (double)Players.Count : 0;
    public double AvgDefense => Players.Any() ? TotalDefense / (double)Players.Count : 0;
    public double AvgGraBezPilki => Players.Any() ? TotalGraBezPilki / (double)Players.Count : 0;
    public double AvgGraZPilka => Players.Any() ? TotalGraZPilka / (double)Players.Count : 0;
    public double AvgStrength => Players.Any() ? TotalStrength / (double)Players.Count : 0;
    public double AvgRating => Players.Any() ? TotalRating / (double)Players.Count : 0;
}
