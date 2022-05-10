using System;
using System.IO;

namespace Lexer
{
    class Program
    {
        const string FileNmae = "1.txt";
        static TokenStorage TokenStorage = new TokenStorage();
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(FileNmae);
            Console.WriteLine(string.Format("{0, -17}{1, -16}{2, -8}{3, -30}", "Token", "Line", "Pos", "Value"));

            bool isBlockComment = false;
            string line;
            string word = "";
            int stringCounter = 0;
            while ((line = sr.ReadLine()) != null)
            {
                bool isLineComment = false;
                bool isString = false;

                for (int i = 0; i < line.Length; i++)
                {
                    char ch = line[i];

                    if (isLineComment)
                    {
                        break;
                    }

                    if (isBlockComment)
                    {
                        if (line[i] == '*' && line.Length > i + 1 && line[i + 1] == '/')
                        {
                            isBlockComment = false;
                            i += 2;
                        }
                        continue;
                    }

                    if (isString)
                    {
                        if (ch == '"' && line[i - 1] != '\\')
                        {
                            isString = false;
                            WriteTokenData(word + '"', stringCounter, i - word.Length);
                            word = "";
                        }
                        else
                        {
                            word += ch;
                        }
                        continue;
                    }

                    switch (ch)
                    {
                        case '/':
                            if (line[i + 1] == '*')
                            {
                                isBlockComment = true;
                                WriteTokenData(word, stringCounter, i - word.Length - 1);
                                word = "";
                                continue;
                            }
                            else if (line[i + 1] == '/')
                            {
                                isLineComment = true;
                            }
                            break;
                        case '"':
                            isString = true;
                            WriteTokenData(word, stringCounter, i - word.Length - 1);
                            word = "\"";
                            break;
                        case '=':
                        case '+':
                        case '-':
                        case '*':
                        case ',':
                        case ':':
                        case ';':
                        case '(':
                        case ')':
                        case '{':
                        case '}':
                        case '[':
                        case ']':
                        case '<':
                        case '>':
                            {
                                WriteTokenData(word, stringCounter, i - word.Length);
                                WriteTokenData(ch.ToString(), stringCounter, i);
                                word = "";
                                break;
                            }
                        default:
                            {
                                if (ch == ' ')
                                {
                                    WriteTokenData(word, stringCounter, i - word.Length);
                                    word = "";
                                }
                                else
                                {
                                    word += ch;
                                }

                                if (i == line.Length - 1)
                                {
                                    WriteTokenData(word, stringCounter, i);
                                    word = "";
                                }

                                break;
                            }
                    }

                }

                if (isString)
                {
                    WriteTokenData(word, stringCounter,line.Length - word.Length);
                    word = "";
                }
                stringCounter++;
            }
            sr.Close();
        }

        static void WriteTokenData(string token, int line, int pos)
        {
            if (token == "")
            {
                return;
            }

            string data = TokenStorage.GetTokenData(token);

            Console.WriteLine(string.Format("{0, -17}{1, -16}{2, -8}{3, -30}", token, line, pos, data));
        }
    }
}