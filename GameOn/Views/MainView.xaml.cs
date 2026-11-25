using GameOn.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        // Vrai une fois la sauvegarde finale faite : la fenêtre peut alors vraiment se fermer
        private bool fermetureAutorisee;

        private async void Window_Closing(object? sender, CancelEventArgs e)
        {
            if (fermetureAutorisee) return;

            // On annule la fermeture le temps de sauvegarder, puis on referme
            e.Cancel = true;

            if (DataContext is MainViewModel vm)
            {
                await vm.QuitAsync();
            }

            fermetureAutorisee = true;
            Close();
        }

    }
}
