namespace Trie.Internal;

internal class TrieNode
{
    internal bool IsEndOfWord { get; set; } = false;
    internal Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();
    internal List<int> IndexInTheText { get; set; } = new List<int>();
}


