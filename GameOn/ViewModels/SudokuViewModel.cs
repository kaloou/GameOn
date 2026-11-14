using GameOn.Commande;
using GameOn.Games.Sudoku;
using GameOn.Models;
using GameOn.Models.Sudoku;
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
        public SudokuViewModel(Action retour, Difficulty difficulty)
        {
            Cells = new ObservableCollection<SudokuCell>();
            sudokuEngine = new SudokuEngine();

            // Si le mode est casual, passe la difficulté en easy
            Difficulty gridDifficulty = difficulty == Difficulty.CASUAL ? Difficulty.EASY : difficulty;

            // Chargement de la grille du sudoku (avec la difficulté sélectionnée)
            _ = LoadSudokuAsync(gridDifficulty);

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
                        cell.PropertyChanged += Cell_PropertyChanged;
                        Cells.Add(cell);
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
    }
}
