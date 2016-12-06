using System;
using System.Collections.Generic;
using SimpleTrie;

namespace SchnappiSchnap.Solvers
{
    struct SolverResults
    {
        public string[] Words { get; }
        public int Score { get; }

        public SolverResults(string[] words, int score)
        {
            Words = words;
            Score = score;
        }
    }

    class BoggleSolver
    {
        private struct Coordinate
        {
            public int X { get; }
            public int Y { get; }

            public Coordinate(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        
        private readonly Trie trie;
        private readonly string[,] board;
        private HashSet<string> words = new HashSet<string>();
        private int score;

        public BoggleSolver(Trie trie, string[,] board)
        {
            this.trie = trie;

            this.board = board;
            for(int x = 0; x < this.board.GetLength(0); ++x)
            {
                for (int y = 0; y < this.board.GetLength(1); ++y)
                {
                    this.board[y, x] = this.board[y, x].ToLower();
                }
            }
        }

        /// <summary>
        /// Finds all valid words in the Boggle board.
        /// </summary>
        /// <returns>A struct containing the results of the search.</returns>
        public SolverResults Solve()
        {
            for (int x = 0; x < board.GetLength(0); ++x)
            {
                for (int y = 0; y < board.GetLength(1); ++y)
                {
                    string letter = board[y, x];
                    Coordinate point = new Coordinate(x, y);

                    Solve(letter, new List<Coordinate> { point });
                }
            }

            string[] finalWords = new string[words.Count];
            words.CopyTo(finalWords);
            Array.Sort(finalWords, (a, b) => b.Length.CompareTo(a.Length));

            return new SolverResults(finalWords, score);
        }

        /// <summary>
        /// Recursively solves the Boggle board, given a starting point.
        /// </summary>
        /// <param name="letters">The prefix of the word to solve.</param>
        /// <param name="visited">The coordinates of the letters already used.</param>
        private void Solve(string letters, List<Coordinate> visited)
        {
            Coordinate currentPoint = visited[visited.Count - 1];

            // To be valid in Boggle, a word has to be 3 letters or longer.
            if (letters.Length > 2 && trie.IsWord(letters))
            {
                words.Add(letters);
                score += GetScore(letters);
            }

            // Stop looking for words starting with newLetters if no such
            // word exists.
            if (!trie.IsPrefix(letters))
                return;

            foreach (Coordinate neighbour in GetNeighbours(currentPoint))
            {
                // You cannot use the same letter cube more than one in a word.
                if (visited.Contains(neighbour))
                    continue;

                visited.Add(neighbour);
                Solve(letters + board[neighbour.Y, neighbour.X], visited);
                visited.Remove(neighbour);
            }
        }

        /// <summary>
        /// Finds the Boggle score of the specified word.
        /// </summary>
        /// <param name="word">The word to score.</param>
        /// <returns>The score of the word.</returns>
        private int GetScore(string word)
        {
            if (word.Length <= 4)
                return 1;
            else if (word.Length <= 5)
                return 2;
            else if (word.Length <= 6)
                return 3;
            else if (word.Length <= 7)
                return 5;
            else
                return 11;
        }

        /// <summary>
        /// Returns the coordinates one space away vertically, horizontally and diagonally. 
        /// </summary>
        /// <param name="point">The coordinate to find the neighbours of.</param>
        /// <returns>An IEnumberable of neighbour coordinates.</returns>
        private IEnumerable<Coordinate> GetNeighbours(Coordinate point)
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    // Don't pick the point itself. 
                    if (i == 0 && j == 0)
                        continue;

                    int col = point.X + j;
                    int row = point.Y + i;

                    // Don't pick a coordinate off the board.
                    if (col < 0 || col > (board.GetLength(0) - 1) || row < 0 || row > (board.GetLength(1) - 1))
                        continue;

                    yield return new Coordinate(col, row);
                }
            }
        }
    }
}
