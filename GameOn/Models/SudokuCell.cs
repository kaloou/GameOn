using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameOn.Models
{
    internal class SudokuCell : INotifyPropertyChanged
    {
        // Valeur de la case (entre 1 et 9)
        private int cellValue;

		public int CellValue
		{
			get { return cellValue; }
			set
			{
				if (value <= 0 || value >= 10)
				{
					throw new ArgumentOutOfRangeException("The cell value must be between 1 and 9 !");
				}

				if (value != cellValue)
				{
                    cellValue = value;
					OnPropertyChanged();
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
			get { return row; }
			set
			{
                if (value < 0 || value > 2)
                {
                    throw new ArgumentOutOfRangeException("The block row of the cell must be between 0 and 2 !");
                }

                row = value;
			}
		}

        // Colonne du bloc de la case (entre 0 et 2) => C'est-à-dire, la position horizontale du bloc dans lequel se trouve la case
        private int blockColumn;

		public int BlockColumn
		{
			get { return column; }
			set
			{
                if (value < 0 || value > 2)
                {
                    throw new ArgumentOutOfRangeException("The block column of the cell must be between 0 and 2 !");
                }

                column = value;
			}
		}

		// Booléen qui définit si la case est modifiable ou non
		private bool isEditable;

		public bool IsEditable
		{
			get { return isEditable; }
			set { isEditable = value; }
		}

		

        public SudokuCell(int cellValue, int row, int column, bool isEditable)
        {
			CellValue = cellValue;
			Row = row;
			Column = column;
			IsEditable = isEditable;
			BlockRow = Row / 3;
			BlockColumn = Column / 3;
        }

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
