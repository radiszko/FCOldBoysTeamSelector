-- Seed data: Zawodnicy (bez hardcoded w kodzie)
INSERT INTO players (name, nationality, group_id, is_goalkeeper, speed, stamina, defense, attack, strength) VALUES
('Roberto Nervosso', 'IT', 1, false, 8, 7, 5, 8, 6),
('Marco Zvodić', 'HR', 1, false, 7, 8, 6, 7, 7),
('Radko Skodić', 'HR', 1, false, 6, 6, 8, 5, 8),
('Nico Pantofelić', 'HR', 1, false, 9, 7, 4, 9, 5),
('Sebastian Hojny', 'CZ', 1, false, 7, 9, 6, 7, 6),
('Pavlo Glovković', 'HR', 1, false, 6, 7, 7, 6, 7),
('Igoro Serata', 'IT', 2, false, 8, 6, 6, 8, 7),
('Zoran Zalević', 'HR', 2, false, 5, 6, 9, 4, 9),
('Dragan Zastavić', 'HR', 2, false, 7, 7, 7, 7, 7),
('Davor Dryblović', 'HR', 2, false, 9, 8, 3, 9, 4),
('Tomo Torpedić', 'HR', 2, false, 8, 7, 5, 8, 6),
('David Krasnalić', 'HR', 2, false, 6, 6, 7, 6, 8),
('Tin Elektrić', 'HR', 3, false, 9, 9, 4, 8, 5),
('Paolo Portiere', 'IT', 4, true, 5, 5, 9, 3, 8),
('Zeb Macahan', 'US', 3, false, 7, 8, 6, 7, 8),
('Cvetan Gregorić', 'HR', 3, false, 6, 7, 8, 5, 8),
('Tacco Luppo', 'IT', 4, false, 6, 6, 6, 6, 6)
ON CONFLICT (id) DO NOTHING;

-- Tabela do przechowywania statystyk zawodników
CREATE TABLE IF NOT EXISTS player_stats (
  id SERIAL PRIMARY KEY,
  player_id INTEGER NOT NULL REFERENCES players(id) ON DELETE CASCADE,
  matches INTEGER NOT NULL DEFAULT 0,
  wins INTEGER NOT NULL DEFAULT 0,
  draws INTEGER NOT NULL DEFAULT 0,
  losses INTEGER NOT NULL DEFAULT 0,
  appearances INTEGER NOT NULL DEFAULT 0,
  total_score INTEGER NOT NULL DEFAULT 0,
  total_conceded INTEGER NOT NULL DEFAULT 0,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT timezone('utc'::text, now()),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT timezone('utc'::text, now())
);

-- Index dla lepszych wyników
CREATE INDEX IF NOT EXISTS idx_player_stats_player_id ON player_stats(player_id);