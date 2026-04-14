# Supabase Database Setup Instructions

## Przeprowadź te kroki w Supabase Dashboard:

### Krok 1: Uruchom seed danych dla zawodników

1. Wejdź na: https://supabase.com/dashboard/project/mmybmfoynuilwksuluxw/sql
2. Kliknij **New Query**
3. Kliknij ikonę "New" (plus) i wybierz **Import from SQL file**
4. Wybierz plik: `supabase_setup.sql`
5. Kliknij **Run** (trójkąt na dole)

Dzięki temu:
- ✅ Zawodnicy będą pobierani z bazy, nie z kodu
- ✅ Tablica `players` będzie zawierać 17 zawodników
- ✅ Tablica `player_stats` będzie gotowa do statystyk

### Krok 2: Uruchom setup dla meczów (opcjonalnie)

Jeśli chcesz przenieść historię meczów do Supabase:

1. Kliknij **New Query**
2. Kliknij ikonę "New" i wybierz **Import from SQL file**
3. Wybierz plik: `matches_setup.sql`
4. Kliknij **Run**

Dzięki temu:
- ✅ Historia meczów będzie w bazie
- ✅ Automatyczne aktualizowanie statystyk
- ✅ Automatyczne czyszczenie starych meczów

### Krok 3: Sprawdź dane

Wejdź na: https://supabase.com/dashboard/project/mmybmfoynuilwksuluxw/editor

Powinienes widzieć tabele:
- `players` - 17 zawodników
- `player_stats` - pusta (będzie wypełniona automatycznie)

---

## Co zostało zaimplementowane w kodzie:

### ✅ Usunięte hardcoded dane:
- Nie ma już 17 zawodników w kodzie
- Dane są pobierane z Supabase

### ✅ Dodane statystyki zawodników:
- Model `PlayerStats` w bazie
- Frekwencja: ile razy grał (wszystkie mecze)
- % frekwencji: (Obecności / Mecze) × 100
- Statystyki meczowe: wygrane, remisy, przegrane

### ✅ Naprawiony link "Generuj Skład":
- Prawidłowy base href: `/`
- Linki działają poprawnie

---

## Jak to działa teraz:

1. **Pierwsze uruchomienie:**
   - Aplikacja próbuje pobrać zawodników z Supabase
   - Jeśli tabela jest pusta - zostanie stworzona z seed danymi

2. **Dodawanie/edycja zawodników:**
   - Dane są zapisywane w Supabase
   - Wszyscy użytkownicy widzą te same zawodników

3. **Statystyki:**
   - Po zapisaniu meczu z wynikiem
   - Statystyki zawodników są automatycznie aktualizowane
   - Liczone są wygrane, remisy, przegrane
   - Wyliczana jest frekwencja i % wygranych

---

## Testowanie:

1. Uruchom aplikację
2. Wejdź na `/admin/players` - zobaczysz zawodników z bazy
3. Edytuj parametry - są zapisywane w Supabase
4. Przejdź na `/statistics` - zobaczysz statystyki
5. Generuj mecz, zapisz z wynikiem - statystyki się aktualizują