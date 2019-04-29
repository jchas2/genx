using System.Collections.Generic;

namespace GenX.Cli.Core.Commands.Generate
{
    public class Configuration
    {
        public class ParameterConfiguration
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class MessageInfo
        {
            public MessageInfo(string message) => Message = message;

            public MessageInfo(string message, bool isError)
            {
                Message = message;
                IsError = isError;
            }

            public string Message { get; set; }
            public bool IsError { get; set; }
        }

        public Configuration() { }

        public string XsltPath { get; set; }
        public List<ParameterConfiguration> Parameters { get; set; } = new List<ParameterConfiguration>();
        public string MetadataPath { get; set; }
        public string OutputDirectory { get; set; }
        public string OutputFileExtension { get; set; }
        public string OutputFilename { get; set; }
        public string OutputFileNamePrefix { get; set; }
        public string OutputFileNameSuffix { get; set; }
        public List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();
    }
}
