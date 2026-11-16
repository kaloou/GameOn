using GameOn.Models.Sudoku;
using GameOn.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOn.Views
{
    /// <summary>
    /// Logique d'interaction pour Sudoku.xaml
    /// </summary>
    public partial class SudokuView : UserControl
    {
        // Case actuelle sélectionnée par le joueur
        private SudokuCell? selectedCell;

        // Bordure de la case sélectionnée par le joueur
        private Border? selectedBorder;

        public SudokuView()
        {
            InitializeComponent();
        }

        // Autorisation des valeurs 1 à 9 uniquement dans les cases du Sudoku
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(e.Text.Length == 1 && e.Text[0] >= '1' && e.Text[0] <= '9');
        }

        // Récupération de la case sélectionnée pour le clavier
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                selectedCell = textBox.DataContext as SudokuCell;
            }
        }

        // Insertion d'une valeur dans la case sélectionnée
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCell != null && selectedCell.IsEditable)
            {
                if (sender is Button button && int.TryParse(button.Content?.ToString(), out int number))
                {
                    if (DataContext is SudokuViewModel viewModel)
                    {
                        if (viewModel.IsNoteMode)
                        {
                            viewModel.ManageNote(selectedCell, number);
                        }
                        else
                        {
                            selectedCell.CellValue = number;

                            foreach (SudokuNote note in selectedCell.Notes)
                            {
                                note.IsVisible = false;
                            }
                        }
                    }
                }
            }
        }

        // Suppression de l'intégralité des valeurs insérées par le joueur dans la grille
        private void ResetGrid_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel)
            {
                MessageBoxResult result = MessageBox.Show("Êtes-vous certain de vouloir réinitialiser la grille ?" +
                    "\nToutes vos modifications seront perdues !",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (SudokuCell cell in viewModel.Cells)
                    {
                        if (cell.IsEditable)
                        {
                            cell.CellValue = 0;
                        }
                    }
                }
            }
        }

        // Suppression de la valeur insérée par le joueur dans la case sélectionnée
        private void EraseCell_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel)
            {
                if (selectedCell != null && selectedCell.IsEditable)
                {
                    
                    if (selectedCell.CellValue == 0)
                    {
                        foreach (SudokuNote note in selectedCell.Notes)
                        {
                            note.IsVisible = false;
                        }
                    }
                    else
                    {
                        selectedCell.CellValue = 0;
                    }
                }
            }
        }

        // Confirmation de la grille (détermine si les valeurs insérées par le joueur sont les bonnes)
        private void ConfirmGrid_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel)
            {
                if (viewModel.IsGridValid())
                {
                    MessageBox.Show("Félicitations ! Grille validée !");
                }
                else
                {
                    MessageBox.Show("Grille invalide !");
                }
            }
        }

        // Affichage de la réponse de la case sélectionnée par le joueur (uniquement disponible pour le mode Casual en production)
        private void GiveCellAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel)
            {
                if (selectedCell != null && selectedCell.IsEditable)
                {
                    selectedCell.CellValue = viewModel.GiveCellAnswer(selectedCell);
                }
            }
        }

        // Changement de mode (mode note ou mode classique)
        private void SwitchNoteMode_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel)
            {
                viewModel.SwitchNoteMode();

                if (!viewModel.IsNoteMode && selectedBorder != null)
                {
                    selectedBorder.BorderBrush = Brushes.Black;
                    selectedBorder = null;
                }
            }
        }

        // Récupération de la case sélectionnée en mode notes
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid)
            {
                selectedCell = grid.DataContext as SudokuCell;

                if (DataContext is SudokuViewModel viewModel && viewModel.IsNoteMode)
                {
                    if (selectedBorder != null)
                    {
                        selectedBorder.BorderBrush = Brushes.Black;
                    }

                    selectedBorder = grid.Parent as Border;

                    if (selectedBorder != null)
                    {
                        selectedBorder.BorderBrush = Brushes.DodgerBlue;
                    }

                    grid.Focus();
                }
            }
        }

        // Gestion de la saisie des notes (autorisation des valeurs 1 à 9)
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel && viewModel.IsNoteMode)
            {
                if (selectedCell != null && selectedCell.IsEditable && selectedCell.CellValue == 0)
                {
                    int value = 0;

                    if (e.Key >= Key.D1 && e.Key <= Key.D9)
                    {
                        value = e.Key - Key.D0;
                    }
                    else if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9)
                    {
                        value = e.Key - Key.NumPad0;
                    }

                    if (value >= 1 && value <= 9)
                    {
                        viewModel.ManageNote(selectedCell, value);
                        e.Handled = true;
                    }
                }
            }
        }

        // [TEMPORAIRE] Affichage de la solution
        private void SolveGrid_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SudokuViewModel viewModel)
            {
                viewModel.SolveGrid();
            }
        }
    }
}
