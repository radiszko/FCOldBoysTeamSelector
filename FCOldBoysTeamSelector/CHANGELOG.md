# Changelog

Wszystkie istotne zmiany w tym projekcie będą dokumentowane w pliku CHANGELOG.md. Format oparty o [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [1.0.0] - 2026-04-15

### Added
- Podstawowa funkcjonalność generowania składów drużyn piłkarskich
- Zarządzanie zawodnikami (CRUD)
- Integracja z Supabase jako bazą danych
- System grup zawodników (always together)
- Oznaczanie bramkarzy
- Generowanie zespołów z uwzględnieniem balansu siły
- Historia meczów z zapisywaniem wyników
- Statystyki graczy
- Clipboard copy functionality
- Panel administracyjny dla graczy
- Responsive design z Bootstrap 5

### Changed
- Refactor SupabaseService z hardcoded credentials do konfiguracji
- Dodanie Microsoft.Extensions.Logging.Configuration

### Fixed
- Naprawa błędu kompilacji - brakujący pakiet Logging.Configuration
- Dodanie RLS policy "Allow public read access" dla tabeli players

### Security
- Publikacyjny klucz API w aplikacji (zalecane użycie Service Key dla operacji zapisu)

---

## [Unreleased]

### TODO
- Uwierzytelnianie użytkowników
- Eksport/import zawodników (CSV)
- Szablony turniejów
- Powiadomienia email
