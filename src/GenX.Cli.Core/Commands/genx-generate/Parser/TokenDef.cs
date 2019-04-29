namespace GenX.Cli.Core.Commands.Generate.Parser
{
    public class TokenDef
    {
        public TokenDef(TokenType returnsToken, string identifier)
        {
            ReturnsToken = returnsToken;
            Identifier = identifier;
        }

        public TokenType ReturnsToken { get; set; }
        public string Identifier { get; set; }
    }
}
