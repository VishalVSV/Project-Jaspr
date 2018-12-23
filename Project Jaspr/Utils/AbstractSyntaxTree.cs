using Project_Jaspr.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils
{
    public class AbstractSyntaxTree
    {
        public Node root;

        public static AbstractSyntaxTree FromTokenArray(List<Token> tokens)
        {
            AbstractSyntaxTree ast = new AbstractSyntaxTree();

            ast.root = new Program();

            List<List<Token>> lines = new List<List<Token>>();


            List<Token> line2 = new List<Token>();
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].value != "ENDLINE")
                {
                    line2.Add(tokens[i]);
                }
                else
                {
                    lines.Add(line2);
                    line2 = new List<Token>();
                }
            }

            List<Function> functions = new List<Function>();
            for (int i = 0; i < lines.Count; i++)
            {
                List<Token> line = lines[i];
                if (line[0].value == "func")
                {
                    Function function = new Function();
                    function.Name = line[1].value;
                    int j = i + 1;
                    while (j < lines.Count && lines[j][0].value == "INDENT")
                    {
                        List<Token> lne = lines[j];
                        if (lne[1].value == "return")
                        {
                            try
                            {
                                if (lne[2].value.Substring(0, "CONSTANT:".Length) == "CONSTANT:")
                                {
                                    function.branches.Add(new Return(int.Parse(lne[2].value.Substring("CONSTANT:".Length))));
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        j++;
                    }

                    functions.Add(function);
                    i = j-1;
                }
            }

            ast.root.branches.AddRange(functions);

            return ast;
        }
    }

    public abstract class Node
    {
        public List<Node> branches = new List<Node>();


        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine(ToString());

            for (int i = 0; i < branches.Count; i++)
                branches[i].PrintPretty(indent, i == branches.Count - 1);
        }
    }
}
