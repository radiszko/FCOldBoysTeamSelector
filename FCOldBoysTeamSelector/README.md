# FC Old Boys Team Selector

<div align="center">

**Wersja: 1.0.0**

*Aplikacja do generowania zrównoważonych składów drużyn piłkarskich*

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Version](https://img.shields.io/badge/version-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![Blazor](https://img.shields.io/badge/Blazor-WASM-512bd4)

</div>

## 📋 Opis

FC Old Boys Team Selector to aplikacja webowa built w Blazor WebAssembly, która pozwala na łatwe generowanie zrównoważonych drużyn piłkarskich z uwzględnieniem parametrów zawodników.

## ✨ Cechy

- **Generowanie drużyn** - Automatyczne tworzenie zbalansowanych zespołów
- **Zarządzanie zawodnikami** - CRUD operacje z parametrami (szybkość, wybieganie, obrona, gra z/bez piłki, siła)
- **System grup** - Zawodnicy z tej samej grupy zawsze grają razem
- **Bramkarze** - Specjalna obsługa bramkarzy
- **Statystyki** - Historia meczów i statystyki graczy
- **Supabase integracja** - Pełna synchronizacja z bazą danych

## 🛠️ Stack technologiczny

- **Frontend:** Blazor WebAssembly (.NET 10)
- **UI:** Bootstrap 5 + Bootstrap Icons
- **Backend/DB:** Supabase (PostgreSQL)
- **Authentication:** Supabase Auth (RLS policies)

## 🚀 Instalacja

### Wymagania

- .NET 10 SDK
- Konto Supabase
- Węzłowy dostępu do internetu

### Konfiguracja lokalna

1. Sklonuj repozytorium:
```bash
git clone https://github.com/radiszko/FCOldBoysTeamSelector.git
cd FCOldBoysTeamSelector
```

2. Skonfiguruj Supabase w `appsettings.json`:
```json
{
  "Supabase": {
    "BaseUrl": "https://twoj-projekt.supabase.co",
    "ApiKey": "twoj-api-key"
  }
}
```

3. Uruchom aplikację:
```bash
dotnet run
```

Aplikacja będzie dostępna pod adresem: http://localhost:5011

## 📦 Deployment

Aplikacja może być hostowana na:
- **GitHub Pages** - statyczne hostingi
- **Azure Static Web Apps**
- **Netlify**
- **Vercel**

## 🔒 Bezpieczeństwo

- Używaj **Service Role Key** tylko na serwerze backendowym
- **Public key** jest bezpieczny do użycia w klient z RLS policy
- Włącz odpowiednie RLS (Row Level Security) policy w Supabase

## 📝 Licencja

MIT License - zobacz plik LICENSE dla szczegółów.

## 🤝 Wkład

Wnoszenie wkładu jest mile widziane! Proszę otworzyć issue przed znaczącymi zmianami.

## 📞 Kontakt

- GitHub: [@radiszko](https://github.com/radiszko)

---

**Changelog:** Zobacz [CHANGELOG.md](CHANGELOG.md) dla historii zmian.
