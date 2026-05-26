using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GameGuideApp.Core.Interfaces;
using GameGuideApp.Core.Models;

namespace GameGuideApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IGuideRepository _repository;
        private readonly ILicenseService _licenseService;

        private PlatformItem _selectedPlatform;
        private GameItem _selectedGame;
        private Guide _selectedGuide;
        private string _gameSearchText;
        private string _licenseStatus;
        private string _breadcrumbText;
        private string _statusMessage;

        public MainViewModel(IGuideRepository repository, ILicenseService licenseService)
        {
            _repository = repository;
            _licenseService = licenseService;

            Platforms = new ObservableCollection<PlatformItem>
            {
                new PlatformItem { Name = "PC", Icon = "🖥️", IsActive = true },
                new PlatformItem { Name = "PlayStation", Icon = "🎮" },
                new PlatformItem { Name = "Xbox", Icon = "🕹️" },
                new PlatformItem { Name = "Switch", Icon = "🧩" }
            };

            AllGames = new ObservableCollection<GameItem>();
            FilteredGames = new ObservableCollection<GameItem>();
            GuidesForSelectedGame = new ObservableCollection<Guide>();
            ContentBlocks = new ObservableCollection<ContentBlock>();

            LoadDemoDataCommand = new RelayCommand(_ => LoadDemoData());
            SaveGuidesCommand = new RelayCommand(_ => SaveGuides(), _ => GuidesForSelectedGame.Any() || AllGames.SelectMany(g => g.Guides).Any());
            SaveGuideCommand = new RelayCommand(_ => SaveGuide(), _ => SelectedGuide != null);
            ShareGuideCommand = new RelayCommand(_ => ShareGuide(), _ => SelectedGuide != null);

            RefreshLicenseState();
            LoadDemoData();
            SelectedPlatform = Platforms.FirstOrDefault();
        }

        public ObservableCollection<PlatformItem> Platforms { get; }
        public PlatformItem SelectedPlatform { get => _selectedPlatform; set { _selectedPlatform = value; foreach (var p in Platforms) p.IsActive = p == value; OnPropertyChanged(); UpdateBreadcrumb(); } }
        public ObservableCollection<GameItem> AllGames { get; }
        public ObservableCollection<GameItem> FilteredGames { get; }
        public string GameSearchText { get => _gameSearchText; set { _gameSearchText = value; OnPropertyChanged(); ApplyGameFilter(); } }
        public GameItem SelectedGame { get => _selectedGame; set { _selectedGame = value; OnPropertyChanged(); PopulateGuidesForSelectedGame(); UpdateBreadcrumb(); } }
        public ObservableCollection<Guide> GuidesForSelectedGame { get; }
        public Guide SelectedGuide { get => _selectedGuide; set { _selectedGuide = value; OnPropertyChanged(); ParseSelectedGuideContent(); UpdateBreadcrumb(); } }
        public ObservableCollection<ContentBlock> ContentBlocks { get; }
        public string BreadcrumbText { get => _breadcrumbText; private set { _breadcrumbText = value; OnPropertyChanged(); } }
        public string LicenseStatus { get => _licenseStatus; private set { _licenseStatus = value; OnPropertyChanged(); } }
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand LoadDemoDataCommand { get; }
        public ICommand SaveGuidesCommand { get; }
        public ICommand SaveGuideCommand { get; }
        public ICommand ShareGuideCommand { get; }

        private void RefreshLicenseState()
        {
            var license = _licenseService.GetCurrentLicense();
            LicenseStatus = license.IsLicensed ? "Lizenziert" : "Unlizenziert";
        }

        public void LoadDemoData()
        {
            AllGames.Clear();

            AllGames.Add(new GameItem
            {
                Id = "elden-ring",
                Name = "Elden Ring",
                Genre = "Action RPG",
                ThumbEmoji = "🛡️",
                ThumbColor = "#684A2A",
                Guides = new ObservableCollection<Guide>
                {
                    CreateGuide("Elden Ring", "Limgrave Start-Route", "Der optimale Einstieg in die Zwischenwelt", "Einsteiger", false, false,
@"## Erste Schritte in Limgrave
Folge dem goldenen Pfad nur als Orientierung und nimm dir Zeit für Nebenwege.
>> TIPP | Sammle früh Schmiedesteine in den Ruinen, um deine Startwaffe zu stärken.
- Kirche von Elleh besuchen und Händler treffen.
- Torrente durch Rast an einem Gnadenort freischalten.
[MAP] Limgrave Kernroute
## Frühe Runenquellen
Die südliche Halbinsel bietet leichte Gegner und wertvolle Ausrüstung.
- Fort Haight säubern und Asche des Kriegsherrn sichern."),
                    CreateGuide("Elden Ring", "Margit, der Gefallene Omen", "Erster Prüfstein auf dem Weg zur Erdenwurzel", "Bosskampf", true, false,
@"## Kampfvorbereitung
Nutze Geisterasche und verbessere Heilflaschen vor dem Nebeltor.
>> TIPP | Beschwöre Rogier vor dem Kampf, um Druck von dir zu nehmen.
- Warte nach Margits Sprung kurz mit der Ausweichrolle.
- Nutze schwere Angriffe nach seinem Hammer-Slam."),
                    CreateGuide("Elden Ring", "Bleed Arcane Build", "Blutung als konstanter Bosskiller", "Build Guide", true, false, "## Kernattribute\nSetze auf Arkanenergie und Geschick für schnellen Statusaufbau."),
                    CreateGuide("Elden Ring", "Alle Katagrien-Dungeons", "Unterwelt-Routen und Belohnungen", "Exploration", true, true, "## Übersicht\nGesperrter Premiuminhalt."),
                    CreateGuide("Elden Ring", "Alle Enden – Übersicht", "Jede Abschlussvariante im Vergleich", "Story", true, true, "## Übersicht\nGesperrter Premiuminhalt.")
                }
            });

            AllGames.Add(new GameItem
            {
                Id = "diablo-iv", Name = "Diablo IV", Genre = "ARPG", ThumbEmoji = "🔥", ThumbColor = "#5C2323",
                Guides = new ObservableCollection<Guide>
                {
                    CreateGuide("Diablo IV","Nightmare Dungeon Affix-Guide","Affixe lesen und effizient reagieren","Endgame",true,false,"## Affix-Priorisierung\nDefensive Affixe zuerst, dann Tempo."),
                    CreateGuide("Diablo IV","Blizzard Sorc – S-Tier Build","Kontrollorientierter Frost-Meta Build","Build Guide",false,false,"## Skill-Kern\nFrost Nova und Blizzard als Rotationskern.")
                }
            });
            AllGames.Add(new GameItem { Id = "balatro", Name = "Balatro", Genre = "Roguelike Deckbuilder", ThumbEmoji = "🃏", ThumbColor = "#2D3C59", Guides = new ObservableCollection<Guide> { CreateGuide("Balatro", "Joker-Synergien Übersicht", "Die stärksten Joker-Ketten", "Grundlagen", false, false, "## Joker-Engine\nKombiniere Multiplikator-Joker mit Econ-Jokern.") } });
            AllGames.Add(new GameItem { Id = "witcher3", Name = "The Witcher 3", Genre = "Open World RPG", ThumbEmoji = "🐺", ThumbColor = "#3E4658", Guides = new ObservableCollection<Guide> { CreateGuide("The Witcher 3", "Gwent – Alle Karten Guide", "Deckbau, Händler und Turniere", "Mini-Spiel", false, false, "## Kartensuche\nKaufe zuerst Karten bei Wirten in Velen.") } });

            ApplyGameFilter();
            SelectedGame = FilteredGames.FirstOrDefault();
            StatusMessage = "Demo-Daten geladen.";
            CommandManager.InvalidateRequerySuggested();
        }

        public static ObservableCollection<ContentBlock> ParseContent(string raw)
        {
            var blocks = new ObservableCollection<ContentBlock>();
            if (string.IsNullOrWhiteSpace(raw)) return blocks;

            var lines = raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var step = 1;
            foreach (var rawLine in lines)
            {
                var line = rawLine?.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("##")) blocks.Add(new ContentBlock { Type = ContentBlockType.Heading, Text = line.Substring(2).Trim() });
                else if (line.StartsWith(">>"))
                {
                    var tip = line.Substring(2).Trim();
                    var parts = tip.Split(new[] { '|' }, 2);
                    blocks.Add(new ContentBlock { Type = ContentBlockType.Tip, TipLabel = "TIPP", Text = parts.Length == 2 ? parts[1].Trim() : tip });
                }
                else if (line.StartsWith("- ")) blocks.Add(new ContentBlock { Type = ContentBlockType.Step, Text = line.Substring(2).Trim(), StepNumber = step++ });
                else if (line.StartsWith("[MAP]")) blocks.Add(new ContentBlock { Type = ContentBlockType.Map, Text = line.Substring(5).Trim() });
                else blocks.Add(new ContentBlock { Type = ContentBlockType.Paragraph, Text = line });
            }
            return blocks;
        }

        private Guide CreateGuide(string gameName, string title, string subtitle, string category, bool isPremium, bool isLocked, string content)
            => new Guide { Id = Guid.NewGuid().ToString("N"), GameName = gameName, Title = title, Subtitle = subtitle, Category = category, IsPremium = isPremium, IsLocked = isLocked, Content = content, UpdatedAtUtc = DateTime.UtcNow };

        private void ApplyGameFilter()
        {
            var query = (GameSearchText ?? string.Empty).Trim();
            var result = string.IsNullOrEmpty(query) ? AllGames : new ObservableCollection<GameItem>(AllGames.Where(g => g.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0));
            FilteredGames.Clear();
            foreach (var game in result) FilteredGames.Add(game);
        }

        private void PopulateGuidesForSelectedGame()
        {
            GuidesForSelectedGame.Clear();
            if (SelectedGame != null) foreach (var guide in SelectedGame.Guides) GuidesForSelectedGame.Add(guide);
            SelectedGuide = GuidesForSelectedGame.FirstOrDefault();
            CommandManager.InvalidateRequerySuggested();
        }

        private void ParseSelectedGuideContent()
        {
            ContentBlocks.Clear();
            if (SelectedGuide == null) return;
            foreach (var b in ParseContent(SelectedGuide.Content)) ContentBlocks.Add(b);
        }

        private void SaveGuides()
        {
            _repository.SaveAll(AllGames.SelectMany(g => g.Guides).ToList());
            StatusMessage = "Guides gespeichert.";
        }

        private void SaveGuide() { if (SelectedGuide != null) SaveGuides(); }
        private void ShareGuide() { StatusMessage = SelectedGuide == null ? "Kein Guide ausgewählt." : "Teilen vorbereitet: " + SelectedGuide.Title; }
        private void UpdateBreadcrumb() => BreadcrumbText = string.Format("{0} › {1} › {2}", SelectedPlatform?.Name ?? "Plattform", SelectedGame?.Name ?? "Spiel", SelectedGuide?.Title ?? "Guide");
    }
}
