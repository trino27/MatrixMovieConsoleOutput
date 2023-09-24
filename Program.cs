using System;
using System.Collections.Generic;
using System.Threading;

namespace CyberHW13
{
    class Program
    {
        const int consoleWidth = 160;
        const int consoleHeight = 40;

        public static object locker = new object();

        public static bool StartIndexPainter(List<int> SubRowStartIndex, int index)
        {
            for (int i = 0; i < SubRowStartIndex.Count; i++)
            {
                if (SubRowStartIndex[i] == index)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool AllowCreateSubRow()
        {
            Random random = new Random();
            if (random.Next(0, consoleHeight) < 1 )
            {
                return true;
            }
            else return false;
        }

        public static void ShowRow(List<int> SubRowStartIndex, char[] Row, object RowIndex)
        {
            lock (locker)
            {
                for (int i = consoleHeight - 1; i >= 0; i--)
                {

                    for (int j = 0; j < SubRowStartIndex.Count; j++)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(RowIndex), i);
                        if (SubRowStartIndex[j] - 1 == i || StartIndexPainter(SubRowStartIndex, i + 1))
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(Row[i]);
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                        }
                        else if (SubRowStartIndex[j] - 2 == i || StartIndexPainter(SubRowStartIndex, i + 2))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(Row[i]);
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                        }
                        else Console.WriteLine(Row[i]);
                    }
                }

            }
        }
        public static void SubRowMove(List<int> SubRowStartIndex, List<char[]> SubRowCharList, char[] Row)
        {
            for (int i = consoleHeight - 1; i > 0; i--)
            {
                Row[i] = Row[i - 1];
                if (Row[i] != ' ') Row[i]++;
            }

            if (SubRowStartIndex[^1] <= SubRowCharList[^1].Length)
            {
                Row[0] = SubRowCharList[^1][SubRowStartIndex[^1] - 1];
            }
            else Row[0] = ' ';
        }
        public static void Action(object RowIndex, char[] Row)
        {
            List<int> SubRowStartIndex = new List<int>();
            List<char[]> SubRowCharList = new List<char[]>();

            bool IsRowEmpty = true;
            bool IsCreateSubRowAllow = false;
            bool IsSubRowExist = false;

            do
            {
                if (AllowCreateSubRow())
                {
                    IsCreateSubRowAllow = true;
                    IsRowEmpty = false;
                    SubRowStartIndex.Add(-1);
                }
                else
                {
                    bool IsCharExist = false;
                    for (int i = 0; i < Row.Length; i++)
                    {
                        if (Row[i] != ' ')
                        {
                            IsCharExist = true;
                        }
                    }
                    if (!IsCharExist)
                    {
                        IsRowEmpty = true;
                    }
                }
                
                if (IsRowEmpty)
                {
                    IsCreateSubRowAllow = false;
                    IsSubRowExist = false;

                    SubRowStartIndex = new List<int>();
                    SubRowCharList = new List<char[]>();
                    
                }
                if (IsCreateSubRowAllow)
                {
                    IsCreateSubRowAllow = false;
                    IsSubRowExist = true;

                    SubRowCharList.Add(CreateSubRow());
                    SubRowStartIndex[^1] = 1;
                    
                }
                if (IsSubRowExist)
                {
                    SubRowMove(SubRowStartIndex, SubRowCharList, Row);
                    ShowRow(SubRowStartIndex, Row, RowIndex);
                    for (int i = 0; i < SubRowStartIndex.Count; i++)
                    {
                        if (SubRowStartIndex[i] != consoleHeight + SubRowCharList[i].Length)
                        {
                            SubRowStartIndex[i]++;
                        }
                    }
                }

                Thread.Sleep(500);
            } while (true);
        }

        public static char[] CreateSubRow()
        {
            Random random = new Random();

            int subRowSize = random.Next(3, consoleHeight / 2 - 1);
            char[] SubRow = new char[subRowSize];
            for (int i = 0; i < subRowSize; i++)
            {
                SubRow[i] = Convert.ToChar(random.Next(33, 87));
            }
            return SubRow;
        }
        public static void CreateRow(object x)
        {
            char[] Row = new char[consoleHeight];

            for (int i = 0; i < consoleHeight; i++)
            {
                Row[i] = ' ';
            }

            Action(x, Row);
        }

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetWindowSize(consoleWidth + 2, consoleHeight + 2);
            Console.CursorVisible = false;
            Random random = new Random();

            List<Thread> ParameterizedThreads = new List<Thread>();
            List<int> AlreadyContains = new List<int>();

            int randomValue = 0;

            do
            {
                randomValue = random.Next(0, consoleWidth);
                if (!AlreadyContains.Contains(randomValue))
                {
                    AlreadyContains.Add(randomValue);
                }
                if (AlreadyContains.Count == consoleWidth) break;
            }
            while (true);

            for (int i = 0; i < consoleWidth; i++)
            {
                ParameterizedThreads.Add(new Thread(new ParameterizedThreadStart(CreateRow)));
                ParameterizedThreads[i].Start(AlreadyContains[i]);
                Thread.Sleep(100);
                
            }

        }
    }
}
