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
        private readonly SessionService _session;
        private readonly GameSessionService _sessions;
        private readonly Difficulty _difficulty;
        private int? _sessionId;                   // null tant que la partie n'existe pas en base
        private bool _sauvegardeEnCours;
        private bool _sauvegardeEnAttente;


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
        public SudokuViewModel(Action retour, Difficulty difficulty,
               SessionService session, GameSessionService sessions)
        {
            Cells = new ObservableCollection<SudokuCell>();
            sudokuEngine = new SudokuEngine();

            _session = session;
            _sessions = sessions;
            _difficulty = difficulty;

            // Si le mode est casual, passe la difficulté en easy
            Difficulty gridDifficulty = difficulty == Difficulty.CASUAL ? Difficulty.EASY : difficulty;

            // Reprend la partie du jour si elle existe, sinon charge une nouvelle grille
            _ = DemarrerPartieAsync(gridDifficulty);

            BackCommand = new RelaisCommande(retour);
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
                }
                Cells.Clear();

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        int value = grid[row, col];
                        bool isEditable = value == 0;

                        SudokuCell cell = new SudokuCell(value, row, col, isEditable);

                        AjouterCase(cell);
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
                _ = SauvegarderAsync();
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
        private async Task DemarrerPartieAsync(Difficulty gridDifficulty)
        {
            try
            {
                int employeeId = _session.Utilisateur!.EmployeeId;

                GameSession? partie = await _sessions.TrouverPartieDuJourAsync(
                    employeeId, SeedData.SudokuGameId, _difficulty);

                if (partie != null)
                {
                    Restaurer(SudokuSaveData.DepuisJson(partie.GameData));
                    _sessionId = partie.SessionId;
                    return;
                }

                await LoadSudokuAsync(gridDifficulty);

                // la grille n'a pas pu être chargée
                if (Cells.Count != 81) return; 

                GameSession nouvelle = await _sessions.CreerPartieAsync(
                    employeeId, SeedData.SudokuGameId, _difficulty, CreerSaveData().VersJson());
                _sessionId = nouvelle.SessionId;

                // Enregistre les coups joués pendant la création de la partie
                _ = SauvegarderAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de sauvegarde du Sudoku : {ex.Message}");
            }
        }

        // Ajoute une case à la grille et écoute ses changements (valeur et notes)
        private void AjouterCase(SudokuCell cell)
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
                _ = SauvegarderAsync();
            }
        }

        // État actuel de la grille et  données à sauvegarder
        private SudokuSaveData CreerSaveData()
        {
            var data = new SudokuSaveData();

            foreach (SudokuCell cell in Cells)
            {
                int i = cell.Row * 9 + cell.Column;
                data.GrilleDepart[i] = cell.IsEditable ? 0 : cell.CellValue;
                data.Valeurs[i] = cell.CellValue;
                data.Notes[i] = cell.Notes.Where(n => n.IsVisible).Select(n => n.NoteValue).ToArray();
            }

            if (solutionGrid != null)
            {
                data.Solution = SudokuSaveData.Aplatir(solutionGrid);
            }

            return data;
        }

        // Données sauvegardées, grille affichée
        private void Restaurer(SudokuSaveData data)
        {
            solutionGrid = SudokuSaveData.VersGrille(data.Solution);
            Cells.Clear();

            for (int i = 0; i < 81; i++)
            {
                bool isEditable = data.GrilleDepart[i] == 0;
                var cell = new SudokuCell(data.Valeurs[i], i / 9, i % 9, isEditable);

                foreach (int note in data.Notes[i])
                {
                    cell.Notes[note - 1].IsVisible = true;
                }

                AjouterCase(cell); // abonnement APRÈS avoir rempli la case : la restauration ne déclenche pas de sauvegarde
            }

            OnPropertyChanged(nameof(IsGridComplete));
        }

        // Sauvegarde à chaque coup. Si plusieurs coups arrivent pendant une sauvegarde
        private async Task SauvegarderAsync()
        {
            if (_sessionId == null) return;

            if (_sauvegardeEnCours)
            {
                _sauvegardeEnAttente = true;
                return;
            }

            _sauvegardeEnCours = true;
            try
            {
                do
                {
                    _sauvegardeEnAttente = false;
                    await _sessions.SauvegarderAsync(_sessionId.Value, CreerSaveData().VersJson());
                }
                while (_sauvegardeEnAttente);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de sauvegarde du Sudoku : {ex.Message}");
            }
            finally
            {
                _sauvegardeEnCours = false;
            }
        }

    }
}
