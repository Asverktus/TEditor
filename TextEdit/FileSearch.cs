using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextFileProcessor
{
  public class SearchResult
  {
    public string filePath { get; private set; }
    public string fileName { get; private set; }
    public string keyword { get; private set; }
    public int occurrences { get; private set; }
    public List<int> lineNumbers { get; private set; }

    public SearchResult(string path, string keyword)
    {
      filePath = path;
      fileName = Path.GetFileName(path);
      this.keyword = keyword;
      occurrences = 0;
      lineNumbers = new List<int>();
    }

    public void AddOccurrence(int lineNumber)
    {
      ++occurrences;
      lineNumbers.Add(lineNumber);
    }

    public override string ToString()
    {
      string lineNumbersStr;
      lineNumbersStr = string.Join(", ", lineNumbers);

      return $"{fileName}: {occurrences} matches at lines {lineNumbersStr}";
    }
  }

  public class FileSearch
  {
    private List<string> _directories;
    private List<string> _extensions;

    public bool searchSubdirectories { get; set; } = true;
    public bool caseSensitive { get; set; } = false;

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
      option = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

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
      searchKeyword = caseSensitive ? keyword : keyword.ToLower();

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
            compareLine = caseSensitive ? line : line.ToLower();

            if (compareLine.Contains(searchKeyword))
            {
              result.AddOccurrence(lineNum);
            }

            ++lineNum;
          }

          if (result.occurrences > 0)
          {
            results.Add(result);
          }
        }
        catch
        {

        }
      }

      return results;
    }
  }
}