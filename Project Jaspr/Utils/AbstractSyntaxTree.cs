using Project_Jaspr.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils
{
    public static class Utils
    {
        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }

    public abstract class ProductionRule
    {
        public string[] tokens;
        public string name;

        public ProductionRule(string[] tokens, string name)
        {
            this.tokens = tokens;
            this.name = name;
        }

        public abstract Node getNode(string[] args, Node parent);

        public abstract string EvaluateAsm(string[] args);
    }

    public class FunctionR : ProductionRule
    {
        private string asm = "{0}:";

        public FunctionR(string[] tokens, string name) : base(tokens, name)
        {
        }

        public override string EvaluateAsm(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                asm = asm.Replace("{" + i.ToString() + "}", args[i]);
            }

            return asm;
        }

        public override Node getNode(string[] args, Node parent)
        {
            Function func = new Function();
            func.Name = args[0];
            func.parent = parent;
            return func;
        }
    }

    public abstract class StatementR : ProductionRule
    {
        public StatementR(string[] tokens, string name) : base(tokens, name)
        {
        }
    }

    public class ReturnR : StatementR
    {
        public ReturnR(string[] tokens, string name) : base(tokens, name)
        {
        }

        public override string EvaluateAsm(string[] args)
        {
            return "";
        }

        public override Node getNode(string[] args, Node parent)
        {
            Return rtr = null;
            if (Utils.IsDigitsOnly(args[0]))
            {
                rtr = new Return(new IntConstant(int.Parse(args[0])));
                rtr.parent = parent;
            }
            else
            {
                Int vr = new Int();
                vr.Name = args[0];
                IntRef rf = new IntRef(vr);
                rtr = new Return(rf);
                rtr.branches[0].parent = rtr;
                rtr.parent = parent;
            }
            return rtr;
        }
    }

    public class FunctionStatementR : StatementR
    {
        public FunctionStatementR(string[] tokens, string name) : base(tokens, name)
        {
        }

        public override string EvaluateAsm(string[] args)
        {
            return "";
        }

        public override Node getNode(string[] args, Node parent)
        {
            FunctionRef func = new FunctionRef();
            func.Name = args[0];
            func.parent = parent;
            return func;
        }
    }

    public class VariableR : ProductionRule
    {
        public VariableR(string[] tokens, string name) : base(tokens, name)
        {
        }

        public override string EvaluateAsm(string[] args)
        {
            return "";
        }

        public override Node getNode(string[] args, Node parent)
        {
            Variable var = null;
            if (args[0] == "int")
            {
                var = new Int();
                var.Name = args[1];
                if (Utils.IsDigitsOnly(args[2]))
                {
                    var.SetValue(int.Parse(args[2]));
                }
                else
                {
                    var.SetValue(0);
                    var.extra = "mov eax,[" + args[2] + "]" + "\n" +
                        "mov [%var%],eax";
                }
                var.parent = parent;
            }

            return var;
        }
    }

    public class AssignR : StatementR
    {
        public AssignR(string[] tokens, string name) : base(tokens, name)
        {
        }

        public override string EvaluateAsm(string[] args)
        {
            throw new NotImplementedException();
        }

        public override Node getNode(string[] args, Node parent)
        {
            Assign assign = null;

            if (name != "funcAssignment")
            {
                if (Utils.IsDigitsOnly(args[1]))
                {
                    assign = new Assign();
                    assign.Variable = args[0];
                    assign.expression = new IntConstant(int.Parse(args[1]));
                    assign.parent = parent;
                }
                else
                {
                    assign = new Assign();
                    assign.Variable = args[0];
                    Int ik = new Int();
                    ik.Name = args[0];
                    assign.expression = new IntRef(ik);
                    assign.expression.parent = assign;
                    assign.parent = parent;
                }
            }
            else
            {
                assign = new Assign();
                assign.Variable = args[0];
                FunctionRef function = new FunctionRef();
                function.Name = args[1];
                assign.expression = function;
                assign.parent = parent;
            }

            return assign;
        }
    }

    public static class Defs
    {
        public static ProductionRule[] grammer = {
            new FunctionR(new string[] { "func", "Identifier", "(", ")" }, "func"),
            new ReturnR(new string[] { "return", "Constant"}, "return"),
            new ReturnR(new string[] { "return", "Identifier"}, "return"),
            new FunctionStatementR(new string[] { "Identifier","(",")"},"funcRef"),
            new VariableR(new string[] { "Identifier","Identifier","=","Constant"},"variable"),
            new VariableR(new string[] { "Identifier","Identifier","=","Identifier"},"variable"),
            new AssignR(new string[] { "Identifier","=","Constant"},"assignment"),
            new AssignR(new string[] { "Identifier","=","Identifier"},"assignment"),
            new AssignR(new string[] { "Identifier","=","Identifier","(",")"},"funcAssignment")
        };
    }

    public class AbstractSyntaxTree
    {
        public Node root;

        public static AbstractSyntaxTree astFromTokens(List<Token> tokens)
        {
            AbstractSyntaxTree ast = new AbstractSyntaxTree();

            ast.root = new Program();
            bool canUpBranch = true;
            ref Node curr = ref ast.root;
            for (int i = 0; i < tokens.Count; i += 0)
            {
                Token tok = tokens[i];

                for (int k = 0; k < Defs.grammer.Length; k++)
                {
                    ProductionRule prRule = Defs.grammer[k];
                    int successes = 0;
                    int j = 0;

                    List<string> args = new List<string>();
                    bool match = false;
                    while (j < prRule.tokens.Length)
                    {
                        if (i >= tokens.Count)
                            break;
                        tok = tokens[i];

                        if (tok.value == "return")
                        {
                            Console.Write("");
                        }

                        if (tok.value == "INDENT" && curr.branches.Count != 0 && i != 0 && tokens[i - 1].value == "ENDLINE" && canUpBranch)
                        {
                            curr = curr.branches[curr.branches.Count - 1];
                            canUpBranch = false;
                        }
                        else if (tok.value == "INDENT" && curr.branches.Count != 0 && i != 0 && tokens[i - 1].value == "ENDLINE")
                        {

                        }
                        else if (curr.parent != null && tokens[i - 1].value == "ENDLINE")
                        {
                            curr = curr.parent;
                            canUpBranch = true;
                        }

                        match = false;
                        if (tok.value == prRule.tokens[j])
                        {
                            j++;
                            i++;
                            match = true;
                            successes++;
                        }
                        else if (tok.type == prRule.tokens[j])
                        {
                            j++;
                            i++;
                            match = true;
                            successes++;
                            args.Add(tok.value);
                        }
                        else if (tok.value == "INDENT")
                        {
                            i++;
                        }
                        else if (tok.value == "ENDLINE")
                        {
                            i++;
                        }
                        else
                        {
                            //i++;
                            if (match == false)
                            {
                                if (k + 1 != Defs.grammer.Length)
                                {
                                    prRule = Defs.grammer[++k];
                                    j = 0;
                                }
                                else
                                {
                                    prRule = Defs.grammer[0];
                                    k = 0;
                                    j = 0;
                                }
                                for (int s = 0; s < successes; s++)
                                {
                                    i--;
                                }
                                successes = 0;
                                args.Clear();
                            }
                        }


                    }
                    if (match)
                    {
                        Node node = prRule.getNode(args.ToArray(), curr);
                        if (node != null)
                            curr.branches.Add(node);
                        continue;
                    }
                    //if (k == Defs.grammer.Length - 1)
                    //{
                    //    i--;
                    //}
                }
            }

            while (curr.parent != null)
            {
                curr = curr.parent;
            }

            return ast;
        }

        public static Node Parse(List<Token> tokens)
        {
            Node n = new Nodes.IntConstant(0);



            return n;
        }

        /*public static AbstractSyntaxTree FromTokenArray(List<Token> tokens)
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
                                    function.branches.Add(new Return(new IntConstant(int.Parse(lne[2].value.Substring("CONSTANT:".Length)))));
                                }
                                else if (lne[2].value == "++")
                                {
                                    function.branches.Add(new Return(new Increment(int.Parse(lne[2].value.Substring("CONSTANT:".Length)))));
                                }
                            }
                            catch (Exception)
                            {
                                if (lne[2].value == "++")
                                {
                                    function.branches.Add(new Return(new Increment(int.Parse(lne[3].value.Substring("CONSTANT:".Length)))));
                                }
                            }
                        }


                        j++;
                    }

                    functions.Add(function);
                    i = j - 1;
                }
            }

            ast.root.branches.AddRange(functions);

            return ast;
        }*/
    }

    public abstract class Node
    {
        public List<Node> branches = new List<Node>();
        public Node parent;

        public abstract string EvaluateAsm();

        public int getBranchSize()
        {
            return branches.Count;
        }

        public List<Node> getBranches()
        {
            return branches;
        }

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