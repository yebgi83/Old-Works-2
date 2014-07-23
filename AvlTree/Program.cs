using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryTree
{
    class Program
    {
        class AvlNode 
        {
            protected int height;
            protected int value;
            
            protected AvlNode parentNode;
            protected AvlNode leftNode;
            protected AvlNode rightNode;
            
            public AvlNode(int value)
            {
                this.value = value;
            }
            
            public int GetHeight()
            {
                return this.height;
            }
            
            public int GetValue()
            {
                return this.value;
            }
            
            public int GetBalanceFactor()
            {
                return ((this.leftNode != null) ? (this.leftNode.GetHeight() + 1) : 0) - ((this.rightNode != null) ? (this.rightNode.GetHeight() + 1) : 0);
            }
            
            public AvlNode GetParentNode()
            {
                return this.parentNode;
            }
            
            public AvlNode GetLeftNode()
            {
                return this.leftNode;
            }
            
            public AvlNode GetRightNode()
            {
                return this.rightNode;
            }
            
            public void SetValue(int value)
            {
                this.value = value;
            }
            
            public virtual void SetLeftNode(AvlNode value)
            {
                this.leftNode = value;
                this.height = Math.Max((this.leftNode != null) ? this.leftNode.height + 1 : 0, (this.rightNode != null) ? this.rightNode.height + 1 : 0);
                
                if (value == null)
                {
                    return;
                }
                
                value.parentNode = this;
                
                AvlNode node = value;
                
                while (node.parentNode != null)
                {
                    node.parentNode.height = node.height + 1;
                    node = node.parentNode;
                }
                
                Rotate(value);
            }
            
            public virtual void SetRightNode(AvlNode value)
            { 
                this.rightNode = value;
                this.height = Math.Max((this.leftNode != null) ? this.leftNode.height + 1 : 0, (this.rightNode != null) ? this.rightNode.height + 1 : 0);
                
                if (value == null)
                {
                    return;
                }
                
                value.parentNode = this;
                
                AvlNode node = value;
                
                while (node.parentNode != null)
                {
                    node.parentNode.height = node.height + 1;
                    node = node.parentNode;
                }

                Rotate(value);
            }
            
            public void Rotate(AvlNode value)
            {
                int current_direction = 0, child_direction = 0;
                
                AvlNode parent = this.parentNode;
                
                if (parent == null)
                {
                    return;
                }
                
                int balanceFactor = parent.GetBalanceFactor();
                
                if (balanceFactor <= 1)
                {
                    return;
                }
                else if (balanceFactor <= -1)
                {
                    return;
                }
                
                if (parent.leftNode == this)
                {
                    current_direction = 1;
                }
                else if (parent.rightNode == this)
                {
                    current_direction = -1;
                }
                
                if (this.leftNode == value)
                {
                    child_direction = 1;
                }
                else if (this.rightNode == value)
                {
                    child_direction = -1;
                }
                
                if (current_direction == 1 && child_direction == 1)
                {
                    // Left-Left
                    RotateClockwise(this.parentNode, this, this.leftNode);
                }
                else if (current_direction == -1 && child_direction == -1)
                {
                    // Right-Right
                    RotateAntiClockwise(this.parentNode, this, this.rightNode);
                }
                else if (current_direction == 1 && child_direction == -1)
                {
                    // Left-Right
                    RotateAntiClockwise(this.parentNode, this, this.rightNode);
                }
                else if (current_direction == -1 && child_direction == 1)
                {   
                    // Right-Left
                    RotateClockwise(this.parentNode, this, this.rightNode);
                }
            }

            private void RotateAntiClockwise(AvlNode parentNode, AvlNode currentNode, AvlNode childNode)
            {
                AvlNode ancestorNode = parentNode.parentNode;
                
                if (ancestorNode == null)
                {
                    return;
                }
                
                if (ancestorNode.leftNode == parentNode)
                {
                    ancestorNode.SetLeftNode(currentNode);
                }
                else if (ancestorNode.rightNode == parentNode)
                {
                    ancestorNode.SetRightNode(currentNode);
                }
                
                currentNode.SetLeftNode(parentNode);
                
                parentNode.SetLeftNode(null);
                parentNode.SetRightNode(null);
            }
            
            private void RotateClockwise(AvlNode parentNode, AvlNode currentNode, AvlNode childNode)
            {
                AvlNode ancestorNode = parentNode.parentNode;
                
                if (ancestorNode == null)
                {
                    return;
                }
                
                if (ancestorNode.leftNode == parentNode)
                {
                    ancestorNode.SetLeftNode(currentNode);
                }
                else if (ancestorNode.rightNode == parentNode)
                {
                    ancestorNode.SetRightNode(currentNode);
                }
                
                currentNode.SetRightNode(parentNode);
            }
            
            static public void Traverse(AvlNode node, string list)
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
            AvlNode[] node = 
            {
                new AvlNode(1),
                new AvlNode(2),
                new AvlNode(3),
                new AvlNode(4),
                new AvlNode(5),
                new AvlNode(6),
                new AvlNode(7)
            };
            
            node[0].SetLeftNode(node[1]);
            node[0].SetRightNode(node[2]);
            node[1].SetLeftNode(node[3]);
            node[3].SetRightNode(node[4]);
            node[4].SetRightNode(node[5]);
            node[2].SetRightNode(node[6]);
            
            AvlNode.Traverse(node[0], "");
            Console.ReadKey();
        }
    }
}
