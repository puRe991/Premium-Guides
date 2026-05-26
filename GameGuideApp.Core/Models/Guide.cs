using System;

namespace GameGuideApp.Core.Models
{
    // Repräsentiert einen einzelnen Guide-Eintrag in der App.
    // Alles ist bewusst textbasiert gehalten, damit später einfach
    // ein Import/Export oder Store-Sync ergänzt werden kann.
    public class Guide
    {
        // Eindeutige technische ID des Guides.
        public string Id { get; set; }

        // Name des Spiels, z. B. "Elden Ring" oder "Diablo IV".
        public string GameName { get; set; }

        // Sichtbarer Titel des Guides.
        public string Title { get; set; }

        public string Subtitle { get; set; }

        // Kategorie (z. B. Leveling, PvE, Boss, Crafting).
        public string Category { get; set; }

        // Der eigentliche Guide-Text (Anleitung / Tipps / Schritte).
        public string Content { get; set; }

        // Optionaler Pfad zu einer Grafik wie Map, Screenshot oder Diagramm.
        // Für den Start reicht ein lokaler Dateipfad oder eine URL als Text.
        public string MapAssetPath { get; set; }

        // Zeitstempel für letzte inhaltliche Änderung (UTC).
        public DateTime UpdatedAtUtc { get; set; }

        // Kennzeichnet Premium-/gesperrte Inhalte.
        public bool IsLocked { get; set; }

        public bool IsPremium { get; set; }
    }
}
