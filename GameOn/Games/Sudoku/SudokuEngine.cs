using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using GameOn.Models;

namespace GameOn.Games.Sudoku;

public class SudokuEngine
{
    public async Task<int[,]> GenerateSudokuPuzzleAsync(Difficulty difficulty)
    {
        var apiResponse = await SudokuApi.GetSudokuPuzzleAsync(difficulty);

        var sudoku_grid = JsonToList(apiResponse);
        
        return sudoku_grid;
    }
    
    public int [,] JsonToList(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var sudoku_grid = new int[9, 9];

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (root[i][j].ValueKind == JsonValueKind.Null)
                    sudoku_grid[i, j] = 0; // 0 = empty cell
                else
                    sudoku_grid[i, j] = root[i][j].GetInt32();
            }
        }

        return sudoku_grid;

    }
}
