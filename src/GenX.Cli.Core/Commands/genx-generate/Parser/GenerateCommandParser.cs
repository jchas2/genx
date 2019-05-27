using System;
using System.Collections.Generic;
using System.Linq;

namespace GenX.Cli.Core.Commands.Generate.Parser
{
    public class GenerateCommandParser
    {
        private Stack<Token> _tokens;
        private Token _lookaheadFirst;
        private Token _lookaheadSecond;
        private Configuration _configuration;

        public Configuration Parse(List<Token> tokens)
        {
            LoadStack(tokens);
            PrepareLookaheads();

            _configuration = new Configuration();

            MetadataCondition();
            DiscardToken(TokenType.Terminator);

            return _configuration;
        }

        private void MetadataCondition()
        {
            if (IsMetadataOption(_lookaheadFirst) && IsTerm(_lookaheadSecond))
            {
                if (_lookaheadFirst.TokenType == TokenType.MetaData && IsTerm(_lookaheadSecond))
                {
                    _configuration.MetadataPath = _lookaheadSecond.Value;
                }
                else if (_lookaheadFirst.TokenType == TokenType.Filter && IsTerm(_lookaheadSecond))
                {
                    _configuration.MetadataFilter = _lookaheadSecond.Value;
                }
                else
                {
                    _configuration.Messages.Add(new Configuration.MessageInfo(StringResources.ParserExpectedMetadataOption, true));
                }

                DiscardToken();
                DiscardToken();
                MetadataConditionNext();
            }
            else
            {
                _configuration.Messages.Add(new Configuration.MessageInfo(StringResources.ParserExpectedMetadataOption, true));
            }
        }

        private void MetadataConditionNext()
        {
            if (IsMetadataOption(_lookaheadFirst))
            {
                MetadataCondition();
            }
            else
            {
                XsltCondition();
            }
        }

        private void XsltCondition()
        {
            if (IsXsltOption(_lookaheadFirst) || IsOutputOption(_lookaheadFirst))
            {
                if (_lookaheadFirst.TokenType == TokenType.XsltPath && IsTerm(_lookaheadSecond))
                {
                    _configuration.XsltPath = _lookaheadSecond.Value;
                }
                else if (_lookaheadFirst.TokenType == TokenType.XsltParam && IsTerm(_lookaheadSecond))
                {
                    XsltParamCondition();
                }
                else if (_lookaheadFirst.TokenType == TokenType.OutputDir && IsTerm(_lookaheadSecond))
                {
                    _configuration.OutputDirectory = _lookaheadSecond.Value;
                }
                else if (_lookaheadFirst.TokenType == TokenType.OutputExtension && IsTerm(_lookaheadSecond))
                {
                    _configuration.OutputFileExtension = _lookaheadSecond.Value;
                }
                else if (_lookaheadFirst.TokenType == TokenType.OutputPrefix && IsTerm(_lookaheadSecond))
                {
                    _configuration.OutputFileNamePrefix = _lookaheadSecond.Value;
                }
                else if (_lookaheadFirst.TokenType == TokenType.OutputSuffix && IsTerm(_lookaheadSecond))
                {
                    _configuration.OutputFileNameSuffix = _lookaheadSecond.Value;
                }
                else
                {
                    AddParserError(StringResources.ParserExpectedXsltOption);
                }

                DiscardToken();
                DiscardToken();
                XsltConditionNext();
            }
            else
            {
                AddParserError(StringResources.ParserExpectedXsltOption);
            }
        }

        private void XsltConditionNext()
        {
            if (IsXsltOption(_lookaheadFirst) || IsOutputOption(_lookaheadFirst))
            {
                XsltCondition();
            }
            else if (_lookaheadFirst.TokenType == TokenType.Terminator)
            {
                // No-op.
            }
            else
            {
                AddParserError(StringResources.ParserExpectedXsltOption);
            }
        }

        private void XsltParamCondition()
        {
            string[] paramNameAndValue = _lookaheadSecond.Value.Split(":", StringSplitOptions.RemoveEmptyEntries);

            if (paramNameAndValue.Length == 2)
            {
                _configuration.Parameters.Add(new Configuration.ParameterConfiguration { Name = paramNameAndValue[0], Value = paramNameAndValue[1] });
            }
            else
            {
                AddParserError(StringResources.ParserExpectedXsltParam);
            }
        }

        private void LoadStack(List<Token> tokens)
        {
            _tokens = new Stack<Token>();
            int count = tokens.Count;

            for (int i = count - 1; i >= 0; i--)
            {
                _tokens.Push(tokens[i]);
            }
        }

        private void PrepareLookaheads()
        {
            _lookaheadFirst = _tokens.Pop();

            _lookaheadSecond = _tokens.Count != 0
                ? _tokens.Pop()
                : CreateTerminatorToken();
        }

        private void DiscardToken(TokenType tokenType)
        {
            if (_lookaheadFirst.TokenType != tokenType)
            {
                AddParserWarning(StringResources.ParserDiscardedUnexpectedToken);
            }

            DiscardToken();
        }

        private void DiscardToken()
        {
            _lookaheadFirst = _lookaheadSecond.Clone();
            _lookaheadSecond = _tokens.Any() 
                ? _tokens.Pop() 
                : CreateTerminatorToken();
        }

        private Token CreateTerminatorToken() =>
            new Token(TokenType.Terminator, string.Empty);

        private bool IsMetadataOption(Token token) =>
            token.TokenType == TokenType.MetaData ||
            token.TokenType == TokenType.Filter;

        private bool IsXsltOption(Token token) =>
            token.TokenType == TokenType.XsltPath ||
            token.TokenType == TokenType.XsltParam;

        private bool IsOutputOption(Token token) =>
            token.TokenType == TokenType.OutputDir ||
            token.TokenType == TokenType.OutputExtension ||
            token.TokenType == TokenType.OutputPrefix ||
            token.TokenType == TokenType.OutputSuffix;

        private bool IsTerm(Token token) =>
            token.TokenType == TokenType.Term ||
            token.TokenType == TokenType.QuotedTerm;

        private void AddParserError(string message)
        {
            if (_configuration.Messages.Any(messageInfo => messageInfo.Message == message && messageInfo.IsError) == false)
            {
                _configuration.Messages.Add(new Configuration.MessageInfo(message, true));
            }
        }

        private void AddParserWarning(string message)
        {
            if (_configuration.Messages.Any(messageInfo => messageInfo.Message == message && messageInfo.IsError == false) == false)
            {
                _configuration.Messages.Add(new Configuration.MessageInfo(message, false));
            }
        }
    }
}
