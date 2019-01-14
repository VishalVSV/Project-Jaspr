using Project_Jaspr.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            List<List<Token>> tons = Lexer.Lex("Program.jass", true);

            foreach (List<Token> item in tons)
            {
                foreach (Token tok in item)
                {
                    Console.Write(tok.ToString() + ",");
                }
                Console.Write("\n");
            }

            AbstractSyntaxTree ast = Parser.SParse(tons);


            Compiler.ExpCompile(ast, "Program");

            ast.root.PrintPretty("", true);
            Console.ReadKey();
        }
    }
}
