using System.Net.Http;
using GameOn.Models;

namespace GameOn.Games.Sudoku
{
    public static class SudokuApi
    {
    
        public static async Task<string> GetSudokuPuzzleAsync(Difficulty difficulty)
        {
            int seed = DateTime.UtcNow.Year * 1000 + DateTime.UtcNow.DayOfYear;
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", Environment.GetEnvironmentVariable("API_NINJAS_KEY"));

            string difficultyString = difficulty.ToString().ToLower();

            var response = await client.GetAsync($"https://api.api-ninjas.com/v1/sudokugenerate?difficulty={difficultyString}&width=3&height=3&seed={seed}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get Sudoku puzzle: {response.ReasonPhrase}");
            }
            
            return await response.Content.ReadAsStringAsync();
        }
    }
}


