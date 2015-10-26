using System.IO;

namespace Adaptive.ReactiveTrader.Client
{
    public class TextFileWriter : Adaptive.ReactiveTrader.Client.Domain.Instrumentation.ITextFileWriter
    {
        public void WriteFile(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }
    }
}
