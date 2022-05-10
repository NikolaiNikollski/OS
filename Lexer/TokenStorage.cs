
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TokenStorage
{
    private Dictionary<string, string> Tokens = new Dictionary<string, string>();

    public string GetTokenData(string token)
    {
        if (Tokens.ContainsKey(token))
        {
            return Tokens[token];
        }

        decimal number;
        if (decimal.TryParse(token, out number))
        {
            return "NUMBER";
        }

        if (token[0] == '0' && token[1] == 'x')
        {
            try
            {
                Convert.ToInt32(token, 16);
                return "HEX NUMBER";
            }
            catch
            {
                return "ERROR: INVALID HEX NUMBER";
            }
        }

        if (token[0] == '"')
        {
            if (token[token.Length - 1] == '"')
            {
                return "STRING";
            }
            else
            {
                return "ERROR: unfinished srting";
            }
        }

        string identidierPattern = @"^[a-zA-Z]\w*$";
        Regex rx = new Regex(identidierPattern);
        if (rx.Match(token).Success)
        {
            return "IDENTIFIER";
        }
        else
        {
            return "INVALID IDENTIFIER";
        }
    }

public TokenStorage()
{
    Tokens = new Dictionary<string, string>() {
            {"and", "AND" },
            {"auto", "AUTO" },
            {"bool", "BOOL" },
            {"break", "BREAK" },
            {"case", "CASE" },
            {"catch", "CATCH" },
            {"char", "CHAR" },
            {"class", "CLASS" },
            {"const", "CONST" },
            {"constexpr", "CONSTEXPR" },
            {"Convert", "CONVERT" },
            {"continue", "CONTINUE" },
            {"default", "DEFAULT" },
            {"delete", "DELETE" },
            {"do", "DO" },
            {"double", "DOUBLE" },
            {"dynamic_cast", "DYNAMIC_CAST" },
            {"else", "ELSE" },
            {"enum", "ENUM" },
            {"explicit", "EXPLICIT" },
            {"EventArgs", "EVENTARGS" },
            {"false", "FALSE" },
            {"float", "FLOAT" },
            {"for", "FOR" },
            {"friend", "FRIEND" },
            {"if", "IF" },
            {"long", "LONG" },
            {"mutable", "MUTABLE" },
            {"namespace", "NAMESPACE" },
            {"new", "NEW" },
            {"not", "NOT" },
            {"nullptr", "NULLPTR" },
            {"operator", "OPERATOR" },
            {"or", "OR" },
            {"private", "PRIVATE" },
            {"protected", "PROTECTED" },
            {"public", "PUBLIC" },
            {"return", "RETURN" },
            {"short", "SHORT" },
            {"signed", "SIGNED" },
            {"static", "STATIC" },
            {"static_cast", "STATIC_CAST" },
            {"struct", "STRUCT" },
            {"switch", "SWITCH" },
            {"template", "TEMPLATE" },
            {"this", "THIS" },
            {"throw", "THROW" },
            {"true", "TRUE" },
            {"try", "TRY" },
            {"ToDouble", "TODOUBLE" },
            {"typename", "TYPENAME" },
            {"unsigned", "UNSIGNED" },
            {"using", "USING" },
            {"virtual", "VIRTUAL" },
            {"void", "VOID" },
            {"while", "WHILE" },
            {"xor", "XOR" },
            {"override", "OVERRIDE" },
            {"object", "OBJECT"},
            {"sender", "SENDER"},
            {"final", "FINAL" },
            {"=", "ASSIGN" },
            {"+", "PLUS" },
            {"-", "MINUS" },
            {"*", "MULTIPLICATION" },
            {"/", "DIVISIO" },
            {",", "COMMA" },
            {";", "SEMI_COLON" },
            {":", "COLON" },
            {"(", "OPEN_PARENTHESIS" },
            {")", "CLOSE_PARENTHESIS" },
            {"{", "OPEN_CURLY_BRACKET" },
            {"}", "CLOSE_CURLY_BRACKET" },
            {"[", "OPEN_SQUARE_BRACKET" },
            {"]", "CLOSE_SQUARE_BRACKET" },
            {"||", "OR" },
            {"&&", "AND" },
            {"/*", "OPEN_BLOCK_COMMENT" },
            {"*/", "CLOSE_BLOCK_COMMENT" },
            {"<", "SIGN_LESS" },
            {">", "SIGN_MORE" },
        };
}
};

