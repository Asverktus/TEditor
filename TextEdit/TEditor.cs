using System;
using System.Collections.Generic;
using System.IO;

namespace TextFileProcessor
{
  public class TEditor
  {
    private const int InitialStateCount = 1;

    private TDocument? _document;
    private Stack<TMemento> _undoStack;
    private Stack<TMemento> _redoStack;

    public bool IsDocumentOpen
    {
      get
      {
        return _document != null;
      }
    }

    public TEditor()
    {
      _undoStack = new Stack<TMemento>();
      _redoStack = new Stack<TMemento>();
      _document = null;
    }

    public void OpenFile(string path)
    {
      _document = new TDocument(path);
      SaveState();

      string fileName;
      fileName = Path.GetFileName(path);

      Console.WriteLine($"Opened: {fileName}");
    }

    public void CreateFile(string path, string content = "")
    {
      _document = new TDocument(path, content);
      SaveState();

      string fileName;
      fileName = Path.GetFileName(path);

      Console.WriteLine($"Created: {fileName}");
    }

    private void SaveState()
    {
      if (_document == null)
      {
        return;
      }

      TMemento memento;
      memento = _document.CreateMemento();

      _undoStack.Push(memento);
      _redoStack.Clear();
    }

    public void EditContent(string newContent)
    {
      if (_document == null)
      {
        Console.WriteLine("No document open");
        return;
      }

      SaveState();
      _document.content = newContent;
    }

    public void AppendText(string text)
    {
      if (_document == null)
      {
        Console.WriteLine("No document open");
        return;
      }

      SaveState();
      _document.content = _document.content + text;
    }

    public bool Undo()
    {
      if (_document == null)
      {
        return false;
      }

      if (_undoStack.Count <= InitialStateCount)
      {
        return false;
      }

      TMemento current;
      current = _undoStack.Pop();

      _redoStack.Push(current);

      TMemento previous;
      previous = _undoStack.Peek();

      _document.RestoreMemento(previous);

      return true;
    }

    public bool Redo()
    {
      if (_document == null)
      {
        return false;
      }

      if (_redoStack.Count == 0)
      {
        return false;
      }

      TMemento redo;
      redo = _redoStack.Pop();

      _undoStack.Push(redo);
      _document.RestoreMemento(redo);

      return true;
    }

    public void Save()
    {
      if (_document == null)
      {
        Console.WriteLine("No document open");
        return;
      }

      string oldPath;
      oldPath = _document.GetFullPath();

      long oldSize;
      oldSize = 0;

      if (File.Exists(oldPath))
      {
        FileInfo oldFileInfo;
        oldFileInfo = new FileInfo(oldPath);
        oldSize = oldFileInfo.Length;
      }

      _document.Save();

      string newPath;
      newPath = _document.GetFullPath();

      FileInfo fileInfo;
      fileInfo = new FileInfo(newPath);

      Console.WriteLine(
        $"Saved successfully!\n" +
        $"Location: {newPath}\n" +
        $"Size: {fileInfo.Length} bytes (was {oldSize} bytes)\n" +
        $"Modified: {_document.lastModified:yyyy-MM-dd HH:mm:ss}"
      );
    }

    public void SaveAs(string newPath)
    {
      if (_document == null)
      {
        Console.WriteLine("No document open");
        return;
      }

      _document.SaveAs(newPath);
      Console.WriteLine($"Saved as: {Path.GetFileName(newPath)}");
    }

    public void ShowContent()
    {
      if (_document == null)
      {
        Console.WriteLine("No document open");
        return;
      }

      int lineCount;
      lineCount = 0;

      string contentToShow;
      contentToShow = _document.content ?? "";

      if (!string.IsNullOrEmpty(contentToShow))
      {
        lineCount = contentToShow.Split('\n').Length;
      }

      string stats;
      stats = $"\n--- DOCUMENT CONTENT ---\n{contentToShow}\n--- END OF DOCUMENT ---\nStats: {contentToShow.Length} chars, {lineCount} lines\n";

      Console.WriteLine(stats);
    }
  }
}