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
            Keywords.Add("=");
            Keywords.Add("return");
            Keywords.Add("#");
            Keywords.Add("+");
        }

        public static List<List<Token>> Lex(string path, bool exp)
        {
            List<List<Token>> ret = new List<List<Token>>();
            string prog = "";
            using (StreamReader sr = new StreamReader(path))
            {
                prog = sr.ReadToEnd();
            }
            string[] lines = prog.Split(new[] { "\r\n" }, StringSplitOptions.None);

            int i = 0, j = 0;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.Contains("#"))
                {
                    line = line.Substring(0, line.IndexOf("#"));
                    i++;
                    continue;
                }
                List<Token> lne = new List<Token>();
                Start:
                while (line != "")
                {
                    j = 0;
                    bool isKeyword = false;
                    if (line.Substring(0, 1) == " ")
                    {
                        line = line.Substring(1);
                    }
                    if (line.StartsWith("\t"))
                    {
                        line = line.Substring(1);
                        lne.Add(Token.Construct("INDENT"));
                    }

                    while (j < Keywords.Count)
                    {
                        string Keyword = Keywords[j];
                        if (line.StartsWith(Keyword))
                        {
                            lne.Add(Token.Construct(Keyword));
                            j = 0;
                            line = line.Substring(Keyword.Length);
                            isKeyword = true;
                        }

                        j++;
                    }
                    if (!isKeyword)
                    {
                        int k = 0;
                        bool isProcessed = false;
                        while (k < line.Length)
                        {
                            foreach (string key in Keywords)
                            {
                                if (k + key.Length <= line.Length)
                                    if (line.Substring(k, key.Length) == key || line.Substring(k, 1) == " ")
                                    {
                                        string toAdd = line.Substring(0, k);
                                        if (!IsDigitsOnly(toAdd))
                                        {
                                            lne.Add(Token.Construct(toAdd, "Identifier"));
                                        }
                                        else
                                        {
                                            lne.Add(Token.Construct(toAdd, "Constant"));
                                        }
                                        isProcessed = true;
                                        line = line.Substring(k);
                                        if (k < line.Length)
                                        {
                                            goto Start;
                                        }
                                    }
                            }
                            k++;
                        }
                        if (!isProcessed)
                        {
                            string[] vals = line.Split(' ');
                            line = "";
                            foreach (string toAdd in vals)
                            {
                                if (!IsDigitsOnly(toAdd))
                                {
                                    lne.Add(Token.Construct(toAdd, "Identifier"));
                                }
                                else
                                {
                                    lne.Add(Token.Construct(toAdd, "Constant"));
                                }
                            }
                        }
                    }
                }
                i++;
                lne.ForEach((Token t) => {
                    if (t.value == "")
                        lne.Remove(t);
                });
                ret.Add(lne);
            }

            return ret;
        }

        
        static void AddToken(ref string tok, ref List<Token> tokens)
        {
            if (tok.Contains("\r\n\t"))
            {
                string a = tok.Substring(0, tok.IndexOf("\r\n\t"));
                string b = tok.Substring(tok.IndexOf("\r\n\t") + "\r\n\t".Length);

                if (a != "")
                {
                    if (!IsDigitsOnly(a))
                        tokens.Add(Token.Construct(a, "Identifier"));
                    else
                        tokens.Add(Token.Construct(a, "Constant"));
                }
                tokens.Add(Token.Construct("ENDLINE"));
                tokens.Add(Token.Construct("INDENT"));
                if (b != "")
                {
                    if (!IsDigitsOnly(b))
                        tokens.Add(Token.Construct(b, "Identifier"));
                    else
                        tokens.Add(Token.Construct(b, "Constant"));
                }
            }
            else if (tok.Contains("\r\n"))
            {
                string a = tok.Substring(0, tok.IndexOf("\r\n"));
                string b = tok.Substring(tok.IndexOf("\r\n") + "\r\n".Length);

                if (a != "")
                {
                    if (!IsDigitsOnly(a))
                        tokens.Add(Token.Construct(a, "Identifier"));
                    else
                        tokens.Add(Token.Construct(a, "Constant"));
                }
                tokens.Add(Token.Construct("ENDLINE"));
                if (b != "")
                {
                    if (!IsDigitsOnly(b))
                        tokens.Add(Token.Construct(b, "Identifier"));
                    else
                        tokens.Add(Token.Construct(b, "Constant"));
                }
            }
            else
            {
                if (!IsDigitsOnly(tok))
                {
                    tokens.Add(Token.Construct(tok, "Identifier"));
                }
                else
                {
                    tokens.Add(Token.Construct(tok, "Constant"));
                }
            }

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
        public string type;

        public static Token Construct(string value, string type = "Keyword")
        {
            Token t = new Token();
            t.value = value;
            t.type = type;
            return t;
        }

        public override string ToString()
        {
            return value;
        }
    }
}