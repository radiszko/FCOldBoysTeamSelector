-- Tabela zawodników
CREATE TABLE IF NOT EXISTS players (
  id SERIAL PRIMARY KEY,
  name TEXT NOT NULL,
  nationality TEXT DEFAULT '',
  group_id INTEGER,
  is_goalkeeper BOOLEAN NOT NULL DEFAULT false,
  speed INTEGER NOT NULL DEFAULT 5,
  stamina INTEGER NOT NULL DEFAULT 5,
  defense INTEGER NOT NULL DEFAULT 5,
  gra_bez_pilki INTEGER NOT NULL DEFAULT 5,
  gra_z_pilka INTEGER NOT NULL DEFAULT 5,
  strength INTEGER NOT NULL DEFAULT 5
);

-- RLS: pozwól anonowemu kluczowi na pełny dostęp do players
ALTER TABLE players ENABLE ROW LEVEL SECURITY;
CREATE POLICY "Allow anon full access on players" ON players FOR ALL USING (true) WITH CHECK (true);

-- Seed data: Zawodnicy
INSERT INTO players (name, nationality, group_id, is_goalkeeper, speed, stamina, defense, gra_bez_pilki, gra_z_pilka, strength) VALUES
('Roberto Nervosso', 'IT', 1, false, 7, 9, 9, 6, 7, 9),
('Marco Zvodić', 'HR', 1, false, 5, 6, 9, 8, 9, 9),
('Radko Skodić', 'HR', 1, false, 7, 7, 9, 6, 5, 10),
('Nico Pantofelić', 'HR', 1, false, 8, 8, 9, 6, 8, 6),
('Sebastian Hojny', 'CZ', 1, false, 3, 3, 6, 7, 9, 9),
('Pavlo Glovković', 'HR', 1, false, 7, 6, 7, 10, 7, 10),
('Igoro Serata', 'IT', 2, false, 5, 5, 7, 7, 8, 9),
('Zoran Zalević', 'HR', 2, false, 3, 5, 9, 6, 5, 10),
('Dragan Zastavić', 'HR', 2, false, 8, 8, 10, 5, 7, 10),
('Davor Dryblović', 'HR', 2, false, 9, 6, 5, 9, 10, 8),
('Tomo Torpedić', 'HR', 2, false, 10, 9, 6, 7, 8, 7),
('David Krasnalić', 'HR', 2, false, 10, 9, 6, 9, 7, 5),
('Tin Elektrić', 'HR', 3, false, 8, 10, 8, 7, 6, 7),
('Paolo Portiere', 'IT', 4, true, 5, 5, 5, 5, 5, 5),
('Zeb Macahan', 'US', 3, false, 8, 8, 8, 7, 4, 7),
('Cvetan Gregorić', 'HR', 3, false, 6, 7, 9, 5, 6, 9),
('Tacco Luppo', 'IT', 4, false, 9, 7, 6, 8, 8, 8)
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