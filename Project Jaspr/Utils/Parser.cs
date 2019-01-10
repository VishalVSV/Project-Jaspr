using Project_Jaspr.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils
{
    public static class Parser
    {
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

                int k = 0;
                bool match = false;
                int lvl = level;
                for (int n = 0; n < Defs.grammer.Length; n++)
                {
                    ProductionRule prRule = Defs.grammer[n];
                    int successes = 0;
                    if (line.Count == 0)
                    {
                        break;
                    }
                    List<string> args = new List<string>();
                    for (int j = lvl; j < line.Count; j++)
                    {
                        for (int i = 0; i < prRule.tokens.Length; i++)
                        {
                            match = false;
                            string tok = prRule.tokens[i];
                            if (line[j].value == tok)
                            {
                                match = true;
                                successes++;
                            }
                            else if (line[j].type == tok)
                            {
                                match = true;
                                successes++;
                                args.Add(line[j].value);
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
                            j++;
                        }
                    }
                    if (match)
                    {
                        Node node = prRule.getNode(args.ToArray(), curr);
                        curr.branches.Add(node);
                        break;
                    }
                }
                if (!match)
                {
                    throw new Exception("Syntax error at line:" + tokens.IndexOf(line).ToString());
                }
            }

            while (curr.parent != null)
            {
                curr = curr.parent;
            }

            return ast;
        }
    }
}
