using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr.Utils
{
    public class AbstractSyntaxTree
    {
        public Node root;
    }

    public abstract class Node {
        List<Node> branches = new List<Node>();


    }
}
