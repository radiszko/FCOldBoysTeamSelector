-- Migracja: zamień kolumnę 'attack' na 'gra_bez_pilki' i 'gra_z_pilka'
-- Uruchom w Supabase SQL Editor

-- 1. Dodaj nowe kolumny
ALTER TABLE players ADD COLUMN IF NOT EXISTS gra_bez_pilki INTEGER NOT NULL DEFAULT 5;
ALTER TABLE players ADD COLUMN IF NOT EXISTS gra_z_pilka INTEGER NOT NULL DEFAULT 5;

-- 2. Usuń starą kolumnę 'attack'
ALTER TABLE players DROP COLUMN IF EXISTS attack;

-- 3. Zaktualizuj dane zawodników z CSV (nazwy z bazy, wartości z pliku)
-- CSV kolumny: SZY=speed, WYT=stamina, SIŁ=strength, GBP=gra_bez_pilki, GZP=gra_z_pilka, OBR=defense

UPDATE players SET speed=8, stamina=6, strength=4, gra_bez_pilki=6, gra_z_pilka=6, defense=6 WHERE name='Arco Mandatić';
UPDATE players SET speed=6, stamina=6, strength=5, gra_bez_pilki=8, gra_z_pilka=6, defense=4 WHERE name='Artur Kowalczyk';
UPDATE players SET speed=8, stamina=8, strength=7, gra_bez_pilki=7, gra_z_pilka=7, defense=7 WHERE name='Bartolomeo Elefante';
UPDATE players SET speed=6, stamina=7, strength=7, gra_bez_pilki=5, gra_z_pilka=2, defense=5 WHERE name='Burak Fatih Ćviklić';
UPDATE players SET speed=6, stamina=6, strength=5, gra_bez_pilki=6, gra_z_pilka=5, defense=6 WHERE name='Christopher Butter';
UPDATE players SET speed=6, stamina=7, strength=9, gra_bez_pilki=5, gra_z_pilka=6, defense=9 WHERE name='Cvetan Gregorić';
UPDATE players SET speed=10, stamina=9, strength=5, gra_bez_pilki=9, gra_z_pilka=7, defense=6 WHERE name='David Krasnalić';
UPDATE players SET speed=9, stamina=6, strength=8, gra_bez_pilki=9, gra_z_pilka=10, defense=5 WHERE name='Davor Dryblović';
UPDATE players SET speed=8, stamina=8, strength=10, gra_bez_pilki=5, gra_z_pilka=7, defense=10 WHERE name='Dragan Zastavić';
UPDATE players SET speed=5, stamina=5, strength=9, gra_bez_pilki=7, gra_z_pilka=8, defense=7 WHERE name='Igoro Serata';
UPDATE players SET speed=7, stamina=7, strength=8, gra_bez_pilki=7, gra_z_pilka=7, defense=7 WHERE name='Loukas Adamos';
UPDATE players SET speed=8, stamina=7, strength=8, gra_bez_pilki=9, gra_z_pilka=10, defense=4 WHERE name='Luka Lagović';
UPDATE players SET speed=4, stamina=3, strength=9, gra_bez_pilki=7, gra_z_pilka=7, defense=6 WHERE name='Lukas Frydelić';
UPDATE players SET speed=9, stamina=6, strength=9, gra_bez_pilki=9, gra_z_pilka=10, defense=5 WHERE name='Marcelino';
UPDATE players SET speed=5, stamina=6, strength=9, gra_bez_pilki=8, gra_z_pilka=9, defense=9 WHERE name='Marco Zvodić';
UPDATE players SET speed=10, stamina=7, strength=7, gra_bez_pilki=9, gra_z_pilka=8, defense=6 WHERE name='Mario Zaivanić';
UPDATE players SET speed=8, stamina=8, strength=6, gra_bez_pilki=6, gra_z_pilka=8, defense=9 WHERE name='Nico Pantofelić';
UPDATE players SET speed=5, stamina=5, strength=5, gra_bez_pilki=5, gra_z_pilka=5, defense=5 WHERE name='Paolo Portiere';
UPDATE players SET speed=7, stamina=6, strength=10, gra_bez_pilki=10, gra_z_pilka=7, defense=7 WHERE name='Pavlo Glovković';
UPDATE players SET speed=7, stamina=7, strength=10, gra_bez_pilki=6, gra_z_pilka=5, defense=9 WHERE name='Radko Skodić';
UPDATE players SET speed=8, stamina=6, strength=8, gra_bez_pilki=9, gra_z_pilka=9, defense=10 WHERE name='Rafał Ćwiru';
UPDATE players SET speed=7, stamina=9, strength=9, gra_bez_pilki=6, gra_z_pilka=7, defense=9 WHERE name='Roberto Nervosso';
UPDATE players SET speed=5, stamina=5, strength=5, gra_bez_pilki=5, gra_z_pilka=5, defense=5 WHERE name='Roberto Paradić';
UPDATE players SET speed=3, stamina=3, strength=9, gra_bez_pilki=7, gra_z_pilka=9, defense=6 WHERE name='Sebastian Hojny';
UPDATE players SET speed=7, stamina=7, strength=6, gra_bez_pilki=7, gra_z_pilka=5, defense=7 WHERE name='Sebastiano Gardino';
UPDATE players SET speed=6, stamina=7, strength=7, gra_bez_pilki=7, gra_z_pilka=6, defense=7 WHERE name='Szaboszlai Szajbus';
UPDATE players SET speed=9, stamina=7, strength=8, gra_bez_pilki=8, gra_z_pilka=8, defense=6 WHERE name='Tacco Luppo';
UPDATE players SET speed=7, stamina=7, strength=5, gra_bez_pilki=6, gra_z_pilka=6, defense=6 WHERE name='Tanni Armani';
UPDATE players SET speed=8, stamina=10, strength=7, gra_bez_pilki=7, gra_z_pilka=6, defense=8 WHERE name='Tin Elektrić';
UPDATE players SET speed=10, stamina=9, strength=7, gra_bez_pilki=7, gra_z_pilka=8, defense=6 WHERE name='Tomo Torpedić';
UPDATE players SET speed=8, stamina=8, strength=9, gra_bez_pilki=8, gra_z_pilka=4, defense=6 WHERE name='Voja Sprinterić';
UPDATE players SET speed=8, stamina=8, strength=7, gra_bez_pilki=7, gra_z_pilka=4, defense=8 WHERE name='Zeb Macahan';
UPDATE players SET speed=3, stamina=5, strength=10, gra_bez_pilki=6, gra_z_pilka=5, defense=9 WHERE name='Zoran Zalević';

-- 4. Wstaw nowych zawodników, których jeszcze nie ma w bazie
-- (INSERT ... ON CONFLICT wymaga UNIQUE na name, więc użyjemy warunkowego INSERT)
INSERT INTO players (name, nationality, speed, stamina, strength, gra_bez_pilki, gra_z_pilka, defense)
SELECT v.name, v.nationality, v.speed, v.stamina, v.strength, v.gra_bez_pilki, v.gra_z_pilka, v.defense
FROM (VALUES
  ('Arco Mandatić', '', 8, 6, 4, 6, 6, 6),
  ('Artur Kowalczyk', '', 6, 6, 5, 8, 6, 4),
  ('Bartolomeo Elefante', '', 8, 8, 7, 7, 7, 7),
  ('Burak Fatih Ćviklić', '', 6, 7, 7, 5, 2, 5),
  ('Christopher Butter', '', 6, 6, 5, 6, 5, 6),
  ('Loukas Adamos', '', 7, 7, 8, 7, 7, 7),
  ('Luka Lagović', '', 8, 7, 8, 9, 10, 4),
  ('Lukas Frydelić', '', 4, 3, 9, 7, 7, 6),
  ('Marcelino', '', 9, 6, 9, 9, 10, 5),
  ('Mario Zaivanić', '', 10, 7, 7, 9, 8, 6),
  ('Rafał Ćwiru', '', 8, 6, 8, 9, 9, 10),
  ('Roberto Paradić', '', 5, 5, 5, 5, 5, 5),
  ('Sebastiano Gardino', '', 7, 7, 6, 7, 5, 7),
  ('Szaboszlai Szajbus', '', 6, 7, 7, 7, 6, 7),
  ('Tanni Armani', '', 7, 7, 5, 6, 6, 6),
  ('Voja Sprinterić', '', 8, 8, 9, 8, 4, 6)
) AS v(name, nationality, speed, stamina, strength, gra_bez_pilki, gra_z_pilka, defense)
WHERE NOT EXISTS (SELECT 1 FROM players p WHERE p.name = v.name);
