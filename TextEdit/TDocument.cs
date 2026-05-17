using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TextFileProcessor
{
  [Serializable]
  public class TDocument
  {
    private const int MinUndoStackSize = 1;

    public string? filePath { get; set; }
    public string? content { get; set; }
    public DateTime lastModified { get; set; }

    public TDocument()
    {
      filePath = null;
      content = null;
    }

    public TDocument(string path)
    {
      if (!File.Exists(path))
      {
        throw new Exception($"File not found: {path}");
      }

      filePath = Path.GetFullPath(path);
      content = File.ReadAllText(path);
      lastModified = File.GetLastWriteTime(path);
    }

    public TDocument(string path, string initialContent)
    {
      filePath = Path.GetFullPath(path);
      content = initialContent;
      lastModified = DateTime.Now;
    }

    public void Save()
    {
      if (string.IsNullOrEmpty(filePath))
      {
        throw new Exception("File path is not set");
      }

      string? directory;
      directory = Path.GetDirectoryName(filePath);

      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      File.WriteAllText(filePath, content ?? "");
      lastModified = File.GetLastWriteTime(filePath);
    }

    public void SaveAs(string newPath)
    {
      filePath = Path.GetFullPath(newPath);
      Save();
    }

    public TMemento CreateMemento()
    {
      TMemento memento;
      memento = new TMemento(content ?? "");

      return memento;
    }

    public void RestoreMemento(TMemento memento)
    {
      content = memento.content;
    }

    public void BinarySerialize(string targetPath)
    {
      FileStream stream;
      stream = new FileStream(targetPath, FileMode.Create);

      using (stream)
      {
        BinaryWriter writer;
        writer = new BinaryWriter(stream, Encoding.UTF8);

        using (writer)
        {
          writer.Write(filePath ?? "");
          writer.Write(content ?? "");
          writer.Write(lastModified.Ticks);
        }
      }
    }

    public static TDocument BinaryDeserialize(string path)
    {
      FileStream stream;
      stream = new FileStream(path, FileMode.Open);

      using (stream)
      {
        BinaryReader reader;
        reader = new BinaryReader(stream, Encoding.UTF8);

        using (reader)
        {
          TDocument document;
          document = new TDocument
          {
            filePath = reader.ReadString(),
            content = reader.ReadString(),
            lastModified = new DateTime(reader.ReadInt64())
          };

          return document;
        }
      }
    }

    public void XmlSerialize(string targetPath)
    {
      XmlSerializer serializer;
      serializer = new XmlSerializer(typeof(TDocument));

      using (StreamWriter writer = new StreamWriter(targetPath, false, Encoding.UTF8))
      {
        serializer.Serialize(writer, this);
      }
    }

    public static TDocument XmlDeserialize(string path)
    {
      XmlSerializer serializer;
      serializer = new XmlSerializer(typeof(TDocument));

      using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
      {
        TDocument? document;
        document = (TDocument?)serializer.Deserialize(reader);

        if (document == null)
        {
          throw new Exception("Failed to deserialize XML document");
        }

        return document;
      }
    }

    public override string ToString()
    {
      string fileName;
      fileName = Path.GetFileName(filePath ?? "unknown");

      return $"{fileName}: {(content ?? "").Length} chars, modified {lastModified:HH:mm:ss}";
    }

    public string GetFullPath()
    {
      return filePath ?? "";
    }
  }
}