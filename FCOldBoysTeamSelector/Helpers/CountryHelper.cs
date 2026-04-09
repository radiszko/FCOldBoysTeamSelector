namespace FCOldBoysTeamSelector.Helpers;

public static class CountryHelper
{
    /// <summary>Konwertuje 2-literowy kod ISO 3166-1 alpha-2 na emoji flagi.</summary>
    public static string ToFlagEmoji(string? countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) return "";
        return string.Concat(countryCode.ToUpper()
            .Select(c => char.ConvertFromUtf32(c - 'A' + 0x1F1E6)));
    }

    public static readonly List<(string Code, string Name)> Countries =
    [
        // Brak flagi
        ("",   "— brak flagi —"),

        // Europa
        ("AL", "Albania"),
        ("AT", "Austria"),
        ("BY", "Białoruś"),
        ("BE", "Belgia"),
        ("BA", "Bośnia i Hercegowina"),
        ("BG", "Bułgaria"),
        ("HR", "Chorwacja"),
        ("CZ", "Czechy"),
        ("DK", "Dania"),
        ("EE", "Estonia"),
        ("FI", "Finlandia"),
        ("FR", "Francja"),
        ("GR", "Grecja"),
        ("ES", "Hiszpania"),
        ("NL", "Holandia"),
        ("IE", "Irlandia"),
        ("IS", "Islandia"),
        ("LT", "Litwa"),
        ("LV", "Łotwa"),
        ("MK", "Macedonia Północna"),
        ("MD", "Mołdawia"),
        ("DE", "Niemcy"),
        ("NO", "Norwegia"),
        ("PL", "Polska"),
        ("PT", "Portugalia"),
        ("RO", "Rumunia"),
        ("RS", "Serbia"),
        ("SK", "Słowacja"),
        ("SI", "Słowenia"),
        ("CH", "Szwajcaria"),
        ("SE", "Szwecja"),
        ("TR", "Turcja"),
        ("UA", "Ukraina"),
        ("HU", "Węgry"),
        ("GB", "Wielka Brytania"),
        ("IT", "Włochy"),

        // Ameryka Północna i Środkowa
        ("CA", "Kanada"),
        ("MX", "Meksyk"),
        ("US", "USA"),
        ("CR", "Kostaryka"),
        ("PA", "Panama"),
        ("JM", "Jamajka"),

        // Ameryka Południowa
        ("AR", "Argentyna"),
        ("BO", "Boliwia"),
        ("BR", "Brazylia"),
        ("CL", "Chile"),
        ("CO", "Kolumbia"),
        ("EC", "Ekwador"),
        ("PY", "Paragwaj"),
        ("PE", "Peru"),
        ("UY", "Urugwaj"),
        ("VE", "Wenezuela"),
    ];
}
