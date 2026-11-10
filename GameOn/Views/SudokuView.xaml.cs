using GameOn.Models;
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

        private SudokuCell? selectedCell;

        public SudokuView()
        {
            InitializeComponent();
        }

        // Autorisation des valeurs 1 à 9 uniquement
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(e.Text[0] >= '1' && e.Text[0] <= '9');
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
            if (selectedCell == null || !selectedCell.IsEditable)
            {
                return;
            }

            if (sender is Button button && int.TryParse(button.Content?.ToString(), out int number))
            {
                selectedCell.CellValue = number;
            }
        }
    }
}
