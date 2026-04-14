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
  entry RECORD;
  existing_stats PLAYERSTATS;
  win_team INTEGER;
BEGIN
  -- Dla każdego gracza w meczu
  FOR entry IN SELECT * FROM jsonb_array_elements(player_entries) LOOP
    -- Znajdź lub utwórz statystyki
    SELECT * INTO existing_stats
    FROM player_stats
    WHERE player_id = entry->>'playerId'::INTEGER;

    IF NOT FOUND THEN
      INSERT INTO player_stats (player_id, appearances)
      VALUES (entry->>'playerId'::INTEGER, 1);
      CONTINUE;
    END IF;

    -- Aktualizuj występy
    UPDATE player_stats
    SET appearances = appearances + 1
    WHERE id = existing_stats.id;

    -- Jeśli mecz ma wynik, aktualizuj statystyki meczowe
    IF score1 IS NOT NULL AND score2 IS NOT NULL THEN
      win_team := winning_team;

      -- Jeśli remis - żadna ekipa nie wygrała
      IF win_team = 0 THEN
        UPDATE player_stats
        SET draws = draws + 1,
            matches = matches + 1
        WHERE id = existing_stats.id;
      ELSE
        -- Sprawdź czy gracz grał w ekipie wygrywającej
        IF entry->>'teamNumber'::INTEGER = win_team THEN
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