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

        // Grille du jour pour une date et une difficulté : la même seed redonne toujours la même grille
        public static async Task<string> GetSudokuPuzzleAsync(Difficulty difficulty, DateTime date)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Clé API manquante : ajoute API_NINJAS_KEY dans le fichier .env");

            // L'API ne connaît que easy, medium et hard
            string niveau = difficulty switch
            {
                Difficulty.EASY or Difficulty.EASY_CASUAL => "easy",
                Difficulty.MEDIUM or Difficulty.MEDIUM_CASUAL => "medium",
                _ => "hard"
            };

            // Seed différente par jour ET par difficulté : une grille casual n'est jamais celle du classé
            int seed = (date.Year * 1000 + date.DayOfYear) * 10 + (int)difficulty;

            string url = $"https://api.api-ninjas.com/v1/sudokugenerate?difficulty={niveau}&width=3&height=3&seed={seed}";
            using var response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get Sudoku puzzle: {json}");

            return json;
        }
    }
}
