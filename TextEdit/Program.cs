using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TextFileProcessor
{
  public enum MenuCommand
  {
    Exit = 0,
    OpenFile = 1,
    NewFile = 2,
    EditContent = 3,
    AppendText = 4,
    Undo = 5,
    Redo = 6,
    Save = 7,
    ShowContent = 8,
    SearchFiles = 9,
    IndexFiles = 10,
    SaveAsBinary = 11,
    LoadBinary = 12,
    SaveAsXml = 13,
    LoadXml = 14,
    BuildIndex = 15
  }

  class Program
  {
    private static IndexBuilder? _indexBuilder;

    static void Main()
    {
      TEditor editor;
      editor = new TEditor();

      FileSearch searcher;
      searcher = new FileSearch();

      _indexBuilder = new IndexBuilder();

      string currentDir;
      currentDir = Directory.GetCurrentDirectory();

      searcher.AddDirectory(currentDir);
      searcher.AddExtension(".txt");

      Console.WriteLine(
        $"WORKING DIRECTORY: {currentDir}\n" +
        $"Files will be saved here if you don't specify a full path\n" +
        $"Type 0 to exit.\n"
      );

      while (true)
      {
        Console.WriteLine(
          "\n=== TEXT EDITOR ===\n" +
          "1. Open file\n" +
          "2. New file\n" +
          "3. Edit content\n" +
          "4. Append text\n" +
          "5. Undo\n" +
          "6. Redo\n" +
          "7. Save (shows full path)\n" +
          "8. Show content\n" +
          "9. Search files (direct)\n" +
          "10. Index files (direct search)\n" +
          "11. Save as Binary\n" +
          "12. Load Binary\n" +
          "13. Save as XML\n" +
          "14. Load XML\n" +
          "15. Build index (fast search)\n" +
          "0. Exit"
        );

        Console.Write("Choose: ");
        string? input;
        input = Console.ReadLine();

        int choice;
        bool parseResult;
        parseResult = int.TryParse(input, out choice);

        if (!parseResult)
        {
          Console.WriteLine("Invalid number");
          continue;
        }

        if (choice == (int)MenuCommand.Exit)
        {
          break;
        }

        try
        {
          MenuCommand selectedCommand;
          selectedCommand = (MenuCommand)choice;

          switch (selectedCommand)
          {
            case MenuCommand.OpenFile:
              Console.Write("Path: ");
              string? openPath;
              openPath = Console.ReadLine();

              if (string.IsNullOrWhiteSpace(openPath))
              {
                Console.WriteLine("Path cannot be empty");
                break;
              }

              editor.OpenFile(openPath);
              break;

            case MenuCommand.NewFile:
              Console.Write("Path: ");
              string? newPath;
              newPath = Console.ReadLine();

              if (string.IsNullOrWhiteSpace(newPath))
              {
                Console.WriteLine("Path cannot be empty");
                break;
              }

              Console.Write("Initial content (optional): ");
              string? initContent;
              initContent = Console.ReadLine();

              editor.CreateFile(newPath, initContent ?? "");
              break;

            case MenuCommand.EditContent:
              Console.Write("New content: ");
              string? newContent;
              newContent = Console.ReadLine();

              editor.EditContent(newContent ?? "");
              break;

            case MenuCommand.AppendText:
              Console.Write("Text to append: ");
              string? appendText;
              appendText = Console.ReadLine();

              editor.AppendText(appendText ?? "");
              break;

            case MenuCommand.Undo:
              bool undoResult;
              undoResult = editor.Undo();
              Console.WriteLine(undoResult ? "Undo successful" : "Nothing to undo");
              break;

            case MenuCommand.Redo:
              bool redoResult;
              redoResult = editor.Redo();
              Console.WriteLine(redoResult ? "Redo successful" : "Nothing to redo");
              break;

            case MenuCommand.Save:
              editor.Save();
              break;

            case MenuCommand.ShowContent:
              editor.ShowContent();
              break;

            case MenuCommand.SearchFiles:
              Console.Write("Keyword: ");
              string? keyword;
              keyword = Console.ReadLine();

              if (string.IsNullOrWhiteSpace(keyword))
              {
                Console.WriteLine("Keyword cannot be empty");
                break;
              }

              List<SearchResult> results;
              results = searcher.Search(keyword);

              if (results.Any())
              {
                string resultMessage;
                resultMessage = $"\nFound {results.Count} files with '{keyword}':";

                foreach (SearchResult result in results)
                {
                  resultMessage = resultMessage + $"\n   {result}";
                }

                Console.WriteLine(resultMessage);
              }
              else
              {
                Console.WriteLine("No files found");
              }
              break;

            case MenuCommand.IndexFiles:
              Console.Write("Keyword to index: ");
              string? indexKeyword;
              indexKeyword = Console.ReadLine();

              if (string.IsNullOrWhiteSpace(indexKeyword))
              {
                Console.WriteLine("Keyword cannot be empty");
                break;
              }

              List<SearchResult> indexed;
              indexed = searcher.Search(indexKeyword);

              string indexMessage;
              indexMessage = $"\nINDEX RESULTS FOR '{indexKeyword}':";

              if (indexed.Any())
              {
                foreach (SearchResult result in indexed)
                {
                  indexMessage = indexMessage + $"\n   {result.FileName} - {result.Occurrences} matches";
                }
              }
              else
              {
                indexMessage = indexMessage + "\nNo matches found";
              }

              Console.WriteLine(indexMessage);
              break;

            case MenuCommand.SaveAsBinary:
              if (!editor.IsDocumentOpen)
              {
                Console.WriteLine("No document open");
                break;
              }

              Console.Write("Enter binary file path to save: ");
              string? binaryPath;
              binaryPath = Console.ReadLine();

              Console.WriteLine("Binary serialization not fully implemented in this demo");
              break;

            case MenuCommand.LoadBinary:
              Console.Write("Enter binary file path to load: ");
              string? loadBinaryPath;
              loadBinaryPath = Console.ReadLine();

              Console.WriteLine("Binary deserialization not fully implemented in this demo");
              break;

            case MenuCommand.SaveAsXml:
              if (!editor.IsDocumentOpen)
              {
                Console.WriteLine("No document open");
                break;
              }

              Console.Write("Enter XML file path to save: ");
              string? xmlPath;
              xmlPath = Console.ReadLine();

              Console.WriteLine("XML serialization not fully implemented in this demo");
              break;

            case MenuCommand.LoadXml:
              Console.Write("Enter XML file path to load: ");
              string? loadXmlPath;
              loadXmlPath = Console.ReadLine();

              Console.WriteLine("XML deserialization not fully implemented in this demo");
              break;

            case MenuCommand.BuildIndex:
              Console.Write("Enter directory to index: ");
              string? indexDir;
              indexDir = Console.ReadLine();

              if (string.IsNullOrWhiteSpace(indexDir))
              {
                indexDir = currentDir;
              }

              Console.Write("Enter keywords (comma separated): ");
              string? keywordsInput;
              keywordsInput = Console.ReadLine();

              if (string.IsNullOrWhiteSpace(keywordsInput))
              {
                Console.WriteLine("Keywords cannot be empty");
                break;
              }

              List<string> keywords;
              keywords = keywordsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(k => k.Trim())
                                      .ToList();

              if (keywords.Count == 0)
              {
                Console.WriteLine("No valid keywords provided");
                break;
              }

              if (_indexBuilder == null)
              {
                _indexBuilder = new IndexBuilder();
              }

              _indexBuilder.BuildIndex(indexDir, keywords, true);
              Console.WriteLine("Index built successfully!");
              break;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error: {ex.Message}");
        }
      }
    }
  }
}