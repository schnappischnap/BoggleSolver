using System.Collections.Generic;
using System.IO;

namespace SimpleTrie
{
    public class Trie
    {
        private class Node
        {
            public char Value { get; set; }
            public bool IsWord { get; set; }
            public Dictionary<char, Node> Children { get; set; } = new Dictionary<char, Node>();
            public bool IsPrefix => Children.Count > 0;
        }

        private readonly Node root = new Node();

        public Trie(string filepath)
        {
            foreach(string line in File.ReadLines(filepath))
            {
                AddWord(line);
            }
        }

        public Trie (IEnumerable<string> words)
        {
            foreach(string word in words)
            {
                AddWord(word);
            }
        }

        /// <summary>
        /// Adds a word to the trie.
        /// </summary>
        /// <param name="word">The word to add.</param>
        private void AddWord(string word)
        {
            Node currentNode = root;
            word = word.ToLower();

            for (int i = 0; i < word.Length; ++i)
            {
                char letter = word[i];
                Node child;

                if (!currentNode.Children.TryGetValue(letter, out child))
                {
                    child = new Node();
                    child.Value = letter;

                    currentNode.Children.Add(letter, child);
                }

                if (i == word.Length - 1)
                    child.IsWord = true;

                currentNode = child;
            }
        }

        /// <summary>
        /// Gets the node assossiated with the specified word.
        /// </summary>
        /// <param name="word">The string to search for.</param>
        /// <param name="node">The node to be returned.</param>
        /// <returns>True if the node was found, else false.</returns>
        private bool TryGetNode(string word, out Node node)
        {
            Node currentNode = root;

            foreach (char c in word)
            {
                if (!currentNode.Children.TryGetValue(c, out currentNode))
                {
                    node = null;
                    return false;
                }
            }

            node = currentNode;
            return true;
        } 

        /// <summary>
        /// Returns true if the specified string is a prefix to a word in the trie.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>True if the string is a prefix, else false.</returns>
        public bool IsPrefix(string str)
        {
            Node node;
            if (!TryGetNode(str, out node))
                return false;

            return node.IsPrefix;
        }

        /// <summary>
        /// Returns true if the specified string is word in the trie.
        /// </summary>
        /// <param name="str">The string to test.</param>
        /// <returns>True if the string is a word, else false.</returns>
        public bool IsWord(string str)
        {
            Node node;
            if (!TryGetNode(str, out node))
                return false;

            return node.IsWord;
        }
    }
}