using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils
{


    public static class Lexer
    {
        public static List<string> Keywords = new List<string>();

        static Lexer()
        {
            Keywords.Add("func");
            Keywords.Add("(");
            Keywords.Add(")");
            Keywords.Add("return");
        }

        public static List<Token> Lex(string path)
        {
            List<Token> tokens = new List<Token>();

            string prog;
            using (StreamReader sr = new StreamReader(path))
            {
                prog = sr.ReadToEnd();
            }

            int i = 0;
            while (i < prog.Length)
            {
                bool keywordIs = false;

                foreach (string keyword in Keywords)
                {
                    try
                    {
                        string l = prog.Substring(i, 1);
                        if (prog.Substring(i, 1) == " ")
                            i++;
                        l = prog.Substring(i, 1);

                        if (prog.Substring(i, keyword.Length) == keyword)
                        {
                            tokens.Add(Token.Construct(keyword));
                            i += keyword.Length;
                            keywordIs = true;
                        }

                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        continue;
                    }
                }
                if (!keywordIs)
                {
                    int j = i - 1;
                    string tok = "";
                    while (j < prog.Length)
                    {
                        bool isKeyword = true;
                        if (prog.Substring(j, 1) == " ")
                            j++;
                        foreach (string key in Keywords)
                        {
                            try
                            {
                                if (prog.Substring(j, key.Length) != key)
                                {
                                    isKeyword = false;
                                }
                                else
                                {
                                    isKeyword = true;
                                    break;
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                continue;
                            }
                        }

                        if (!isKeyword)
                        {
                            tok += prog.Substring(j, 1);
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (tok.Contains("\r\n"))
                    {
                        if (tok.Substring(0, tok.IndexOf("\r\n")) != "")
                            if (!IsDigitsOnly(tok.Substring(0, tok.IndexOf("\r\n"))))
                                tokens.Add(Token.Construct(tok.Substring(0, tok.IndexOf("\r\n"))));
                            else
                                tokens.Add(Token.Construct("CONSTANT:" + tok.Substring(0, tok.IndexOf("\r\n"))));
                        tokens.Add(Token.Construct("ENDLINE"));

                        if (tok.Contains("\t"))
                            tokens.Add(Token.Construct("INDENT"));
                    }
                    else
                    {
                        if (!IsDigitsOnly(tok))
                            tokens.Add(Token.Construct(tok));
                        else
                            tokens.Add(Token.Construct("CONSTANT:" + tok));
                    }
                    i += j - i - 1;
                }


                i++;
            }
            tokens.Add(Token.Construct("ENDLINE"));


            return tokens;
        }

        static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

    }

    public class Token
    {
        public string value;

        public static Token Construct(string value)
        {
            Token t = new Token();
            t.value = value;
            return t;
        }
    }
}
