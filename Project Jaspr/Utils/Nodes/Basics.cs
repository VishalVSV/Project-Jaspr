using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils.Nodes
{
    public class Program : Node
    {
        public Program()
        {

        }
    }

    public class Function : Node
    {
        public new List<Statement> branches = new List<Statement>();
        public string Name;

        public Function()
        {

        }
    }

    public abstract class Statement : Node
    {
        public string ASM;

        public abstract string Evaluate();
    }

    public class Return : Statement
    {
        public new string ASM = @"push {0}
call _ExitProcess@4";

        public Return(int i)
        {
            branches.Add(new IntConstant(i));
        }

        public override string Evaluate()
        {
            return ASM.Replace("{0}",((IntConstant)branches[0]).GetValue().ToString());
        }
    }

    public class IntConstant : Node
    {
        int val;

        public IntConstant()
        {

        }

        public IntConstant(int value)
        {
            val = value;
        }

        public int GetValue()
        {
            return val;
        }
    }
}
