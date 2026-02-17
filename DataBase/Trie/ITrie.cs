namespace TrieApi;

public interface ITrie
{
    void Insert(string data, int? position = null);
    bool Delete(string data);
    bool Contains(string data);
    IEnumerable<string> GetAllWithPrefix(string prefix);
    IEnumerable<string> GetAll();
    IEnumerable<int> GetPositions(string word);

    void Reset(IEnumerable<KeyValuePair<string, int>> str);
    void Reset(IEnumerable<string> str);
    void Clear();
}
