-- Tabela do przechowywania historii meczów
CREATE TABLE IF NOT EXISTS matches (
  id SERIAL PRIMARY KEY,
  match_day TEXT NOT NULL,
  match_time TEXT NOT NULL,
  match_venue TEXT DEFAULT '',
  score1 INTEGER,
  score2 INTEGER,
  winning_team INTEGER,
  player_entries JSONB NOT NULL,
  generated_at TIMESTAMP WITH TIME ZONE DEFAULT timezone('utc'::text, now())
);

-- Tabela do przechowywania statystyk zawodników (jeśli jeszcze nie istnieje)
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

CREATE INDEX IF NOT EXISTS idx_player_stats_player_id ON player_stats(player_id);

-- RLS: pozwól anonowemu kluczowi na pełny dostęp
ALTER TABLE matches ENABLE ROW LEVEL SECURITY;
CREATE POLICY "Allow anon full access on matches" ON matches FOR ALL USING (true) WITH CHECK (true);

-- Function do usuwania meczów starszych niż określony dzień tygodnia
CREATE OR REPLACE FUNCTION cleanup_old_matches(target_day_of_week TEXT)
RETURNS VOID AS $$
BEGIN
  -- Usuń mecz, który jest późniejszy niż dany dzień tygodnia od dziś
  DELETE FROM matches
  WHERE
    -- Pobierz dzisiejszy dzień tygodnia
    EXTRACT(DOW FROM generated_at) + 1 = EXTRACT(DOW FROM NOW())::INTEGER + 1
    OR
    -- Pobierz datę następnego dnia tygodnia
    EXTRACT(DOW FROM generated_at + INTERVAL '1 day') + 1 = EXTRACT(DOW FROM NOW())::INTEGER + 1;
END;
$$ LANGUAGE plpgsql;

-- Function do aktualizacji statystyk zawodników po zapisaniu meczu
CREATE OR REPLACE FUNCTION update_player_stats()
RETURNS TRIGGER AS $$
DECLARE
  entry JSONB;
  existing_stats player_stats%ROWTYPE;
  p_id INTEGER;
  t_num INTEGER;
  win_team INTEGER;
BEGIN
  -- Dla każdego gracza w meczu
  FOR entry IN SELECT * FROM jsonb_array_elements(NEW.player_entries) LOOP
    p_id := (entry->>'player_id')::INTEGER;
    t_num := (entry->>'team_number')::INTEGER;

    -- Znajdź lub utwórz statystyki
    SELECT * INTO existing_stats
    FROM player_stats
    WHERE player_id = p_id;

    IF NOT FOUND THEN
      INSERT INTO player_stats (player_id, appearances)
      VALUES (p_id, 1);

      -- Jeśli mecz ma wynik, zaktualizuj nowo wstawiony rekord
      IF NEW.score1 IS NOT NULL AND NEW.score2 IS NOT NULL AND t_num != 0 THEN
        win_team := NEW.winning_team;
        IF win_team = 0 THEN
          UPDATE player_stats SET draws = draws + 1, matches = matches + 1 WHERE player_id = p_id;
        ELSIF t_num = win_team THEN
          UPDATE player_stats SET wins = wins + 1, matches = matches + 1 WHERE player_id = p_id;
        ELSE
          UPDATE player_stats SET losses = losses + 1, matches = matches + 1 WHERE player_id = p_id;
        END IF;
      END IF;
      CONTINUE;
    END IF;

    -- Aktualizuj występy
    UPDATE player_stats
    SET appearances = appearances + 1
    WHERE id = existing_stats.id;

    -- Jeśli mecz ma wynik i gracz nie jest rezerwą, aktualizuj statystyki meczowe
    IF NEW.score1 IS NOT NULL AND NEW.score2 IS NOT NULL AND t_num != 0 THEN
      win_team := NEW.winning_team;

      IF win_team = 0 THEN
        UPDATE player_stats
        SET draws = draws + 1,
            matches = matches + 1
        WHERE id = existing_stats.id;
      ELSIF t_num = win_team THEN
        UPDATE player_stats
        SET wins = wins + 1,
            matches = matches + 1
        WHERE id = existing_stats.id;
      ELSE
        UPDATE player_stats
        SET losses = losses + 1,
            matches = matches + 1
        WHERE id = existing_stats.id;
      END IF;
    END IF;
  END LOOP;

  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger po zapisaniu meczu
CREATE TRIGGER on_match_created
  AFTER INSERT ON matches
  FOR EACH ROW
  EXECUTE FUNCTION update_player_stats();