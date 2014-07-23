using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryTree
{
    class Program
    {
        class Node 
        {
            protected int value;
            
            protected Node parentNode;
            protected Node leftNode;
            protected Node rightNode;
            
            public Node(int value)
            {
                this.value = value;
            }
            
            public int GetValue()
            {
                return this.value;
            }

            public Node GetParentNode()
            {
                return this.parentNode;
            }
            
            public Node GetLeftNode()
            {
                return this.leftNode;
            }
            
            public Node GetRightNode()
            {
                return this.rightNode;
            }
            
            public void SetValue(int value)
            {
                this.value = value;
            }
            
            public virtual void SetLeftNode(Node value)
            {
                this.leftNode = value;
                value.parentNode = this;
            }
            
            public virtual void SetRightNode(Node value)
            { 
                this.rightNode = value;
                value.parentNode = this;
            }
            
            static public void Traverse(Node node, string list)
            {
                if (node == null)
                {
                    return;
                }
            
                list += node.value;
                
                if (node.leftNode == null && node.rightNode == null)
                {
                    Console.WriteLine(list);
                }
                else
                {    
                    list += "-";
                
                    Traverse(node.leftNode, list);
                    Traverse(node.rightNode, list);
                }
            }
        }
        
        static void Main(string[] args)
        {
            Node[] node = 
            {
                new Node(1),
                new Node(2),
                new Node(3),
                new Node(4),
                new Node(5),
                new Node(6)
            };
            
            node[0].SetLeftNode(node[1]);
            node[0].SetRightNode(node[2]);
            node[1].SetLeftNode(node[3]);
            node[3].SetRightNode(node[4]);
            
            Node.Traverse(node[0], "");
            Console.ReadKey();
        }
    }
}
