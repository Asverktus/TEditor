using System;
using System.Collections.Generic;
using System.IO;

namespace TextFileProcessor
{
  public class TEditor
  {
    private TDocument _document;
    private Stack<TMemento> _undoStack;
    private Stack<TMemento> _redoStack;

    public bool isDocumentOpen
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
      int minUndoStackSize;
      minUndoStackSize = 1;

      if (_undoStack.Count <= minUndoStackSize)
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

    public void ShowContent()
    {
      if (_document == null)
      {
        Console.WriteLine("No document open");
        return;
      }

      Console.WriteLine("\n--- DOCUMENT CONTENT ---");
      Console.WriteLine(_document.content);
      Console.WriteLine("--- END OF DOCUMENT ---");

      int lineCount;
      lineCount = 0;

      if (!string.IsNullOrEmpty(_document.content))
      {
        lineCount = _document.content.Split('\n').Length;
      }

      Console.WriteLine($"Stats: {_document.content.Length} chars, {lineCount} lines\n");
    }
  }
}