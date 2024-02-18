namespace Aho_Corasick
{
    /*
        Implementation of the Aho-Corasick algorithm for efficient pattern matching in text processing.
        Includes definitions for the base Trie node and Trie data structure to facilitate pattern matching operations.

        https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm
    */

    /// <summary>
    /// Base Trie node
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="c"></param>
    internal class TrieNode(TrieNode? parent = null, char c = ' ')
    {
        public Dictionary<char, TrieNode> Children = [];
        public TrieNode? Failure { get; set; }
        public List<string> Outputs = [];
        public TrieNode? Parent { get; set; } = parent;
        public char Char { get; set; } = c;
    }

    /// <summary>
    /// Trie is a type of k-ary search tree, a tree data structure used for locating specific keys from within a set
    /// https://en.wikipedia.org/wiki/Trie
    /// </summary>
    public class Trie
    {
        private readonly TrieNode root = new();

        /// <summary>
        /// Parse search words chars into tree reprsentations
        /// After search words are added determine link failures BuildFailure()
        /// </summary>
        /// <param name="keyword"></param>
        public void Add(string keyword)
        {
            var node = root;
            foreach (var c in keyword)
            {
                if (!node.Children.TryGetValue(c, out TrieNode? value))
                {
                    value = new TrieNode(node, c);
                    node.Children[c] = value;
                }
                node = value;
            }
            node.Outputs.Add(keyword);
        }

        /// <summary>
        /// Creating failure links between nodes that share common substrings, enabling fast skipping from one match to the next
        /// Do not skip this step
        /// </summary>
        public void BuildFailure()
        {
            var queue = new Queue<TrieNode>();
            foreach (var child in root.Children.Values)
            {
                child.Failure = root;
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var childPair in current.Children)
                {
                    var child = childPair.Value;
                    var failure = current.Failure;
                    while (failure != null && !failure.Children.ContainsKey(childPair.Key))
                    {
                        failure = failure.Failure;
                    }
                    child.Failure = failure == null ? root : failure.Children[childPair.Key];
                    child.Outputs.AddRange(child.Failure.Outputs);
                    queue.Enqueue(child);
                }
            }
        }


        /// <summary>
        /// Performs DFS search in O(n + z)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<string> Search(string text)
        {
            var result = new List<string>();
            var current = root;
            foreach (var c in text)
            {
                while (current != null && !current.Children.ContainsKey(c))
                    current = current.Failure;
                
                if (current == null)
                {
                    current = root;
                    continue;
                }
                current = current.Children[c];
                result.AddRange(current.Outputs);
            }
            return result;
        }

    }

}
