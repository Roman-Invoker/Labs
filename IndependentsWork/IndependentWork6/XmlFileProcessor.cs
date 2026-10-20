using System.Xml.Linq;

namespace IndependentWork6;

public class XmlFileProcessor : FileProcessor
{
    public override string FileType => "XML-файл";

    public XmlFileProcessor(string filePath) : base(filePath)
    {
    }

    protected override void ProcessContent(TextReader reader)
    {
        XDocument document = XDocument.Load(reader);

        string rootName = document.Root?.Name.LocalName ?? "(немає)";
        int elementCount = document.Descendants().Count();

        Console.WriteLine($"  Кореневий елемент: {rootName}, усього елементів: {elementCount}");
    }
}
