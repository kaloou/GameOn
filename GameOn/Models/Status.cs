using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls.Primitives;

namespace GameOn.Models
{
    public enum Status
    {
        // Status when the game was started by the employee
        STARTED,

        // Status when the game was completed by the employee (for exemple : the grid is correct for a sudoku)
        COMPLETED,

        // Status when the game was started by the employee, but was not completed within the time limit (midnight for the dailly challenge)
        EXPIRED,

    }
}
