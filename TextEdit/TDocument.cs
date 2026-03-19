using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TextFileProcessor
{
  [Serializable]
  public class TDocument
  {
    public string filePath { get; private set; }
    public string content { get; set; }
    public DateTime lastModified { get; private set; }

    private TDocument()
    {

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
      string directory;
      directory = Path.GetDirectoryName(filePath);

      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      File.WriteAllText(filePath, content);
      lastModified = File.GetLastWriteTime(filePath);
    }

    public TMemento CreateMemento()
    {
      TMemento memento;
      memento = new TMemento(content);

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
      StreamWriter writer;
      writer = new StreamWriter(targetPath);

      using (writer)
      {
        XmlSerializer serializer;
        serializer = new XmlSerializer(typeof(TDocument));

        serializer.Serialize(writer, this);
      }
    }

    public static TDocument XmlDeserialize(string path)
    {
      StreamReader reader;
      reader = new StreamReader(path);

      using (reader)
      {
        XmlSerializer serializer;
        serializer = new XmlSerializer(typeof(TDocument));

        TDocument document;
        document = (TDocument)serializer.Deserialize(reader);

        return document;
      }
    }

    public override string ToString()
    {
      string fileName;
      fileName = Path.GetFileName(filePath);

      return $"{fileName}: {content.Length} chars, modified {lastModified:HH:mm:ss}";
    }

    public string GetFullPath()
    {
      return filePath;
    }
  }
}