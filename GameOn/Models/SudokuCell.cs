using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace GameOn.Models
{
    public class SudokuCell : INotifyPropertyChanged
    {
        // Valeur de la case (entre 0 et 9), avec 0 uniquement comme valeur de case vide
        private int cellValue;

		public int CellValue
		{
			get { return cellValue; }
			set
			{
				if (value < 0 || value >= 10)
				{
					throw new ArgumentOutOfRangeException("The cell value must be between 0 and 9, with 0 only for the empty cells !");
				}

				if (value != cellValue)
				{
                    cellValue = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(DisplayValue));
                }
			}
		}

        // Ligne de la case (entre 0 et 8)
        private int row;

		public int Row
		{
			get { return row; }
			set
			{
                if (value < 0 || value > 8)
                {
                    throw new ArgumentOutOfRangeException("The cell row must be between 0 and 8 !");
                }

                row = value;
			}
		}

        // Colonne de la case (entre 0 et 8)
        private int column;

		public int Column
		{
			get { return column; }
			set
			{
                if (value < 0 || value > 8)
                {
                    throw new ArgumentOutOfRangeException("The cell column must be between 0 and 8 !");
                }

                column = value;
			}
		}

        // Ligne du bloc de la case (entre 0 et 2) => C'est-à-dire, la position verticale du bloc dans lequel se trouve la case
        private int blockRow;

		public int BlockRow
		{
			get { return blockRow; }
			set
			{
                if (value < 0 || value > 2)
                {
                    throw new ArgumentOutOfRangeException("The block row of the cell must be between 0 and 2 !");
                }

                blockRow = value;
			}
		}

        // Colonne du bloc de la case (entre 0 et 2) => C'est-à-dire, la position horizontale du bloc dans lequel se trouve la case
        private int blockColumn;

		public int BlockColumn
		{
			get { return blockColumn; }
			set
			{
                if (value < 0 || value > 2)
                {
                    throw new ArgumentOutOfRangeException("The block column of the cell must be between 0 and 2 !");
                }

                blockColumn = value;
			}
		}

		// Booléen qui définit si la case est modifiable ou non
		private bool isEditable;

		public bool IsEditable
		{
			get { return isEditable; }
			set { isEditable = value; }
		}

		// Évènement qui permet de notifier l'interface lorsqu'une propriété est modifiée (en l'occurence, la valeur de la cellule)
		public event PropertyChangedEventHandler? PropertyChanged;

        // Méthode qui permet de déclancher l'évènement PropertyChanged pour la propriété concernée (en l'occurence, la valeur de la cellule)
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		// Valeur affichée dans la grille (les cases 0 sont vides)
		public string DisplayValue
		{
			get
			{
				return CellValue == 0 ? "" : CellValue.ToString();
			}
		}

		// Épaisseur des bordures, configurée pour séparer les blocs de 3 x 3
		public Thickness BorderThickness
		{
			get
			{
				return new Thickness(
					Column % 3 == 0 ? 3 : 1, // Bordure de gauche
					Row % 3 == 0 ? 3 : 1,    // Bordure du haut
					1,                       // Bordure de droite
					1                        // Bordure du bas
				);
			}
		}

		// Constructeur d'une case
        public SudokuCell(int cellValue, int row, int column, bool isEditable)
        {
            CellValue = cellValue;
            Row = row;
            Column = column;
            IsEditable = isEditable;
            BlockRow = Row / 3;
            BlockColumn = Column / 3;
        }
    }
}
