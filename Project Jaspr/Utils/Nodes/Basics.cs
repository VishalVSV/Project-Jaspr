using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils.Nodes
{
    public static class Exceptions
    {
        public class SyntaxError : Exception
        {

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

    public abstract class Statement : Node
    {
        public string ASM;

        public abstract string Evaluate();
    }

    public class Return : Statement
    {
        public new string ASM = @"mov eax, {0}
ret";

        public Return(Expression exp)
        {
            branches.Add(exp);
        }

        public override string Evaluate()
        {
            if (parent.GetType() == typeof(Function))
                return ASM.Replace("{0}", ((Expression)branches[0]).EvaluateAsm());
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
    }

    public class Increment : Expression
    {
        public IntConstant intConst = new IntConstant();

        public Increment(int i)
        {
            intConst = new IntConstant(i);
        }

        public override string EvaluateAsm()
        {
            return (intConst.GetValue() + 1).ToString();
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

            return "{" + nme + " dw " + GetValue() + "}" + "\n" + ext;
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

    public class IntRef : Expression
    {
        Int var;

        public IntRef(Int var)
        {
            this.var = var;
        }

        public override string ToString()
        {
            return "IntRef :"+var.Name;
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
                return "[" + funcName + var.Name + "]";
            }
            else
            {
                return "[" + var.Name + "]";
            }
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

        public override string EvaluateAsm()
        {
            return val.ToString();
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
            if (expression.GetType() != typeof(FunctionRef))
            {
                if (parent != null && parent.GetType() == typeof(Function))
                {
                    return "mov DWORD " + "[" + ((Function)parent).Name + Variable + "]," + expression.EvaluateAsm();
                }
                else
                {
                    return "mov DWORD " + "[" + Variable + "]," + expression.EvaluateAsm();
                }
            }
            else
            {
                if (parent != null && parent.GetType() == typeof(Function))
                {
                    return expression.EvaluateAsm() + "\n" +
     "mov DWORD " + "[" + ((Function)parent).Name + Variable + "],eax";
                }
                else
                {
                    return expression.EvaluateAsm() + "\n" +
     "mov DWORD " + "[" + Variable + "],eax";
                }
            }
        }
    }
}
