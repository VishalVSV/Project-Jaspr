using Project_Jaspr.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            List<Token> tokens = Lexer.Lex("./Program.txt");

            foreach (Token tok in tokens)
            {
                Console.WriteLine(tok.value);
            }

            AbstractSyntaxTree ast = AbstractSyntaxTree.FromTokenArray(tokens);

            Console.Write("\n\n");

            ast.root.PrintPretty("",true);

            Compiler.Compile(ast, "Test");

            Console.ReadKey();
        }
    }
}
