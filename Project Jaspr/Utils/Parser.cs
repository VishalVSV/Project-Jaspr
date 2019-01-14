using Project_Jaspr.Utils.Nodes;
using Project_Jaspr.Utils.PrRules;
using System;
using System.Collections.Generic;

namespace Project_Jaspr.Utils
{
    public static class Parser
    {
        public static void PreProcess(this List<List<Token>> tokens)
        {
            
        }


        public static AbstractSyntaxTree SParse(List<List<Token>> tokens)
        {
            AbstractSyntaxTree ast = new AbstractSyntaxTree();

            ast.root = new Program();

            ref Node curr = ref ast.root;
            int level = 0;
            foreach (List<Token> line in tokens)
            {
                int oldLevel = level;
                level = 0;
                for (int i = 0; i < line.Count; i++)
                {
                    if (line[i].value == "INDENT")
                    {
                        level++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (oldLevel <= level)
                {
                    for (int i = 0; i < level - oldLevel; i++)
                    {
                        if (curr.branches.Count > 0 && curr.branches[curr.branches.Count - 1] != null)
                        {
                            curr = curr.branches[curr.branches.Count - 1];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < oldLevel - level; i++)
                    {
                        if (curr.parent != null)
                            curr = curr.parent;
                    }
                }

                ParseLine(line, level, ref curr, false);
            }

            while (curr.parent != null)
            {
                curr = curr.parent;
            }

            return ast;
        }



        public static void ParseLine(List<Token> line, int level, ref Node curr, bool isBranched)
        {
            bool match = false;
            int lvl = level;
            for (int n = 0; n < Defs.grammer.Length; n++)
            {
                List<List<Token>> toParse = new List<List<Token>>();
                ProductionRule prRule = Defs.grammer[n];
                int successes = 0;
                if (line.Count == 0)
                {
                    break;
                }
                Node toAdd = null;
                List<string> args = new List<string>();
                for (int j = lvl; j < line.Count; j++)
                {
                    for (int i = 0; i < prRule.tokens.Length; i++)
                    {
                        match = false;
                        string tok = prRule.tokens[i];
                        List<string> toks = new List<string>();
                        if (tok.Contains("||"))
                        {
                            toks.AddRange(tok.Split(new string[] { "||" }, StringSplitOptions.None));
                        }
                        else
                        {
                            toks.Add(tok);
                        }
                        if (line[j].value == tok)
                        {
                            match = true;
                            successes++;
                        }
                        else if (toks.Contains(line[j].type))
                        {

                            match = true;
                            successes++;
                            args.Add(line[j].value);
                        }
                        else if (Defs.grammer.Contains(toks))
                        {
                            List<Token> newLine = new List<Token>();
                            newLine.AddRange(line.GetRange(j, line.Count - j));
                            toParse.Add(newLine);
                            j += newLine.Count;
                            match = true;
                        }
                        if (!match)
                        {
                            if (n + 1 < Defs.grammer.Length)
                            {
                                args.Clear();
                                prRule = Defs.grammer[++n];
                                i = -1;
                                j = lvl - 1;
                            }
                        }
                        if (j + 1 < line.Count)
                            j++;
                    }
                }
                if (match)
                {
                    Node node = prRule.getNode(args.ToArray(), curr);
                    node.parent = curr;
                    if(toParse.Count != 0)
                    {
                        foreach (List<Token> toPrse in toParse)
                        {
                            ParseLine(toPrse, 0, ref node, true);
                        }
                    }

                    curr.branches.Add(node);

                    
                    break;
                }
            }
        }
    }
}
