using System.Net.Http;
using GameOn.Models;

namespace GameOn.Games.Sudoku
{
    public static class SudokuApi
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string? apiKey = Environment.GetEnvironmentVariable("API_NINJAS_KEY");

        static SudokuApi()
        {
            
            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }
        
        public static async Task<string> GetSudokuPuzzleAsync(Difficulty difficulty)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Clé API manquante : ajoute API_NINJAS_KEY dans le fichier .env");

            int seed = DateTime.Today.Year * 1000 + DateTime.Today.DayOfYear;

            string url = $"https://api.api-ninjas.com/v1/sudokugenerate?difficulty={difficulty.ToString().ToLower()}&width=3&height=3&seed={seed}";
            using var response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get Sudoku puzzle: {error}");
            }
            
            return await response.Content.ReadAsStringAsync();
        }
    }
}