using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class HuffmanCoding
{
    private class Node : IComparable
    {
        public char symbol { get; set; }
        public double frequency { get; set; }
        public bool isVisited { get; set; }
        //Левый и правый дочерние элементы, если они оба равны null, то данный узел - лист
        public Node left { get; set; }
        public Node right { get; set; }

        public Node() { isVisited = false; }
        //Полноценный узел
        public Node(char s, double f, Node l, Node r)
        {
            symbol = s;
            frequency = f;
            left = l;
            right = r;
            isVisited = false;
        }
        //Лист
        public Node(char s, double f)
        {
            symbol = s;
            frequency = f;
            left = null;
            right = null;
            isVisited = false;
        }
        //Узел полученный объединением двух других узлов
        public Node(Node a, Node b)
        {
            symbol = '\0';
            isVisited = false;
            frequency = a.frequency + b.frequency;
            if (a.frequency < b.frequency)
            {
                left = a;
                right = b;
            }
            else
            {
                left = b;
                right = a;
            }
        }

        public int CompareTo(object obj)
        {
            Node otherNode = obj as Node;
            return frequency.CompareTo(otherNode.frequency);
        }
    }

    private static int SymbolCount;
    private static Node CreateSymbolTree(Dictionary<char, double> SymbolFrequencyDictionary)
    {
        //Дерево символов
        List<Node> SymbolTree = new List<Node>();

        //Очередь из всех символов (отсортирована по частоте)
        Queue<Node> Symbols = new Queue<Node>();
        foreach (KeyValuePair<char, double> symbolItem in SymbolFrequencyDictionary)
        {
            Node newNode = new Node(symbolItem.Key, symbolItem.Value);
            Symbols.Enqueue(newNode);
        }
        SymbolCount = Symbols.Count;

        //Добавляем в дерево первый узел
        Node firstNode;
        if (Symbols.Count > 1)
        {
            firstNode = new Node(Symbols.Dequeue(), Symbols.Dequeue());
        }
        else
        {
            firstNode = Symbols.Dequeue();
        }
        SymbolTree.Add(firstNode);

        //Создаём дерево символов
        while (Symbols.Count > 0)
        {
            //Сортируем дерево по частоте символов
            SymbolTree.Sort();
            bool nodeFromQueue = false;
            //Для создания нового узла в дереве возьмём два узла - newNode1 и newNode2
            Node newNode1 = new Node();
            Node newNode2 = new Node();
            if (SymbolTree.Count > 1)
            {
                //newNode1 изначально присваиваем узел в дереве символов с самой маленькой частотой
                newNode1 = SymbolTree[0];

                //Если в дереве символов частота самого маленького символа больше, чем частота следующего символа в очереди, то
                //меняем newNode1 на следующий символ в очереди
                if ((Symbols.Count > 0) && (newNode1.frequency > Symbols.Peek().frequency))
                {
                    newNode1 = Symbols.Dequeue();
                }
                //В другом случае newNode1 не меняется и мы убираем из дерева символов этот узел
                else
                {
                    SymbolTree.RemoveAt(0);
                }
            }
            else
            {
                nodeFromQueue = true;
                newNode1 = Symbols.Dequeue();
            }
            if (SymbolTree.Count >= 1)
            {
                //Повторяем ту же операцию для newNode2
                newNode2 = SymbolTree[0];
                if ((Symbols.Count > 0) && (newNode2.frequency > Symbols.Peek().frequency))
                {
                    newNode2 = Symbols.Dequeue();
                }
                else
                {
                    SymbolTree.RemoveAt(0);
                }

                //Объединяем 2 узла в 1 и добавляем из в дерево
                Node connectedNodes = new Node(newNode1, newNode2);
                SymbolTree.Add(connectedNodes);
            }
            //Если newNode1 и newNode2 берём из очереди
            else if ((Symbols.Count > 0) && (!nodeFromQueue))
            {
                newNode2 = Symbols.Peek();
                Node connectedNode = new Node(newNode1, newNode2);
                SymbolTree.Add(connectedNode);
            }
            //Если в качестве newNode2 нечего взять
            else
            {
                SymbolTree.Add(newNode1);
            }
        }

        //Объединяем последние узлы в дереве в один
        while (SymbolTree.Count > 1)
        {
            SymbolTree.Sort();
            Node finalNode1 = SymbolTree[0];
            Node finalNode2 = SymbolTree[1];
            SymbolTree.RemoveAt(0);
            SymbolTree.RemoveAt(0);
            SymbolTree.Add(new Node(finalNode1, finalNode2));
        }
        return SymbolTree[0];
    }
    /// <summary>
    /// На вход этому методу подаётся частотный словарь, отстортированный по частоте, на выходе даётся словарь символов с кодировкой
    /// </summary>
    public static Dictionary<char, string> CreateSymboleCodeDictionary(Dictionary<char, double> SymbolFrequencyDictionary)
    {
        Dictionary<char, string> SymbolCodeDictionary = new Dictionary<char, string>();

        //Дерево символов
        Node SymbolTree = CreateSymbolTree(SymbolFrequencyDictionary);

        //Переменная для задания кодов символов
        StringBuilder SymbolCode = new StringBuilder();

        //Создаём стек для обхода дерева и заносим бервую вершину
        Stack<Node> nodeStack = new Stack<Node>();
        nodeStack.Push(SymbolTree);

        //Выполняем обход дерева
        while (SymbolCount != SymbolCodeDictionary.Count)
        {
            Node currentNode = nodeStack.Peek();

            //Если у текущей вершины есть символ, то присваиваем ей код
            if ((currentNode.symbol != '\0') && (!currentNode.isVisited))
            {
                if (SymbolCode.Length == 0)
                    SymbolCode.Append('0');
                SymbolCodeDictionary.Add(currentNode.symbol, SymbolCode.ToString());
            }
            currentNode.isVisited = true;

            //Если есть вершина слева, то перемещаемся в неё и добавляем в код символа 0
            if ((currentNode.left != null) && (!currentNode.left.isVisited))
            {
                SymbolCode.Append('0');
                nodeStack.Push(currentNode.left);
            }
            //Если есть вершина справа, то перемещаемся в неё и добавляем в код символа 1
            else if ((currentNode.right != null) && (!currentNode.right.isVisited))
            {
                SymbolCode.Append('1');
                nodeStack.Push(currentNode.right);
            }
            //Иначе идём на 1 вершину назад и убираем из кода символа последний 0/1
            else
            {
                if (SymbolCode.Length > 0)
                {
                    SymbolCode.Remove(SymbolCode.Length - 1, 1);
                }
                nodeStack.Pop();
            }
        }
        return SymbolCodeDictionary;
    }
}
