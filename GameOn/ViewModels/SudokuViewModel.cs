using GameOn.Commande;
using GameOn.Data;
using GameOn.Games.Sudoku;
using GameOn.Models;
using GameOn.Models.Sudoku;
using GameOn.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Threading;

namespace GameOn.ViewModels
{
    public class SudokuViewModel : BaseViewModel
    {
        // Collection qui contiendra les 81 cases du Sudoku
        public ObservableCollection<SudokuCell> Cells { get; set; }

        // Moteur qui récupère et convertit la grille de Sudoku
        private SudokuEngine sudokuEngine;

        // Commande qui permet de revenir à la page d'accueil des jeux
        public RelaisCommande BackCommand { get; }

        // Tableau qui stocke la solution du Sudoku
        private int[,]? solutionGrid;

        // Sauvegarde de la partie 
        private readonly SessionService session;
        private readonly GameSessionService sessions;
        private readonly IDailyChallengeProvider challenges;
        private DailyChallenge? challenge;   // défi du jour chargé pour cette partie


        private readonly Difficulty difficulty;
        private int? sessionId;                   // null tant que la partie n'existe pas en base
        private bool saveInProgress;
        private bool savePending;
        private DateTime? startedAt;

        // Minuteur qui actualise le chrono chaque seconde
        private readonly DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        // Temps depuis le lancement de la partie
        private TimeSpan time = TimeSpan.Zero;

        // Propriété qui actualise l'affichage quand le temps change
        public TimeSpan Time
        {
            get => time;

            private set
            {
                time = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedTime));
            }
        }

        // Conversion du temps dans le format d'affichage
        public string FormattedTime => $"{(int)Time.TotalHours:00}:{Time.Minutes:00}:{Time.Seconds:00}";

        // Booléen qui définit si la partie est terminée
        private bool isFinished;

        public bool IsFinished
        {
            get => isFinished;
            private set
            {
                if (isFinished != value)
                {
                    isFinished = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanConfirm));
                }
            }
        }

        // Booléen qui définir le statut du bouton "Confirmer". Il n'est actif que si la grille est pleine et la partie pas encore terminée
        public bool CanConfirm => IsGridComplete && !IsFinished;


        // Booléen qui définit si la grille de sudoku est en mode notes ou pas
        private bool isNoteMode;

        public bool IsNoteMode
        {
            get
            {
                return isNoteMode;
            }
            set
            {
                if (isNoteMode != value)
                {
                    isNoteMode = value;
                    OnPropertyChanged();
                }
            }
        }

        // Initialisation des cases, du moteur sudoku et de la commande de retour
        public SudokuViewModel(Action retour, Difficulty difficulty, SessionService session,
                       GameSessionService sessions, IDailyChallengeProvider challenges)
        {
            Cells = new ObservableCollection<SudokuCell>();
            sudokuEngine = new SudokuEngine();

            this.session = session;
            this.sessions = sessions;
            this.difficulty = difficulty;
            this.challenges = challenges;

            // Actualisation du chrono chaque seconde
            timer.Tick += (sender, e) =>
            {
                UpdateTimer();
            };

            // Reprend la partie du jour si elle existe, sinon charge le défi du jour
            // (casual et classé ont chacun leur propre défi)
            _ = StartGameAsync();

            BackCommand = new RelaisCommande(retour);
        }

        // Actualisation du temps écoulé à partir de l'heure du début de la partie
        private void UpdateTimer()
        {
            if (startedAt != null && !IsFinished)
            {
                Time = DateTime.Now - startedAt.Value;
            }
        }

        // Démarrage du chrono
        public void StartTimer()
        {
            UpdateTimer();
            timer.Start();
        }

        // Arrêt du chrono (lorsque le joueur quitte la page)
        public void StopTimer()
        {
            timer.Stop();
        }

        // Relance du chrono (lorsque le joueur revient sur le jeu)
        public void ResumeTimer()
        {
            if (startedAt != null && !IsFinished)
            {
                StartTimer();
            }
        }

        // Récupèration de la grille depuis le moteur Sudoku
        public async Task LoadSudokuAsync(Difficulty difficulty)
        {
            try
            {
                int[,] grid = await sudokuEngine.GetSudokuBaseGridAsync(difficulty);
                int[,] solution = await sudokuEngine.GetSudokuSolutionGridAsync(difficulty);
                solutionGrid = solution;

                foreach (SudokuCell cell in Cells)
                {
                    cell.PropertyChanged -= Cell_PropertyChanged;

                    foreach (SudokuNote note in cell.Notes)
                    {
                        note.PropertyChanged -= Note_PropertyChanged;
                    }
                }
                Cells.Clear();

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        int value = grid[row, col];
                        bool isEditable = value == 0;

                        SudokuCell cell = new SudokuCell(value, row, col, isEditable);

                        AddCell(cell);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans l'affichage du Sudoku : {ex.Message}");
            }
        }

        // Vérification de la validité de la grille
        public bool IsGridValid()
        {
            if (solutionGrid == null || Cells.Count != 81)
            {
                return false;
            }
            else
            {
                foreach (SudokuCell cell in Cells)
                {
                    if (cell.CellValue != solutionGrid[cell.Row, cell.Column])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        // Vérifie que la grille est bien chargée et complète
        public bool IsGridComplete
        {
            get
            {
                return Cells.Count == 81 && Cells.All(c => c.CellValue != 0);
            }
        }

        // Retourne la réponse de la case sélectionnée par le joueur (uniquement disponible pour le mode Casual en production)
        public int GiveCellAnswer(SudokuCell selectedCell)
        {
            if (solutionGrid == null)
            {
                return 0;
            }
            else
            {
                return solutionGrid[selectedCell.Row, selectedCell.Column];
            }
        }

        // Changement du mode (notes ou pas)
        public void SwitchNoteMode()
        {
            IsNoteMode = !IsNoteMode;
        }

        // Ajout d'une note dans une case vide (valeurs 1 à 9)
        public void ManageNote(SudokuCell cell, int value)
        {
            if (cell.IsEditable && cell.CellValue == 0 && value >= 1 && value <= 9)
            {
                SudokuNote note = cell.Notes[value - 1];
                note.IsVisible = !note.IsVisible;
            }
        }

        // Actualisation de l'état du bouton "Confirmer" lorsqu'une case change
        private void Cell_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SudokuCell.CellValue))
            {
                OnPropertyChanged(nameof(IsGridComplete));
                OnPropertyChanged(nameof(CanConfirm));
                _ = SaveGameAsync();
            }
        }

        // [TEMPORAIRE] Remplissage de la grille avec la solution
        public void SolveGrid()
        {
            if (solutionGrid != null && Cells.Count == 81)
            {
                foreach (SudokuCell cell in Cells)
                {
                    if (cell.IsEditable)
                    {
                        cell.CellValue = solutionGrid[cell.Row, cell.Column];

                        foreach (SudokuNote note in cell.Notes)
                        {
                            note.IsVisible = false;
                        }
                    }
                }
            }
        }


        // ===== Sauvegarde de la partie (Save a game) =====

        // Reprise de la partie du jour, ou création d'une nouvelle partie
        private async Task StartGameAsync()
        {
            try
            {
                int employeeId = session.Utilisateur!.EmployeeId;

                GameSession? gameSession = await sessions.TrouverPartieDuJourAsync(
                    employeeId, SeedData.SudokuGameId, difficulty);

                if (gameSession != null)
                {
                    challenge = gameSession.DailyChallenge!;
                    RestoreGame(SudokuSaveData.DepuisJson(gameSession.GameData));
                    sessionId = gameSession.SessionId;
                    startedAt = gameSession.StartedAt;

                    // Partie du jour déjà terminée : on l'affiche, verrouillée
                    if (gameSession.Status == Status.COMPLETED)
                    {
                        Time = (gameSession.CompletedAt ?? DateTime.Now) - gameSession.StartedAt;
                        LockGrid();
                    }
                    else
                    {
                        StartTimer();
                    }

                    return;
                }

                await LoadChallengeAsync();

                // la grille n'a pas pu être chargée
                if (Cells.Count != 81) return;

                GameSession newSession = await sessions.CreerPartieAsync(
                                employeeId, SeedData.SudokuGameId,
                                challenge!.ChallengeId,
                                CreateSaveData().VersJson());
                sessionId = newSession.SessionId;
                startedAt = newSession.StartedAt;

                StartTimer();

                // Enregistre les coups joués pendant la création de la partie
                _ = SaveGameAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de sauvegarde du Sudoku : {ex.Message}");
            }
        }


        // ===== Fin de la partie (Complete a game) =====

        // Vérifie la condition de fin (grille = solution) puis enregistre le résultat.
        // Renvoie le temps de résolution, ou null si la grille est fausse / la partie n'a pas pu être enregistrée.
        public async Task<TimeSpan?> CompleteGameAsync()
        {
            if (IsFinished || sessionId == null || startedAt == null || !IsGridValid())
            {
                return null;
            }

            DateTime? completedAt = await sessions.TerminerAsync(sessionId.Value, CreateSaveData().VersJson());
            if (completedAt == null)
            {
                return null; // la partie n'était plus en cours en base (déjà terminée ou expirée)
            }

            timer.Stop();
            Time = completedAt.Value - startedAt.Value;
            
            LockGrid();
            return Time;
        }

        // Bloque la grille et coupe la sauvegarde
        private void LockGrid()
        {
            IsFinished = true;
            foreach (SudokuCell cell in Cells)
            {
                cell.IsEditable = false;
            }
        }

        // Ajoute une case à la grille et écoute ses changements (valeur et notes)
        private void AddCell(SudokuCell cell)
        {
            cell.PropertyChanged += Cell_PropertyChanged;
            foreach (SudokuNote note in cell.Notes)
            {
                note.PropertyChanged += Note_PropertyChanged;
            }
            Cells.Add(cell);
        }

        private void Note_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SudokuNote.IsVisible))
            {
                _ = SaveGameAsync();
            }
        }

        // État actuel de la grille et  données à sauvegarder
        private SudokuSaveData CreateSaveData()
        {
            var data = new SudokuSaveData();

            foreach (SudokuCell cell in Cells)
            {
                int i = cell.Row * 9 + cell.Column;
                data.Valeurs[i] = cell.CellValue;
                data.Notes[i] = cell.Notes.Where(n => n.IsVisible).Select(n => n.NoteValue).ToArray();
            }

            return data;
        }

        // Grille de départ et solution depuis le défi du jour, progression depuis la sauvegarde
        private void RestoreGame(SudokuSaveData data)
        {
            int[,] grid = sudokuEngine.JsonToList(challenge!.ChallengeData, "puzzle");
            solutionGrid = sudokuEngine.JsonToList(challenge.ChallengeData, "solution");
            Cells.Clear();

            for (int i = 0; i < 81; i++)
            {
                bool isEditable = grid[i / 9, i % 9] == 0;
                var cell = new SudokuCell(data.Valeurs[i], i / 9, i % 9, isEditable);

                foreach (int note in data.Notes[i])
                {
                    cell.Notes[note - 1].IsVisible = true;
                }

                AddCell(cell); // abonnement APRÈS avoir rempli la case : la restauration ne déclenche pas de sauvegarde
            }

            OnPropertyChanged(nameof(IsGridComplete));
            OnPropertyChanged(nameof(CanConfirm));
        
        }


        // Sauvegarde garantie quand le joueur quitte le jeu (changement d'écran ou fermeture).
        // Attend la fin d'une sauvegarde en cours, puis enregistre l'état final.
        public async Task QuitAsync()
        {
            StopTimer();

            if (sessionId == null || IsFinished) return; 

            while (saveInProgress)
            {
                await Task.Delay(50);
            }

            try
            {
                await sessions.SauvegarderAsync(sessionId.Value, CreateSaveData().VersJson());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de sauvegarde du Sudoku : {ex.Message}");
            }
        }

        // Sauvegarde à chaque coup. Si plusieurs coups arrivent pendant une sauvegarde
        private async Task SaveGameAsync()
        {
            if (sessionId == null || IsFinished) return;

            if (saveInProgress)
            {
                savePending = true;
                return;
            }

            saveInProgress = true;
            try
            {
                do
                {
                    savePending = false;
                    await sessions.SauvegarderAsync(sessionId.Value, CreateSaveData().VersJson());
                }
                while (savePending);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de sauvegarde du Sudoku : {ex.Message}");
            }
            finally
            {
                saveInProgress = false;
            }
        }

        // Charge la grille et la solution depuis le défi du jour (une seule source : grille et solution correspondent)
        private async Task LoadChallengeAsync()
        {
            challenge = await challenges.ObtenirDefiDuJourAsync(difficulty);

            int[,] grid = sudokuEngine.JsonToList(challenge.ChallengeData, "puzzle");
            solutionGrid = sudokuEngine.JsonToList(challenge.ChallengeData, "solution");

            Cells.Clear();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = grid[row, col];
                    AddCell(new SudokuCell(value, row, col, value == 0));
                }
            }
        }

    }
}
