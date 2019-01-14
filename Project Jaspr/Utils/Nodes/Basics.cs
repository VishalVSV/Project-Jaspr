using System;
using System.Collections.Generic;

namespace Project_Jaspr.Utils.Nodes
{
    public static class Exceptions
    {
        public class SyntaxError : Exception
        {

        }
    }

    public static class NodeUtils
    {
        public static bool ParentedByFunction(this Node node, out string funcName)
        {
            bool parentedByFunc = false;
            Node func = node;
            funcName = "";
            while (func.parent != null)
            {
                if (func.parent.GetType() == typeof(Function))
                {
                    parentedByFunc = true;
                    funcName = ((Function)func.parent).Name;
                    break;
                }
                else
                {
                    func = func.parent;
                    funcName = "";
                }
            }
            if (!parentedByFunc)
                funcName = "";

            return parentedByFunc;
        }
    }

    public class Program : Node
    {
        public new List<Node> branches = new List<Node>();

        public Program()
        {

        }

        public override string ToString()
        {
            return "Program";
        }

        public override string EvaluateAsm()
        {
            string res = "";
            List<Node> br = getBranches();
            for (int i = 0; i < getBranchSize(); i++)
            {
                res += getBranches()[i].EvaluateAsm() + "\n";
            }

            return res;
        }
    }

    public class FunctionRef : Expression
    {

        public string Name = "";

        public override string EvaluateAsm()
        {
            return "call " + Name;
        }
    }

    public class Function : Node
    {
        public new List<Statement> branches = new List<Statement>();
        public string Name;

        public Function()
        {

        }

        public override string ToString()
        {
            return "Function :" + Name;
        }

        public override string EvaluateAsm()
        {
            string res = Name + ":\n";

            for (int i = 0; i < getBranchSize(); i++)
            {
                res += getBranches()[i].EvaluateAsm() + "\n";
            }

            return res;
        }
    }

    public abstract class Operation : Expression
    {

    }

    public abstract class BinaryOp : Operation
    {
        public abstract string GetMacro();

        public override string EvaluateAsm()
        {
            string asm = "";

            for (int i = 0; i < branches.Count; i++)
            {
                Expression Exp = (Expression)branches[i];

                if (Exp.GetType() == typeof(Addition))
                {
                    branches.AddRange(Exp.branches);
                    branches.Remove(Exp);
                }
            }

            if (branches.Count == 0)
                throw new Exception("Syntax Error");

            foreach (Expression exp in branches)
            {
                asm += "push " + exp.GetIdentifier() + "\n";
            }
            asm += "mov edx," + branches.Count.ToString() + "\n";
            asm += GetMacro();

            return asm;
        }
    }

    public class Addition : BinaryOp
    {
        public override string GetMacro()
        {
            return "addNumbers";
        }
    }

    public abstract class Statement : Node
    {
        public string ASM;

        public abstract string Evaluate();
    }

    public class Return : Statement
    {
        public new string ASM = @"{0}
ret";

        public Return(Expression exp)
        {
            branches.Add(exp);
        }

        public override string Evaluate()
        {
            if (parent.GetType() == typeof(Function))
            {
                return ASM.Replace("{0}", ((Expression)branches[0]).EvaluateAsm());
            }
            else
                throw new Exception("Syntax Error");
        }

        public override string EvaluateAsm()
        {
            return Evaluate();
        }
    }

    public abstract class Expression : Node
    {
        public virtual string GetIdentifier()
        {
            return "";
        }
    }

    public class Increment : Expression
    {
        string var = "";

        public Increment(string arg0)
        {
            if (Utils.IsDigitsOnly(arg0))
            {

            }
            else
            {
                var = arg0;
            }
        }

        public override string EvaluateAsm()
        {
            return "add DWORD [" + var + "],1";
        }
    }

    public abstract class Variable : Node
    {
        public string Name;
        public string extra = "";

        public abstract string GetValue();

        public abstract void SetValue(object a);

        public Node GetParent()
        {
            return parent;
        }

        public override string EvaluateAsm()
        {
            string ext = extra;
            ext = ext.Replace("%var%", Name);

            string nme = Name;
            Node prnt = GetParent();
            if (parent != null && parent.GetType() == typeof(Function))
            {
                nme = ((Function)parent).Name + Name;
            }

            return "{" + nme + " dd " + GetValue() + "}" + "\n" + ext;
        }
    }

    public class Int : Variable
    {
        public int value = 0;

        public override string GetValue()
        {
            return value.ToString();
        }

        public override string ToString()
        {
            return "Int :" + Name;
        }

        public override void SetValue(object a)
        {
            value = (int)a;
        }
    }

    public abstract class VarRef : Expression
    {
        string name;

        public VarRef(string var)
        {
            this.name = var;
        }

        public override string ToString()
        {
            return "IntRef :" + name;
        }

        public abstract string GetSize();

        public override string GetIdentifier()
        {
            string funcName;
            if (this.ParentedByFunction(out funcName))
            {
                return GetSize() + " [" + funcName + name + "]";
            }
            else
            {
                return GetSize() + " [" + name + "]";
            }
        }

        public override string EvaluateAsm()
        {
            bool parentedByFunc = false;
            string funcName = "";
            Node func = this;

            while (func.parent != null)
            {
                if (func.parent.GetType() == typeof(Function))
                {
                    parentedByFunc = true;
                    funcName = ((Function)func.parent).Name;
                    break;
                }
                else
                {
                    func = func.parent;
                }
            }

            if (parentedByFunc)
            {
                return "mov eax," + GetSize() + " [" + funcName + name + "]";
            }
            else
            {
                return "mov eax," + GetSize() + " [" + name + "]";
            }
        }
    }

    public class IntRef : VarRef
    {
        public IntRef(string var) : base(var)
        {
        }

        public override string GetSize()
        {
            return "DWORD";
        }
    }

    public class IntConstant : Expression
    {
        int val;

        public IntConstant()
        {

        }

        public IntConstant(int value)
        {
            val = value;
        }

        public override string GetIdentifier()
        {
            return val.ToString();
        }

        public override string EvaluateAsm()
        {
            return "mov eax," + val.ToString();
        }

        public int GetValue()
        {
            return val;
        }
    }



    public class Assign : Statement
    {
        public string Variable;
        public Expression expression;

        public override string Evaluate()
        {
            throw new NotImplementedException();
        }

        public override string EvaluateAsm()
        {
            if (expression == null)
            {
                if (branches.Count > 0 && branches[branches.Count - 1] is Expression)
                    expression = (Expression)branches[branches.Count - 1];
            }


            if (expression.GetType() != typeof(FunctionRef))
            {
                if (parent != null && parent.GetType() == typeof(Function))
                {
                    return expression.EvaluateAsm() + "\n" + "mov " + "[" + ((Function)parent).Name + Variable + "],eax";
                }
                else
                {
                    return expression.EvaluateAsm() + "\n" + "mov " + "[" + Variable + "],eax";
                }
            }
            else
            {
                if (parent != null && parent.GetType() == typeof(Function))
                {
                    return expression.EvaluateAsm() + "\n" +
     "mov " + "[" + ((Function)parent).Name + Variable + "],eax";
                }
                else
                {
                    return expression.EvaluateAsm() + "\n" +
     "mov " + "[" + Variable + "],eax";
                }
            }
        }
    }
}
