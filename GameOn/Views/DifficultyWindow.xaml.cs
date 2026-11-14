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
using System.Windows.Shapes;

namespace GameOn.Views
{
    /// <summary>
    /// Logique d'interaction pour DifficultyWindow.xaml
    /// </summary>
    public partial class DifficultyWindow : Window
    {
        // Difficulté choisie par le joueur
        public Difficulty SelectedDifficulty { get; private set; }

        public DifficultyWindow()
        {
            InitializeComponent();
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (DifficultyComboBox.SelectedItem is ComboBoxItem item &&
                Enum.TryParse(item.Tag?.ToString(), out Difficulty difficulty))
            {
                SelectedDifficulty = difficulty;
                DialogResult = true;
            }
        }
    }
}
