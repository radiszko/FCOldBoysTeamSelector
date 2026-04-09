using Microsoft.EntityFrameworkCore;
using FCOldBoysTeamSelector.Components;
using FCOldBoysTeamSelector.Data;
using FCOldBoysTeamSelector.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TeamGeneratorService>();

var app = builder.Build();

// Ensure database is created + schema migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    try { db.Database.ExecuteSqlRaw("ALTER TABLE Players ADD COLUMN Nationality TEXT NOT NULL DEFAULT ''"); }
    catch { /* kolumna już istnieje */ }
    try { db.Database.ExecuteSqlRaw("ALTER TABLE Players ADD COLUMN IsGoalkeeper INTEGER NOT NULL DEFAULT 0"); }
    catch { /* kolumna już istnieje */ }
    try
    {
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS MatchRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                GeneratedAt TEXT NOT NULL,
                MatchDay TEXT NOT NULL DEFAULT '',
                MatchTime TEXT NOT NULL DEFAULT '',
                MatchVenue TEXT NOT NULL DEFAULT '',
                Score1 INTEGER NULL,
                Score2 INTEGER NULL
            )");
        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS MatchPlayerEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MatchRecordId INTEGER NOT NULL,
                PlayerId INTEGER NOT NULL,
                PlayerName TEXT NOT NULL DEFAULT '',
                PlayerNationality TEXT NOT NULL DEFAULT '',
                TeamNumber INTEGER NOT NULL,
                FOREIGN KEY (MatchRecordId) REFERENCES MatchRecords(Id) ON DELETE CASCADE
            )");
    }
    catch { /* tabele już istnieją */ }

    // Seed przykładowych zawodników (tylko jeśli baza jest pusta)
    if (!db.Players.Any())
    {
        var seed = new[]
        {
            new { Name="Roberto Nervosso",  Nat="IT", Sp=8, St=7, De=5, At=8, Sr=6 },
            new { Name="Marco Zvodić",       Nat="HR", Sp=7, St=8, De=6, At=7, Sr=7 },
            new { Name="Radko Skodić",       Nat="HR", Sp=6, St=6, De=8, At=5, Sr=8 },
            new { Name="Nico Pantofelić",    Nat="HR", Sp=9, St=7, De=4, At=9, Sr=5 },
            new { Name="Sebastian Hojny",    Nat="CZ", Sp=7, St=9, De=6, At=7, Sr=6 },
            new { Name="Pavlo Glovković",    Nat="HR", Sp=6, St=7, De=7, At=6, Sr=7 },
            new { Name="Igoro Serata",       Nat="IT", Sp=8, St=6, De=6, At=8, Sr=7 },
            new { Name="Zoran Zalević",      Nat="HR", Sp=5, St=6, De=9, At=4, Sr=9 },
            new { Name="Dragan Zastavić",    Nat="HR", Sp=7, St=7, De=7, At=7, Sr=7 },
            new { Name="Davor Dryblović",    Nat="HR", Sp=9, St=8, De=3, At=9, Sr=4 },
            new { Name="Tomo Torpedić",      Nat="HR", Sp=8, St=7, De=5, At=8, Sr=6 },
            new { Name="David Krasnalić",    Nat="HR", Sp=6, St=6, De=7, At=6, Sr=8 },
            new { Name="Tin Elektrić",       Nat="HR", Sp=9, St=9, De=4, At=8, Sr=5 },
            new { Name="Paolo Portiere",     Nat="IT", Sp=5, St=5, De=9, At=3, Sr=8 },
            new { Name="Zeb Macahan",        Nat="US", Sp=7, St=8, De=6, At=7, Sr=8 },
            new { Name="Cvetan Gregorić",    Nat="HR", Sp=6, St=7, De=8, At=5, Sr=8 },
            new { Name="Tacco Luppo",        Nat="IT", Sp=6, St=6, De=6, At=6, Sr=6 },
        };
        foreach (var s in seed)
        {
            db.Players.Add(new FCOldBoysTeamSelector.Models.Player
            {
                Name = s.Name, Nationality = s.Nat,
                Speed = s.Sp, Stamina = s.St, Defense = s.De, Attack = s.At, Strength = s.Sr
            });
        }
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

