using FCOldBoysTeamSelector.Models;

namespace FCOldBoysTeamSelector.Services;

public class TeamGeneratorService
{
    /// <summary>
    /// Generuje dwie drużyny o jak najbardziej wyrównanych parametrach.
    /// Zawodnicy z tym samym GroupId trafiają zawsze do tej samej drużyny.
    /// </summary>
    public (Team Team1, Team Team2) GenerateTeams(List<Player> selectedPlayers, int? seed = null)
    {
        if (selectedPlayers.Count < 2)
            throw new InvalidOperationException("Potrzeba co najmniej 2 zawodników.");

        // Pogrupuj zawodników w jednostki (grupy lub pojedynczy gracze)
        var units = BuildUnits(selectedPlayers);

        int team1Target = selectedPlayers.Count / 2;
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();

        List<Player>? bestTeam1 = null;
        double bestScore = double.MaxValue;

        // Próbuj 8000 losowych przydziałów
        for (int iter = 0; iter < 8000; iter++)
        {
            var shuffled = units.OrderBy(_ => rng.Next()).ToList();
            var t1 = new List<Player>();
            var t2 = new List<Player>();

            foreach (var unit in shuffled)
            {
                if (t1.Count + unit.Count <= team1Target)
                    t1.AddRange(unit);
                else
                    t2.AddRange(unit);
            }

            // Sprawdź czy rozmiary są akceptowalne
            if (Math.Abs(t1.Count - team1Target) > 1)
                continue;

            double score = ComputeScore(t1, t2);
            if (score < bestScore)
            {
                bestScore = score;
                bestTeam1 = new List<Player>(t1);
            }

            // Idealne wyrównanie – nie szukaj dalej
            if (bestScore == 0)
                break;
        }

        // Fallback gdy grup nie udało się rozdzielić poprawnie
        if (bestTeam1 == null || bestTeam1.Count == 0)
        {
            bestTeam1 = selectedPlayers.Take(team1Target).ToList();
        }

        var team2Players = selectedPlayers.Except(bestTeam1).ToList();

        return (
            new Team { Name = "Drużyna 1", Players = bestTeam1 },
            new Team { Name = "Drużyna 2", Players = team2Players }
        );
    }

    private static List<List<Player>> BuildUnits(List<Player> players)
    {
        var grouped = players
            .Where(p => p.GroupId.HasValue)
            .GroupBy(p => p.GroupId!.Value)
            .Select(g => g.ToList());

        var individuals = players
            .Where(p => !p.GroupId.HasValue)
            .Select(p => new List<Player> { p });

        return grouped.Concat(individuals).ToList();
    }

    // Wagi balansowania: szybkość 0.30, wybieganie 0.30, reszta (obrona/atak/siła) po 0.40/3 ≈ 0.1333
    private const double WSpeed   = 0.30;
    private const double WStamina = 0.30;
    private const double WOther   = 0.40 / 3.0;

    private static double ComputeScore(List<Player> t1, List<Player> t2)
    {
        return WSpeed   * Math.Abs(t1.Sum(p => p.Speed)    - t2.Sum(p => p.Speed))
             + WStamina * Math.Abs(t1.Sum(p => p.Stamina)  - t2.Sum(p => p.Stamina))
             + WOther   * Math.Abs(t1.Sum(p => p.Defense)  - t2.Sum(p => p.Defense))
             + WOther   * Math.Abs(t1.Sum(p => p.Attack)   - t2.Sum(p => p.Attack))
             + WOther   * Math.Abs(t1.Sum(p => p.Strength) - t2.Sum(p => p.Strength));
    }

    public double GetBalanceScore(Team t1, Team t2, HashSet<int>? gkIds = null)
    {
        var p1 = gkIds != null ? t1.Players.Where(p => !gkIds.Contains(p.Id)).ToList() : t1.Players;
        var p2 = gkIds != null ? t2.Players.Where(p => !gkIds.Contains(p.Id)).ToList() : t2.Players;
        return ComputeScore(p1, p2);
    }

    /// <summary>
    /// Wariant z przypiętymi zawodnikami: pinned1 trafia zawsze do T1, pinned2 do T2.
    /// Wolni zawodnicy są optymalnie rozdysponowani.
    /// </summary>
    public (Team Team1, Team Team2) GenerateTeamsWithPins(
        List<Player> freePlayers,
        List<Player> pinnedToTeam1,
        List<Player> pinnedToTeam2,
        int? seed = null)
    {
        if (freePlayers.Count == 0)
        {
            return (
                new Team { Name = "Drużyna 1", Players = new List<Player>(pinnedToTeam1) },
                new Team { Name = "Drużyna 2", Players = new List<Player>(pinnedToTeam2) }
            );
        }

        int total = freePlayers.Count + pinnedToTeam1.Count + pinnedToTeam2.Count;
        int t1FreeTarget = Math.Max(0, Math.Min(freePlayers.Count, total / 2 - pinnedToTeam1.Count));

        var units = BuildUnits(freePlayers);
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        List<Player>? bestFreeT1 = null;
        double bestScore = double.MaxValue;

        for (int iter = 0; iter < 8000; iter++)
        {
            var shuffled = units.OrderBy(_ => rng.Next()).ToList();
            var freeT1 = new List<Player>();
            var freeT2 = new List<Player>();

            foreach (var unit in shuffled)
            {
                if (freeT1.Count + unit.Count <= t1FreeTarget)
                    freeT1.AddRange(unit);
                else
                    freeT2.AddRange(unit);
            }

            if (Math.Abs(freeT1.Count - t1FreeTarget) > 1) continue;

            var totalT1 = pinnedToTeam1.Concat(freeT1).ToList();
            var totalT2 = pinnedToTeam2.Concat(freeT2).ToList();
            double score = ComputeScore(totalT1, totalT2);

            if (score < bestScore)
            {
                bestScore = score;
                bestFreeT1 = new List<Player>(freeT1);
            }
            if (bestScore == 0) break;
        }

        bestFreeT1 ??= freePlayers.Take(t1FreeTarget).ToList();
        var bestFreeT2 = freePlayers.Except(bestFreeT1).ToList();

        return (
            new Team { Name = "Drużyna 1", Players = pinnedToTeam1.Concat(bestFreeT1).ToList() },
            new Team { Name = "Drużyna 2", Players = pinnedToTeam2.Concat(bestFreeT2).ToList() }
        );
    }
}
