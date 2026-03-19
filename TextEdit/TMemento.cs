using System;

namespace TextFileProcessor
{
  public class TMemento
  {
    public string content { get; private set; }
    public DateTime timestamp { get; private set; }

    public TMemento(string content)
    {
      this.content = content;
      timestamp = DateTime.Now;
    }
  }
}