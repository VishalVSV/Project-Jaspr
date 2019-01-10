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

            ast.root.PrintPretty("", true);

            Compiler.ExpCompile(ast, "Program");

            Console.ReadKey();
            /*
            string file = "";
            bool debugTokens = false, debugTree = false;

            if (args.Length == 0)
            {
                Console.WriteLine("No input source file specified\n\"Project Jaspr.exe\" -h for help");
                return;
            }

            if (args[0] == "-h")
            {
                Console.WriteLine("-s\t\tSource File");
                Console.WriteLine("-d tokens\tDebug Tokens");
                Console.WriteLine("-d tree\t\tDebug Tree");
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-s")
                {
                    file = args[i + 1];
                    i++;
                }
                else if (args[i] == "-d")
                {
                    if (args[i + 1] == "tokens")
                    {
                        debugTokens = true;
                        i++;
                    }
                    else if (args[i + 1] == "tree")
                    {
                        debugTree = true;
                        i++;
                    }
                }
            }

            List<Token> tokens = null;
            if (!File.Exists(file + ".jass"))
            {
                Console.WriteLine("File does not exist!");
                return;
            }

            if (file != "")
                tokens = Lexer.Lex(file+".jass");
            else
            {
                Console.WriteLine("File is not specified");
                return;
            }

            if(debugTokens)
            foreach (Token tok in tokens)
            {
                Console.WriteLine(tok.value);
            }

            //AbstractSyntaxTree ast = AbstractSyntaxTree.FromTokenArray(tokens);

            AbstractSyntaxTree ast = AbstractSyntaxTree.astFromTokens(tokens);

            Console.Write("\n\n");

            if(debugTree)
            ast.root.PrintPretty("", true);

            Compiler.ExpCompile(ast, file);
            Console.ReadKey();*/
        }
    }
}
