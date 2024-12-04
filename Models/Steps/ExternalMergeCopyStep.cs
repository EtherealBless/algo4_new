using System;
using System.IO;

namespace WpfApp.Models.Steps
{
    public class ExternalMergeCopyStep : SortingStep<string>
    {
        public string Element { get; }
        public string SourceFile { get; }
        public string DestinationFile { get; }

        public ExternalMergeCopyStep(string element, string sourceFile, string destinationFile) 
            : base()  // We only show the current element being copied
        {
            Element = element;
            SourceFile = sourceFile;
            DestinationFile = destinationFile;
        }

        public override string GetDescription() =>
            $"Copying element '{Element}' from {Path.GetFileName(SourceFile)} to {Path.GetFileName(DestinationFile)}";
    }
}
