using System.Text.Json;

namespace GameOn.Models.Sudoku
{
    // État d'une partie de Sudoku, enregistré en JSON dans GameSession.GameData.
    // Les 81 cases sont rangées ligne par ligne : index = ligne * 9 + colonne.
    public class SudokuSaveData
    {
        public int[] GrilleDepart { get; set; } = new int[81];

        public int[] Solution { get; set; } = new int[81];

        // Valeurs actuelles des cases (progression du joueur)
        public int[] Valeurs { get; set; } = new int[81];

        // Pour chaque case, les notes visibles (ex. : [2, 7])
        public int[][] Notes { get; set; } = Enumerable.Range(0, 81).Select(_ => Array.Empty<int>()).ToArray();

        public string VersJson() => JsonSerializer.Serialize(this);

        public static SudokuSaveData DepuisJson(string json)
            => JsonSerializer.Deserialize<SudokuSaveData>(json)
               ?? throw new InvalidOperationException("Sauvegarde de Sudoku illisible.");

        // int[9,9] du moteur -> int[81] (System.Text.Json ne sait pas convertir les int[,])
        public static int[] Aplatir(int[,] grille)
        {
            int[] resultat = new int[81];
            for (int ligne = 0; ligne < 9; ligne++)
                for (int colonne = 0; colonne < 9; colonne++)
                    resultat[ligne * 9 + colonne] = grille[ligne, colonne];
            return resultat;
        }

        // int[81] -> int[9,9], pour redonner au ViewModel le format qu'il utilise
        public static int[,] VersGrille(int[] cases)
        {
            int[,] resultat = new int[9, 9];
            for (int i = 0; i < 81; i++)
                resultat[i / 9, i % 9] = cases[i];
            return resultat;
        }
    }
}