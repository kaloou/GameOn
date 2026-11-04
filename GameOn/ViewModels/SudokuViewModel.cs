using GameOn.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GameOn.ViewModels
{
    internal class SudokuViewModel
    {
        // Collection qui contiendra les 81 cases du Sudoku
        public ObservableCollection<SudokuCell> Cells { get; set; }

        private SudokuEngine sudokuEngine;

        public SudokuViewModel()
        {
            Cells = new ObservableCollection<SudokuCell>();
            sudokuEngine = new SudokuEngine();
            _ = LoadSudokuAsync();
        }

        public async Task LoadSudokuAsync()
        {
            try
            {
                int[,] grid = await sudokuEngine.GenerateSudokuPuzzleAsync(Difficulty.EASY);
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
                Console.WriteLine($"Erreur dans l'affichage du Sudoku : {ex.Message}");
            }
        }
    }
}
