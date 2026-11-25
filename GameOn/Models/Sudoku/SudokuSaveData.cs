using System.Text.Json;

namespace GameOn.Models.Sudoku
{
    // État d'une partie de Sudoku, enregistré en JSON dans GameSession.GameData.
    // Les 81 cases sont rangées ligne par ligne : index = ligne * 9 + colonne.
    // La grille de départ et la solution ne sont pas ici : elles sont dans le DailyChallenge de la partie.
    public class SudokuSaveData
    {
        // Valeurs actuelles des cases (progression du joueur)
        public int[] Valeurs { get; set; } = new int[81];

        // Pour chaque case, les notes visibles (ex. : [2, 7])
        public int[][] Notes { get; set; } = Enumerable.Range(0, 81).Select(_ => Array.Empty<int>()).ToArray();

        public string VersJson() => JsonSerializer.Serialize(this);

        public static SudokuSaveData DepuisJson(string json)
            => JsonSerializer.Deserialize<SudokuSaveData>(json)
               ?? throw new InvalidOperationException("Sauvegarde de Sudoku illisible.");
    }
}