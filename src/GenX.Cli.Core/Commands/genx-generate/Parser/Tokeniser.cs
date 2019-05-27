using System;
using System.Collections.Generic;

namespace GenX.Cli.Core.Commands.Generate.Parser
{
    public class Tokeniser
    {
        private readonly List<TokenDef> _tokenDefs;

        public Tokeniser()
        {
            _tokenDefs = new List<TokenDef>
            {
                new TokenDef(TokenType.Filter, "--FILTER"),
                new TokenDef(TokenType.Filter, "--F"),
                new TokenDef(TokenType.MetaData, "--METADATA"),
                new TokenDef(TokenType.MetaData, "--MD"),
                new TokenDef(TokenType.OutputDir, "--OUTPUTDIR"),
                new TokenDef(TokenType.OutputDir, "--OD"),
                new TokenDef(TokenType.OutputExtension, "--OUTPUTEXTENSION"),
                new TokenDef(TokenType.OutputExtension, "--OE"),
                new TokenDef(TokenType.OutputPrefix, "--OUTPUTPREFIX"),
                new TokenDef(TokenType.OutputPrefix, "--OP"),
                new TokenDef(TokenType.OutputSuffix, "--OUTPUTSUFFIX"),
                new TokenDef(TokenType.OutputSuffix, "--OS"),
                new TokenDef(TokenType.XsltParam, "--XSLTPARAM"),
                new TokenDef(TokenType.XsltParam, "--XP"),
                new TokenDef(TokenType.XsltPath, "--XSLT"),
                new TokenDef(TokenType.XsltPath, "--X")
            };
        }

        public IEnumerable<Token> Tokenise(List<string> args)
        {
            if (args == null || args.Count == 0)
            {
                yield return new Token(TokenType.Terminator);
            }

            foreach (var token in args)
            {
                bool matchedToken = false;

                foreach (var tokenDefinition in _tokenDefs)
                {
                    if (token.Equals(tokenDefinition.Identifier, StringComparison.CurrentCultureIgnoreCase))
                    {
                        yield return new Token(tokenDefinition.ReturnsToken, token);
                        matchedToken = true;
                        break;
                    }
                }

                if (!matchedToken)
                {
                    var tokenType = GetTerm(token);
                    yield return new Token(tokenType, token);
                }
            }
        }

        private TokenType GetTerm(string identifier) =>
            identifier.StartsWith("\"") && identifier.EndsWith("\"")
                ? TokenType.QuotedTerm
                : TokenType.Term;
    }
}
