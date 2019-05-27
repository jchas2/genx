using FluentAssertions;
using GenX.Cli.Core.Commands.Generate.Parser;
using System.Linq;
using Xunit;

namespace genx_generate.Tests
{
    public class WhenUsingGenerateCommandParser
    {
        [Theory]
        [InlineData(new string[] {
            /*Should pass with all parameters */
            "--metadata", @"\.metadata.xml",
            "--filter", "categories",
            "--xslt", @".\repository.xslt",
            "--xsltparam", "Namespace:Acme.Repository", "--xsltparam", "Company:Acme",
            "--outputprefix", "I",
            "--outputsuffix", "Repository",
            "--outputExtension", ".cs",
            "--outputdir", @"\output" },
            0,
            new string[] { "" })]
        [InlineData(new string[] {
            /*Should pass without Xslt parameters */
            "--metadata", @"\.metadata.xml",
            "--xslt", @".\repository.xslt",
            "--outputprefix", "I",
            "--outputsuffix", "Repository",
            "--outputExtension", ".cs",
            "--outputdir", @"\output" },
            0,
            new string[] { "" })]
        [InlineData(new string[] {
            /* Should pass with Xslt parameters */
            "--metadata", @"\.metadata.xml",
            "--xslt", @".\repository.xslt",
            "--xsltparam", "Namespace:Acme.Repository", "--xsltparam", "Company:Acme",
            "--outputprefix", "I",
            "--outputsuffix", "Repository",
            "--outputExtension", ".cs",
            "--outputdir", @"\output" },
            0,
            new string[] { "" })]
        [InlineData(new string[] {
            /* Should pass without output file prefixes or suffixes */
            "--metadata", @"\.metadata.xml",
            "--xslt", @".\repository.xslt",
            "--xsltparam", "Namespace:Acme.Repository", "--xsltparam", "Company:Acme",
            "--outputExtension", ".cs",
            "--outputdir", @"\output" },
            0,
            new string[] { "" })]
        [InlineData(new string[] {
            /* Should pass without minimum required arguments */
            "--metadata", @"\.metadata.xml",
            "--xslt", @".\repository.xslt",
            "--outputdir", @"\output" },
            0,
            new string[] { "" })]
        [InlineData(new string[] {
            /* Should fail with only metadata argument */
            "--metadata", @"\.metadata.xml" },
            1,
            new string[] { "Expected [xslt-options]" })]
        [InlineData(new string[] {
            /* Should fail with empty argument */
            "\"\"" },
            2,
            new string[] {
                "Parser error: Expected metadata-options.",
                "Parser warning: Discarded unexpected token." })]
        public void Should_Parse(string[] args, int messageCount, string[] messages)
        {
            var tokeniser = new Tokeniser();
            var tokens = tokeniser.Tokenise(args.ToList()).ToList();

            var parser = new GenerateCommandParser();
            var configuration = parser.Parse(tokens);

            configuration.Messages.Count.Should().Be(messageCount);

            if (messageCount > 0)
            {
                for (int i = 0; i < messageCount; i++)
                {
                    configuration.Messages[i].Message.Should().Be(messages[i]);
                }
            }
        }
    }
}
