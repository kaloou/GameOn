using DotNetEnv;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameOn.Models.Sudoku
{
    public class SudokuNote : INotifyPropertyChanged
    {
        // Valeur de la note (entre 1 et 9)
        private int noteValue;

        public int NoteValue
        {
            get { return noteValue; }
        }


        // Booléen qui détermine si la note est visible ou non
        private bool isVisible;

		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				if (isVisible != value)
				{
					isVisible = value;
					OnPropertyChanged();
				}
			}
		}

        // Évènement qui permet de notifier l'interface lorsqu'une propriété est modifiée (en l'occurence, la valeur de la cellule)
        public event PropertyChangedEventHandler? PropertyChanged;

        // Méthode qui permet de déclancher l'évènement PropertyChanged pour la propriété concernée (en l'occurence, la valeur de la cellule)
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		// Constructeur des notes
        public SudokuNote(int noteValue)
        {
            if (noteValue <= 0 || noteValue >= 10)
            {
                throw new ArgumentOutOfRangeException("The note value must be between 1 and 9 !");
            }

            this.noteValue = noteValue;
            IsVisible = false;
        }
    }
}
