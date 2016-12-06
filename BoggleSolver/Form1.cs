using System;
using System.Windows.Forms;
using SchnappiSchnap.Solvers;
using SimpleTrie;

namespace Program
{
    public partial class Form1 : Form
    {
        private Trie trie;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trie = new Trie("Wordlist.txt");
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Prevent any non-alphabetic characters being entered.
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void inputTextBox_Leave(object sender, EventArgs e)
        {
            string finalString = "";

            // Remove any non-alphabetic characters.
            foreach (char c in inputTextBox.Text)
            {
                if (char.IsLetter(c))
                    finalString += c;
            }

            inputTextBox.Text = finalString;

            UpdateBoardUI();
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateBoardUI();   
        }

        private void solveButton_Click(object sender, EventArgs e)
        {
            // Check the trie was loaded.
            if (trie == null)
            {
                toolStripStatusLabel.Text = "Dictionary not loaded!";
                return;
            }

            // Check the input is valid.
            if (inputTextBox.Text.Length != 16)
            {
                toolStripStatusLabel.Text = "Invalid input!";
                return;
            }

            SolverResults results = SolveBoard(inputTextBox.Text);

            DisplayResults(results);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            inputTextBox.Text = "";
            outputTextBox.Text = "";
            toolStripStatusLabel.Text = "";
        }

        /// <summary>
        /// Fills the boardTableLayoutPanel's textboxes with input from 
        /// inputTextBox.
        /// </summary>
        private void UpdateBoardUI()
        {
            string inputText = inputTextBox.Text;

            for (int i = 0; i < 16; ++i)
            {
                int x = i % 4;
                int y = i / 4;

                Control textBox = boardTableLayoutPanel.GetControlFromPosition(x, y);

                string letter;
                if (i < inputText.Length)
                {
                    letter = inputText[i].ToString();

                    // There is no 'Q' in Boggle, only 'QU'.
                    if (letter.ToUpper() == "Q")
                        letter = "Qu";
                }
                else
                    letter = "";

                textBox.Text = letter;
            }
        }

        /// <summary>
        /// Organises the input, solve the board and returns the results.
        /// </summary>
        /// <param name="input">The 16-character Boggle board string.</param>
        /// <returns>The results of the Boggle solver.</returns>
        SolverResults SolveBoard(string input)
        {
            string[,] board = new string[4, 4];

            for (int i = 0; i < 16; ++i)
            {
                int x = i % 4;
                int y = i / 4;

                string letter = input[i].ToString().ToUpper();
                // There is no 'Q' in Boggle, only 'QU'.
                if (letter == "Q")
                    letter = "QU";

                board[y, x] = letter;
            }

            BoggleSolver solver = new BoggleSolver(trie, board);
            return solver.Solve();
        }

        /// <summary>
        /// Fills in controls with Boggle results.
        /// </summary>
        /// <param name="results">The results of the Boggle solver.</param>
        void DisplayResults(SolverResults results)
        {
            int length = results.Words[0].Length;
            string outputText = $"{length}-LETTER WORDS";
            foreach (string word in results.Words)
            {
                if (word.Length < length)
                {
                    length = word.Length;
                    outputText += Environment.NewLine + Environment.NewLine + $"{length}-LETTER WORDS";
                }

                outputText += Environment.NewLine + word;
            }

            outputTextBox.Text = outputText;
            toolStripStatusLabel.Text = $"Score: {results.Score}   Words: {results.Words.Length}";
        }
    }
}
