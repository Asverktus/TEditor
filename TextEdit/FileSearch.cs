using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextFileProcessor
{
  public class SearchResult
  {
    public string FilePath { get; private set; }
    public string FileName { get; private set; }
    public string Keyword { get; private set; }
    public int Occurrences { get; private set; }
    public List<int> LineNumbers { get; private set; }

    public SearchResult(string path, string keyword)
    {
      FilePath = path;
      FileName = Path.GetFileName(path);
      Keyword = keyword;
      Occurrences = 0;
      LineNumbers = new List<int>();
    }

    public void AddOccurrence(int lineNumber)
    {
      ++Occurrences;
      LineNumbers.Add(lineNumber);
    }

    public override string ToString()
    {
      string lineNumbersStr;
      lineNumbersStr = string.Join(", ", LineNumbers);

      return $"{FileName}: {Occurrences} matches at lines {lineNumbersStr}";
    }
  }

  public class FileSearch
  {
    private List<string> _directories;
    private List<string> _extensions;

    private bool _searchSubdirectories;
    private bool _caseSensitive;

    public bool SearchSubdirectories
    {
      get
      {
        return _searchSubdirectories;
      }

      set
      {
        _searchSubdirectories = value;
      }
    }

    public bool CaseSensitive
    {
      get
      {
        return _caseSensitive;
      }

      set
      {
        _caseSensitive = value;
      }
    }

    public FileSearch()
    {
      _directories = new List<string>();
      _extensions = new List<string>();
    }

    public void AddDirectory(string path)
    {
      if (!Directory.Exists(path))
      {
        throw new Exception($"Directory not found: {path}");
      }

      if (!_directories.Contains(path))
      {
        _directories.Add(path);
      }
    }

    public void AddExtension(string ext)
    {
      string normExt;
      normExt = ext.StartsWith(".") ? ext : "." + ext;

      if (!_extensions.Contains(normExt))
      {
        _extensions.Add(normExt);
      }
    }

    private IEnumerable<string> GetFiles()
    {
      List<string> files;
      files = new List<string>();

      SearchOption option;
      option = SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

      foreach (string dir in _directories)
      {
        if (_extensions.Count == 0)
        {
          string[] dirFiles;
          dirFiles = Directory.GetFiles(dir, "*.*", option);
          files.AddRange(dirFiles);
        }

        else
        {
          foreach (string ext in _extensions)
          {
            string[] dirFiles;
            dirFiles = Directory.GetFiles(dir, "*" + ext, option);
            files.AddRange(dirFiles);
          }
        }
      }

      IEnumerable<string> distinctFiles;
      distinctFiles = files.Distinct();

      return distinctFiles;
    }

    public List<SearchResult> Search(string keyword)
    {
      List<SearchResult> results;
      results = new List<SearchResult>();

      if (string.IsNullOrWhiteSpace(keyword))
      {
        return results;
      }

      string searchKeyword;
      searchKeyword = CaseSensitive ? keyword : keyword.ToLower();

      foreach (string file in GetFiles())
      {
        try
        {
          string[] lines;
          lines = File.ReadAllLines(file);

          SearchResult result;
          result = new SearchResult(file, keyword);

          int lineNum;
          lineNum = 1;

          foreach (string line in lines)
          {
            string compareLine;
            compareLine = CaseSensitive ? line : line.ToLower();

            if (compareLine.Contains(searchKeyword))
            {
              result.AddOccurrence(lineNum);
            }

            ++lineNum;
          }

          if (result.Occurrences > 0)
          {
            results.Add(result);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error reading file {file}: {ex.Message}");
        }
      }

      return results;
    }
  }
}