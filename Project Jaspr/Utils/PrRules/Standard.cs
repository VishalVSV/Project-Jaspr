using Project_Jaspr.Utils.Nodes;
using System;

namespace Project_Jaspr.Utils.PrRules
{
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

    public static class Standard
    {
        public class AdditionR : ProductionRule
        {
            public AdditionR(string[] tokens, string name) : base(tokens, name)
            {
            }

            public override string EvaluateAsm(string[] args)
            {
                throw new NotImplementedException();
            }

            public override Node getNode(string[] args, Node parent)
            {
                Addition add = new Addition();

                for (int i = 0; i < args.Length; i++)
                {
                    if(Utils.IsDigitsOnly(args[i]))
                    {
                        IntConstant ic = new IntConstant(int.Parse(args[i]));
                        ic.parent = add;
                        add.branches.Add(ic);
                    }else
                    {
                        VarRef iref = new IntRef(args[i]);
                        iref.parent = add;
                        add.branches.Add(iref);
                    }
                }

                add.parent = parent;

                return add;
            }
        }

        public class IncrementR : ProductionRule
        {
            public IncrementR(string[] tokens, string name) : base(tokens, name)
            {
            }

            public override string EvaluateAsm(string[] args)
            {
                throw new NotImplementedException();
            }

            public override Node getNode(string[] args, Node parent)
            {
                string arg = args[0];

                if (parent.GetType() == typeof(Function))
                {
                    arg = ((Function)parent).Name + arg;
                }

                Increment inc = new Increment(arg);
                inc.parent = parent;
                return inc;
            }
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
                    IntRef rf = new IntRef(args[0]);
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

                if (name == "funcAssignment")
                {
                    assign = new Assign();
                    assign.Variable = args[0];
                    FunctionRef function = new FunctionRef();
                    function.Name = args[1];
                    assign.expression = function;
                    assign.parent = parent;
                }
                else if (name == "assignmentExp")
                {
                    assign = new Assign();
                    assign.Variable = args[0];
                    assign.parent = parent;
                }
                else
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
                        assign.expression = new IntRef(args[0]);
                        assign.expression.parent = assign;
                        assign.parent = parent;
                    }
                }

                return assign;
            }
        }
    }
}
