using Project_Jaspr.Utils.PrRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project_Jaspr.Utils.PrRules.Standard;

namespace Project_Jaspr.Utils
{
    public static class Defs
    {
        public static ProductionRule[] grammer = {
            new FunctionR(new string[] { "func", "Identifier", "(", ")" }, "function"),
            new ReturnR(new string[] { "return", "Constant||Identifier"}, "rtrn"),
            new FunctionStatementR(new string[] { "Identifier","(",")"},"funcRef"),
            new VariableR(new string[] { "Identifier","Identifier","=","Constant||Identifier"},"variable"),
            new AssignR(new string[] { "Identifier","=","Constant||Identifier"},"assignment"),
            new AssignR(new string[] { "Identifier","=","addition"},"assignmentExp"),
            new AssignR(new string[] { "Identifier","=","Identifier","(",")"},"funcAssignment"),
            new IncrementR(new string[] { "Identifier","+","+"},"increment"),
            new AdditionR(new string[] { "Constant||Identifier","+","Constant||Identifier"},"addition"),
            new AdditionR(new string[] { "Constant||Identifier","+","addition"},"additionRec")
        };

        public static PreProcessInstruction[] preProcessInstructions =
        {
            new PreProcessInstruction(new string[] { "Identifier","+","="},new string[] { "0","=","0","+"})
        };

        public static bool Contains(string Productionrule)
        {
            foreach (ProductionRule prRule in grammer)
            {
                if (prRule.name == Productionrule)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class PrExt
    {
        public static bool Contains(this ProductionRule[] rules, string rule)
        {
            foreach (ProductionRule rle in rules)
            {
                if (rle.name == rule)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Contains(this ProductionRule[] prRules, IEnumerable<string> rules)
        {
            foreach (string rule in rules)
            {
                if (prRules.Contains(rule))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
