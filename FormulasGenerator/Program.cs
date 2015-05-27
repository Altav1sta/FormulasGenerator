using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulasGenerator
{
    class Program
    {
        public class Node
        {
            public string value;
            public string unar;
            public int index;
            public Node left = null;
            public Node right = null;
            public Node parent = null;

            public string getFormula()
            // Возвращает матформулу от текущего узла в виде строки
            {
                if (left == null)
                {
                    if (unar == null) return value;
                    else return unar + "(" + value + ")";
                }
                else
                {
                    if (unar == null)
                    {
                        if (parent != null) return "(" + left.getFormula() + " " + value + " " + right.getFormula() + ")";
                        else return left.getFormula() + " " + value + " " + right.getFormula();
                    }
                    else return unar + "(" + left.getFormula() + " " + value + " " + right.getFormula() + ")";
                }                
            }

            public Node getElementAt(int pos)
            // Возвращает узел с заданным индексом
            {
                if (index == pos) return this;
                else
                {
                    if (value == null) return null;
                    else
                    {
                        if ((left != null) && (right != null))
                        {
                            Node l = left.getElementAt(pos);
                            if (l != null) return l;
                            else return right.getElementAt(pos);
                        }
                        else return null;
                    }
                }
            }

            public int[] getEmptyNodes(/*ref int[] arr, int pos*/)
            // Заполняет список, переданный по ссылке в аргументе, индексами пустых узлов
            {
                if (value == null)
                {
                    int[] ans = new int[1];
                    ans[0] = index;
                    return ans;
                }
                else
                {
                    if ((left != null) && (right != null))
                    {
                        int[] tmp = left.getEmptyNodes();

                        if (tmp == null) return right.getEmptyNodes();

                        tmp = right.getEmptyNodes();

                        if (tmp == null) return left.getEmptyNodes();
                        else return left.getEmptyNodes().Concat(right.getEmptyNodes()).ToArray();
                    }
                    else return null;
                }
            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Введите названия переменных через пробел:");
            string[] vars = Console.ReadLine().Split(new Char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("Введите числа через пробел:");
            string[] nums = Console.ReadLine().Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            

            // Объединяем переменные и числа как один общий массив аргументов
            string[] arg = new string[vars.Length + nums.Length];
            arg = vars.Concat(nums).ToArray();


            Console.WriteLine("Введите названия унарных функций через пробел:");
            string[] unars = Console.ReadLine().Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("Введите бинарные операции через пробел:");
            string[] binars = Console.ReadLine().Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            bool[] bbinars = new bool[binars.Length]; // Индикатор того, использован ли уже соотв. элемент из массива выше
            Console.WriteLine();


            // Заполняем дерево бинарными операциями
            Random r = new Random();
            Random r2 = new Random();
            Node head = new Node();
            head.index = -1;
            for (int i = 0; i < binars.Length; i++)
            {                
                int[] arr = head.getEmptyNodes();
                int pos = r.Next(0, arr.Length);
                Node el = head.getElementAt(arr[pos]); // Выбрали случайный пустой узел
                // Ищем неиспользованную бинарную операцию
                int ind = -1;
                while (ind == -1)
                {
                    ind = r2.Next(0, binars.Length);
                    if (bbinars[ind] == true) ind = -1;
                }
                // Вставляем заполняем ячейку подходящей операцией и создаем два потомка
                el.value = binars[ind];
                el.left = new Node();
                el.left.parent = el;
                el.left.index = 2 * i;
                el.right = new Node();
                el.right.parent = el;
                el.right.index = 2 * i + 1;
                bbinars[ind] = true;
            }



            // Добавляем числа и переменные            
            for (int i = 0; i < binars.Length + 1; i++)
            {
                int[] ar = head.getEmptyNodes();
                Node el = head.getElementAt(ar[0]);
                // Вставляем случайный аргумент в пустой узел
                int ind = -1;
                while (ind == -1)
                {
                    ind = r.Next(0, arg.Length);
                    if ((el.parent.left.value == arg[ind]) || (el.parent.right.value == arg[ind])) ind = -1;
                }
                el.value = arg[ind];
            }


            // Добавляем унарные операции
            for (int i = 0; i < unars.Length; i++)
            {
                // Выбираем случайную ячейку дерева
                int pos = r.Next(-1, binars.Length * 2);
                Node el = head.getElementAt(pos);
                // Если к этой ячейке уже применена унарная операция - выбираем другую
                if (el.unar != null)
                {
                    i--;
                    continue;
                }
                el.unar = unars[i];
            }


            Console.WriteLine();
            Console.WriteLine("f = " + head.getFormula());
            Console.ReadLine();
        }
    }
}
