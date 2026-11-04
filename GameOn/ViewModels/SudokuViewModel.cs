using GameOn.Commande;
using GameOn.Games.Sudoku;
using GameOn.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // Initialisation des cases, du moteur sudoku et de la commande de retour
        public SudokuViewModel(Action retour)
        {
            Cells = new ObservableCollection<SudokuCell>();
            sudokuEngine = new SudokuEngine();

            BackCommand = new RelaisCommande(retour);

            // Chargement de la grille du sudoku
            _ = LoadSudokuAsync();
        }

        // Récupèration de la grille depuis le moteur Sudoku
        public async Task LoadSudokuAsync()
        {
            try
            {
                int[,] grid = await sudokuEngine.GetSudokuBaseGridAsync(Difficulty.EASY);
                Cells.Clear();

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        int value = grid[row, col];
                        bool isEditable = value == 0;

                        SudokuCell cell = new SudokuCell(value, row, col, isEditable);
                        Cells.Add(cell);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur dans l'affichage du Sudoku : {ex.Message}");
            }
        }
    }
}
