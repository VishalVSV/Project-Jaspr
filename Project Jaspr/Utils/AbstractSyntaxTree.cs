using System;
using System.Collections.Generic;
using static Project_Jaspr.Utils.PrRules.Standard;
using Project_Jaspr.Utils.PrRules;

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

    public class PreProcessInstruction
    {
        public string[] from;
        public string[] to;

        public PreProcessInstruction(string[] from,string[] to)
        {
            this.from = from;
            this.to = to;
        }
    }

    public class AbstractSyntaxTree
    {
        public Node root;
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
            {
                if (branches[i] != null)
                branches[i].PrintPretty(indent, i == branches.Count - 1);
            }
        }
    }
}