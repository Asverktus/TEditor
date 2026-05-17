using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextFileProcessor
{
  public class IndexBuilder
  {
    private Dictionary<string, List<IndexRecord>> _index;
    private string? _indexedDirectory;

    public IndexBuilder()
    {
      _index = new Dictionary<string, List<IndexRecord>>();
      _indexedDirectory = null;
    }

    public bool IsIndexBuilt
    {
      get
      {
        return _indexedDirectory != null && _index.Count > 0;
      }
    }

    public void BuildIndex(string directory, List<string> keywords, bool recursive = true)
    {
      if (!Directory.Exists(directory))
      {
        throw new Exception($"Directory not found: {directory}");
      }

      _index.Clear();
      _indexedDirectory = directory;

      SearchOption option;
      option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

      string[] files;
      files = Directory.GetFiles(directory, "*.txt", option);

      Console.WriteLine($"Indexing {files.Length} files for {keywords.Count} keywords...");

      foreach (string file in files)
      {
        try
        {
          string[] lines;
          lines = File.ReadAllLines(file);

          foreach (string keyword in keywords)
          {
            string lowerKeyword;
            lowerKeyword = keyword.ToLower();

            List<int> lineNumbers;
            lineNumbers = new List<int>();

            int lineNum;
            lineNum = 1;

            foreach (string line in lines)
            {
              if (line.ToLower().Contains(lowerKeyword))
              {
                lineNumbers.Add(lineNum);
              }

              ++lineNum;
            }

            if (lineNumbers.Count > 0)
            {
              IndexRecord record;
              record = new IndexRecord(file, lineNumbers);

              if (!_index.ContainsKey(keyword))
              {
                _index[keyword] = new List<IndexRecord>();
              }

              _index[keyword].Add(record);
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error indexing file {file}: {ex.Message}");
        }
      }

      Console.WriteLine($"Index built. {_index.Count} keywords indexed.");
    }

    public List<SearchResult> SearchFromIndex(string keyword)
    {
      List<SearchResult> results;
      results = new List<SearchResult>();

      if (!IsIndexBuilt)
      {
        Console.WriteLine("Index not built. Call BuildIndex first.");
        return results;
      }

      string lowerKeyword;
      lowerKeyword = keyword.ToLower();

      if (!_index.ContainsKey(lowerKeyword))
      {
        return results;
      }

      foreach (IndexRecord record in _index[lowerKeyword])
      {
        SearchResult result;
        result = new SearchResult(record.FilePath, keyword);

        foreach (int lineNum in record.LineNumbers)
        {
          result.AddOccurrence(lineNum);
        }

        results.Add(result);
      }

      return results;
    }

    public Dictionary<string, List<IndexRecord>> GetFullIndex()
    {
      return _index;
    }
  }

  public class IndexRecord
  {
    public string FilePath { get; private set; }
    public List<int> LineNumbers { get; private set; }

    public IndexRecord(string filePath, List<int> lineNumbers)
    {
      FilePath = filePath;
      LineNumbers = lineNumbers;
    }
  }
}