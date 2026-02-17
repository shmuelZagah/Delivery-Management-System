using Trie.Internal;
namespace TrieApi;


public class Trie : ITrie
{
    private readonly TrieNode _root;

    public Trie()
    {
        _root = new TrieNode();
    }

    public void Insert(string word, int? position = null)
    {
        TrieNode current = _root;
        foreach (char c in word)
        {
            if (!current.Children.TryGetValue(c, out TrieNode? nextNode))
            {
                nextNode = new TrieNode();
                current.Children[c] = nextNode;
            }

            current = nextNode;
        }

        current.IsEndOfWord = true;
        if (position != null)
        {
            if (!current.IndexInTheText.Contains((int)position))
                current.IndexInTheText.Add(position.Value);
        }

    }

    public bool Contains(string word)
    {
        if (_root.Children.Count() == 0) return false;
        var current = _root;
        foreach (char c in word)
        {
            if (!current.Children.TryGetValue(c, out current))
            {
                return false;
            }
        }

        return current.IsEndOfWord;
    }

    public IEnumerable<string> GetAllWithPrefix(string prefix)
    {
        var current = _root;
        foreach (char c in prefix)
        {
            if (!current.Children.TryGetValue(c, out current))
                yield break;
        }

        foreach (var result in GetAllFromNode(prefix, current))
        {
            yield return result;
        }
    }

    private IEnumerable<string> GetAllFromNode(string prefix, TrieNode current)
    {
        if (current.IsEndOfWord)
            yield return prefix;

        foreach (var childKey in current.Children.Keys)
        {
            foreach (var suffix in GetAllFromNode(prefix + childKey, current.Children[childKey]))
            {
                yield return suffix;
            }
        }
    }

    public IEnumerable<int> GetPositions(string word)
    {
        var current = _root;
        foreach (char c in word)
        {
            if (!current.Children.TryGetValue(c, out TrieNode? nextNode))
                return Enumerable.Empty<int>();

            current = nextNode;
        }

        return current.IndexInTheText;
    }

    public bool Delete(string word)
    {
        bool wasDeleted = false;
        Delete(word, _root, 0, ref wasDeleted);
        return wasDeleted;
    }

    private bool Delete(string word, TrieNode current, int index, ref bool wasDeleted)
    {
        //for delete we have 2 cases:
        //case 1: the word is part from other word, so we chang isEndOfWord to flase
        //case 2: the word is a leaf we need to delete up to the first other word

        if (index == word.Length)
        {
            if (!current.IsEndOfWord)
                return false;

            current.IsEndOfWord = false;
            wasDeleted = true;

            return current.Children.Count() == 0;
        }

        char currentKey = word[index];

        //If we cant find the key go out
        if (!current.Children.TryGetValue(currentKey, out TrieNode? childNode))
        {
            return false;
        }

        if (Delete(word, childNode, index + 1, ref wasDeleted))
        {
            current.Children.Remove(currentKey);

            return current.Children.Count == 0 && !current.IsEndOfWord;
        }

        return false;
    }

    public IEnumerable<string> GetAll()
    => GetAllWithPrefix("");

    public void Reset(IEnumerable<string> str)
    {
        _root.Children.Clear();

        foreach (string word in str)
        {
            Insert(word);
        }
    }

    public void Reset(IEnumerable<KeyValuePair<string, int>> str)
    {
        _root.Children.Clear();
       
        foreach (var pair in str)
        {
            Insert(pair.Key, pair.Value);
        }
    }

    public void Clear()
    {
        _root.Children.Clear();
    }
}
