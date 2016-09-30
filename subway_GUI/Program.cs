using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;



namespace subway_GUI
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        public class Station
        {
            private string name;
            private int[] line;
            private int sumLine;
            private int id;
            private int[] neight;
            private int sumNeight;
            private int x;
            private int y;

            public Station(string n,int l,int i,int x,int y)
            {
                name = n;
                line = new int[3];
                neight = new int[5];
                line[0] = l;
                sumLine = 1;
                id = i;
                sumNeight = 0;
                this.x = x;
                this.y = y;
            }

            public Station()
            {
                line = new int[3];
                neight = new int[5];
                sumLine = 0;
                sumNeight = 0;
            }
            
            public string getName()
            {
                return name;
            }

            public int getId()
            {
                return id;
            }

            public int getSumLine()
            {
                return sumLine;
            }

            public int getLine(int i)
            {
                return line[i];
            }

            public int getSumNeight()
            {
                return sumNeight;
            }

            public int getNeight(int i)
            {
                return neight[i];
            }

            public int getX()
            {
                return x;
            }

            public int getY()
            {
                return y;
            }

            public void appendNeight(int id)
            {
                neight[sumNeight++] = id;
            }

            public void appendLine(int li)
            {
                line[sumLine++] = li;
            }
        }
        public static Station[] station = new Station[300];
        static int id = -1;
        static int[,] headLine = new int[20, 50];
        public static string[] nameLine = new string[20];
        [STAThread]
        static void Main(string[] args)
        {
            int line = -1;
            int idInLine = 0;
            //static string exePath1 = System.Environment.CurrentDirectory;
            string exePath2 = new DirectoryInfo("../../").FullName;
            string exePath1 = System.Environment.CurrentDirectory;
            //exePath1 + @"\beijing-subway_GUI\"
            //Station[] station = new Station[300];
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(exePath1 + @"\beijing-subway_GUI\station.xml");
            }
            catch(Exception e)
            {
                try
                {
                    xml.Load(exePath2 + @"\beijing-subway_GUI\station.xml");
                }
                catch(Exception e1)
                {
                    AllocConsole();
                    Console.WriteLine("Input File Invalid");
                    Console.ReadKey();
                    //while (true) ;
                    return;
                }
            }
            XmlElement root = xml.DocumentElement;
            XmlNodeList listLine = root.GetElementsByTagName("Line");

            foreach(XmlNode nodeLine in listLine)
            {
                idInLine = 0;
                nameLine[++line] = ((XmlElement)nodeLine).GetAttribute("name");
                XmlNodeList listStation = ((XmlElement)nodeLine).GetElementsByTagName("Station");
                foreach(XmlNode nodeStation in listStation)
                {
                    XmlNode nodeName = nodeStation.SelectSingleNode("Name");
                    XmlNode nodeX = nodeStation.SelectSingleNode("X");
                    XmlNode nodeY = nodeStation.SelectSingleNode("Y");
                    string tempName = nodeName.InnerText;
                    string tempX = nodeX.InnerText;
                    string tempY = nodeY.InnerText;
                    int i;
                    for(i = 0; i <= id; i++)
                    {
                        if (station[i].getName() == tempName)
                            break;
                    }
                    if (i == id+1)
                    {
                        station[++id] = new Station(tempName, line, id, System.Int32.Parse(tempX), System.Int32.Parse(tempY));
                        headLine[line, ++idInLine] = id;
                        if (line == 17 && idInLine >= 3)
                        {
                            station[headLine[line, idInLine - 1]].appendNeight(i);
                        }
                        else if (idInLine > 1)
                        {
                            station[id].appendNeight(headLine[line, idInLine - 1]);
                            station[headLine[line, idInLine - 1]].appendNeight(id);
                        }
                    }
                    else
                    {
                        headLine[line, ++idInLine] = i;
                        if(line == 17 && idInLine >= 3)
                        {
                            station[headLine[line, idInLine - 1]].appendNeight(i);
                        }
                        else if(idInLine >1)
                        {
                            station[i].appendNeight(headLine[line, idInLine - 1]);
                            station[headLine[line, idInLine - 1]].appendNeight(i);
                        }
                        station[i].appendLine(line);
                    }
                    headLine[line,0] = idInLine;
                }
            }
            headLine[1, 0]--;
            headLine[8, 0]--;
            headLine[17, 0]--;
            /*
            System.IO.StreamWriter file = new System.IO.StreamWriter("id.txt", false);
            for(int i = 0; i <= id; i++)
            {
                string s = "ID is " + station[i].getId() + "\r\nName is " + station[i].getName() + "\r\nLine is " + nameLine[station[i].getLine(0)]+ "\r\nNeight is";
                file.Write(s);
                Console.Write("ID is {2} Name is {0} Line is {1} Neight is", station[i].getName(), nameLine[station[i].getLine(0)],station[i].getId());
                for(int j = 0; j < station[i].getSumNeight(); j++)
                {
                    Console.Write("{0} ",station[station[i].getNeight(j)].getName());
                    file.Write(station[station[i].getNeight(j)].getName() + " ");
                }
                Console.WriteLine("\n");
                file.Write("\r\n\r\n");
            }
            file.Close();
            */
            /*
            System.IO.StreamWriter file = new System.IO.StreamWriter("id2.txt", false);
            for (int i = 0; i <= line; i++)
            {
                file.Write("{0} {1}\r\n",i,nameLine[i]);
            }
            file.Close();
            */
            if(args.Length == 3)
            {
                if(args[0] == "-b")
                {
                    AllocConsole();
                    int start_id = -1, end_id = -1;
                    for(int i = 0;i<= id && (start_id == -1 || end_id ==-1); i++)
                    {
                        if (station[i].getName() == args[1])
                            start_id = i;
                        if (station[i].getName() == args[2])
                            end_id = i;
                    }
                    if(start_id == -1 || end_id == -1)
                    {
                        Console.WriteLine("Name Of Station Is Not Exist!");
                        Console.ReadKey();
                        return;
                    }
                    else if(start_id == end_id)
                    {
                        Console.WriteLine("Information Of Station Is Invalid!");
                        Console.ReadKey();
                        return;
                    }
                    int[,] result;
                    Search(0, start_id, end_id,out result);

                    int tempint = 1;
                    Console.WriteLine("{0}", result[0, 0]+1);
                    while (true)
                    {
                        Console.Write("{0}", station[result[tempint, 0]].getName());
                        if (tempint>0 && result[tempint, 1] != result[tempint - 1, 1] && result[tempint,0]!=end_id && result[tempint,0]!=start_id)
                        {
                            Console.Write("换乘{0}", nameLine[result[tempint, 1]]);
                        }
                        Console.Write("\n");
                        if (result[tempint++, 0] == end_id)
                            break;
                    }
                    Console.ReadKey();
                }
                else if(args[0] == "-c")
                {
                    AllocConsole();
                    int start_id = -1, end_id = -1;
                    for (int i = 0; i <= id && (start_id == -1 || end_id == -1); i++)
                    {
                        if (station[i].getName() == args[1])
                            start_id = i;
                        if (station[i].getName() == args[2])
                            end_id = i;
                    }
                    if (start_id == -1 || end_id == -1)
                    {
                        Console.WriteLine("Name Of Station Is Not Exist!");
                        Console.ReadKey();
                        return;
                    }
                    else if (start_id == end_id)
                    {
                        Console.WriteLine("Information Of Station Is Invalid!");
                        Console.ReadKey();
                        return;
                    }
                    int[,] result;
                    Search(1, start_id, end_id, out result);

                    int tempint = 1;
                    Console.WriteLine("{0}", result[0, 0] + 1);
                    while (true)
                    {
                        Console.Write("{0}", station[result[tempint, 0]].getName());
                        if (tempint > 0 && result[tempint, 1] != result[tempint - 1, 1] && result[tempint, 0] != end_id && result[tempint, 0] != start_id)
                        {
                            Console.Write("换乘{0}", nameLine[result[tempint, 1]]);
                        }
                        Console.Write("\n");
                        if (result[tempint++, 0] == end_id)
                            break;
                    }
                    Console.ReadKey();
                }
            }
            else if (args.Length == 0)
            {
                AllocConsole();
                while (true)
                {
                    string nameIn;
                    nameIn = Console.ReadLine();
                    int i;
                    for (i = 0; i <= line; i++)
                    {
                        if (nameLine[i] == nameIn)
                            break;
                    }
                    if (i == line + 1)
                    {
                        Console.WriteLine("Invalid Input");
                    }
                    else
                    {
                        for (int j = 1; j <= headLine[i, 0]; j++)
                        {
                            Console.WriteLine("{0}", station[headLine[i, j]].getName());
                        }
                    }
                }
            }
            else if (args.Length == 1 && args[0] == "-g")
            {
                GUI gui = new GUI();
                gui.showGUI();
            }
            else
            {
                AllocConsole();
                Console.WriteLine("Input Parameter Invalid");
                Console.ReadKey();
                return;
            }
        }

        public static void Search(int modle,int start, int end,out int[,]result)
        {
            result = new int[100, 2];
            int top = -1, last = 0;
            int[] queue = new int[300];
            int[,] mark = new int[300, 4];
            for(int i = 0; i < 300; i++)
            {
                mark[i, 0] = 0;
                mark[i, 1] = -1;
                mark[i, 2] = 999;
                mark[i, 3] = 999;
            }
            //0 mark,1 per,2 distance,3 exchange
            if(modle == 0)
            {
                //-b
                queue[++top] = start;
                mark[start, 0] = 1;
                mark[start, 2] = 0;
                while (top <= last)
                {
                    int temptop = queue[top];
                    int j = -1;
                    top++;
                    for(int i = 0; i < station[temptop].getSumNeight(); i++)
                    {
                        j = station[temptop].getNeight(i);
                        if(mark[j,0] == 0 ||(mark[j,0] == 1 && mark[j,2]>mark[temptop,2]+1))
                        {
                            if (mark[j, 0] == 0)
                                queue[++last] = j;
                            mark[j, 0] = 1;
                            mark[j, 1] = temptop;
                            mark[j, 2] = mark[temptop, 2] + 1;
                        }
                        if (j == end)
                            break;
                    }
                    if (j == end)
                        break;
                }
                result[0, 0] = mark[end, 2];
                for (int i = mark[end, 2]+1; i >= 1; i--)
                {
                    result[i, 0] = end;
                    int j;
                    if (i != 1)
                    {
                        for (j = 0; j < station[end].getSumLine(); j++)
                        {
                            int k;
                            for (k = 0; k < station[mark[end, 1]].getSumLine(); k++)
                            {
                                if (station[end].getLine(j) == station[mark[end, 1]].getLine(k))
                                    break;
                            }
                            if (k != station[mark[end, 1]].getSumLine())
                                break;
                        }
                        result[i - 1, 1] = station[end].getLine(j);
                    }
                    end = mark[end, 1];
                }
            }
            else if (modle == 1)
            {
                //-c
                int sumExchange = 0;
                for (int i = 0; i < id; i++)
                    if (station[i].getSumLine() > 1)
                        sumExchange++;
                mark[start, 0] = 1;
                mark[start, 2] = 0;
                mark[start, 3] = -1;
                queue[++top] = start;
                if (station[start].getSumLine() == 1)
                    sumExchange++;
                while (top < sumExchange)
                {
                    int tempint = queue[top];
                    top++;
                    for (int i = 0; i < station[tempint].getSumLine(); i++)
                    {
                        int templine = station[tempint].getLine(i);
                        int tempid = -1;
                        for (int j = 0; j <= headLine[templine, 0]; j++)
                            if (headLine[templine, j] == tempint)
                                tempid = j;
                        if (templine == 17)
                        {
                            
                            if(tempint == 263)
                            {
                                mark[158, 0] = 1;
                                mark[158, 1] = 264;
                                mark[158, 2] = 2;
                                mark[158, 3] = 0;
                                mark[264, 0] = 1;
                                mark[264, 1] = 263;
                                mark[264, 2] = 1;
                                mark[264, 3] = 0;
                                mark[27, 0] = 1;
                                mark[27, 1] = 158;
                                mark[27, 2] = 3;
                                mark[27, 3] = 0;
                            }
                            else if(tempint == 264)
                            {
                                mark[158, 0] = 1;
                                mark[158, 1] = 264;
                                mark[158, 2] = 1;
                                mark[158, 3] = 0;
                                mark[263, 0] = 1;
                                mark[263, 1] = 158;
                                mark[263, 2] = 2;
                                mark[263, 3] = 0;
                                mark[27, 0] = 1;
                                mark[27, 1] = 158;
                                mark[27, 2] = 2;
                                mark[27, 3] = 0;
                            }
                            else if(tempint == 158)
                            {
                                if (mark[264, 0] == 0)
                                {
                                    mark[264, 0] = 1;
                                    mark[264, 1] = 263;
                                    mark[264, 2] = mark[158, 2] + 2;
                                    mark[264, 3] = mark[158, 3];
                                }
                                else
                                {
                                    if (mark[158, 3] < mark[264, 3] || (mark[158, 3] == mark[264, 3] && mark[264, 2] > mark[158, 2] + 2))
                                    {
                                        mark[264, 1] = 263;
                                        mark[264, 2] = mark[158, 2] + 2;
                                        mark[264, 3] = mark[158, 3];
                                    }
                                }
                                if (mark[263, 0] == 0)
                                {
                                    mark[263, 0] = 1;
                                    mark[263, 1] = 158;
                                    mark[263, 2] = mark[158, 2] + 1;
                                    mark[263, 3] = mark[158, 3];
                                }
                                else
                                {
                                    if (mark[158, 3] < mark[263, 3] || (mark[158, 3] == mark[263, 3] && mark[263, 2] > mark[158, 2] + 1))
                                    {
                                        mark[263, 1] = 158;
                                        mark[263, 2] = mark[158, 2] + 1;
                                        mark[263, 3] = mark[158, 3];
                                    }
                                }
                                if (mark[27, 0] == 0)
                                {
                                    mark[27, 0] = 1;
                                    mark[27, 1] = 158;
                                    mark[27, 2] = mark[158, 2] + 1;
                                    mark[27, 3] = mark[158, 3];
                                }
                                else
                                {
                                    if(mark[158,3] < mark[27,3] ||(mark[158,3]==mark[27,3] && mark[27,2] > mark[158, 2] + 1))
                                    {
                                        mark[27, 1] = 158;
                                        mark[27, 2] = mark[158, 2] + 1;
                                        mark[27, 3] = mark[158, 3];
                                    }
                                }
                            }
                            else if(tempint == 27)
                            {
                                if (mark[264, 0] == 0)
                                {
                                    mark[264, 0] = 1;
                                    mark[264, 1] = 263;
                                    mark[264, 2] = mark[27, 2] + 3;
                                    mark[264, 3] = mark[27, 3];
                                }
                                else
                                {
                                    if (mark[27, 3] < mark[264, 3] || (mark[27, 3] == mark[264, 3] && mark[264, 2] > mark[27, 2] + 3))
                                    {
                                        mark[264, 1] = 263;
                                        mark[264, 2] = mark[27, 2] + 3;
                                        mark[264, 3] = mark[27, 3];
                                    }
                                }
                                if (mark[263, 0] == 0)
                                {
                                    mark[263, 0] = 1;
                                    mark[263, 1] = 158;
                                    mark[263, 2] = mark[27, 2] + 2;
                                    mark[263, 3] = mark[27, 3];
                                }
                                else
                                {
                                    if (mark[27, 3] < mark[263, 3] || (mark[27, 3] == mark[263, 3] && mark[263, 2] > mark[27, 2] + 2))
                                    {
                                        mark[263, 1] = 158;
                                        mark[263, 2] = mark[27, 2] + 2;
                                        mark[263, 3] = mark[27, 3];
                                    }
                                }
                                if (mark[158, 0] == 0)
                                {
                                    mark[158, 0] = 1;
                                    mark[158, 1] = 27;
                                    mark[158, 2] = mark[27, 2] + 1;
                                    mark[158, 3] = mark[27, 3];
                                }
                                else
                                {
                                    if (mark[27, 3] < mark[158, 3] || (mark[158, 3] == mark[27, 3] && mark[158, 2] > mark[27, 2] + 1))
                                    {
                                        mark[158, 1] = 27;
                                        mark[158, 2] = mark[27, 2] + 1;
                                        mark[158, 3] = mark[27, 3];
                                    }
                                }
                            }
                            
                        }
                        else if (templine == 1 || templine == 8)
                        {
                            //circle
                            for (int j = tempid + 1; j <= headLine[templine, 0]; j++)
                            {
                                if (mark[headLine[templine, j], 0] == 0)
                                {
                                    //first find
                                    mark[headLine[templine, j], 0] = 1;
                                    mark[headLine[templine, j], 1] = tempint;
                                    mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2];
                                    mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    if (station[headLine[templine, j]].getSumLine() > 1)
                                    {
                                        queue[++last] = headLine[templine, j];
                                    }
                                }
                                else
                                {
                                    if (mark[headLine[templine, j], 3] > mark[tempint, 3] + 1)
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                    else if (mark[headLine[templine, j], 3] == mark[tempint, 3] + 1 && mark[headLine[templine, j], 2] > j - tempid + mark[tempint, 2])
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                }
                            }
                            for (int j = 1; j <= tempid - 1; j++)
                            {
                                if (mark[headLine[templine, j], 0] == 0)
                                {
                                    //first find
                                    mark[headLine[templine, j], 0] = 1;
                                    mark[headLine[templine, j], 1] = tempint;
                                    mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2] + headLine[templine, 0];
                                    mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    if (station[headLine[templine, j]].getSumLine() > 1)
                                    {
                                        queue[++last] = headLine[templine, j];
                                    }
                                }
                                else
                                {
                                    if (mark[headLine[templine, j], 3] > mark[tempint, 3] + 1)
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2] + headLine[templine, 0];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                    else if (mark[headLine[templine, j], 3] == mark[tempint, 3] + 1 && mark[headLine[templine, j], 2] > j - tempid + mark[tempint, 2] + headLine[templine, 0])
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2] + headLine[templine, 0];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                }
                            }
                            for (int j = tempid - 1; j >= 1; j--)
                            {
                                if (mark[headLine[templine, j], 3] == mark[tempint, 3] + 1 && mark[headLine[templine, j], 2] > tempid - j + mark[tempint, 2])
                                {
                                    mark[headLine[templine, j], 1] = tempint;
                                    mark[headLine[templine, j], 2] = tempid - j + mark[tempint, 2];
                                    mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                }
                            }
                            for (int j = headLine[templine, 0]; j > tempid; j--)
                            {
                                if (mark[headLine[templine, j], 3] == mark[tempint, 3] + 1 && mark[headLine[templine, j], 2] > tempid + headLine[templine, 0] - j + mark[tempint, 2])
                                {
                                    mark[headLine[templine, j], 1] = tempint;
                                    mark[headLine[templine, j], 2] = tempid - j + mark[tempint, 2] + headLine[templine, 0];
                                    mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                }
                            }
                        }
                        else
                        {
                            //!circle
                            for (int j = tempid + 1; j <= headLine[templine, 0]; j++)
                            {
                                if (mark[headLine[templine, j], 0] == 0)
                                {
                                    //first find
                                    mark[headLine[templine, j], 0] = 1;
                                    mark[headLine[templine, j], 1] = tempint;
                                    mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2];
                                    mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    if (station[headLine[templine, j]].getSumLine() > 1)
                                    {
                                        queue[++last] = headLine[templine, j];
                                    }
                                }
                                else
                                {
                                    if (mark[headLine[templine, j], 3] > mark[tempint, 3] + 1)
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                    else if (mark[headLine[templine, j], 3] == mark[tempint, 3] + 1 && mark[headLine[templine, j], 2] > j - tempid + mark[tempint, 2])
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = j - tempid + mark[tempint, 2];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                }
                            }
                            for (int j = tempid - 1; j >= 1; j--)
                            {
                                if (mark[headLine[templine, j], 0] == 0)
                                {
                                    //first find
                                    mark[headLine[templine, j], 0] = 1;
                                    mark[headLine[templine, j], 1] = tempint;
                                    mark[headLine[templine, j], 2] = tempid - j + mark[tempint, 2];
                                    mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    if (station[headLine[templine, j]].getSumLine() > 1)
                                    {
                                        queue[++last] = headLine[templine, j];
                                    }
                                }
                                else
                                {
                                    if (mark[headLine[templine, j], 3] > (mark[tempint, 3] + 1))
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = tempid - j + mark[tempint, 2];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                    else if (mark[headLine[templine, j], 3] == (mark[tempint, 3] + 1) && mark[headLine[templine, j], 2] > tempid - j + mark[tempint, 2])
                                    {
                                        mark[headLine[templine, j], 1] = tempint;
                                        mark[headLine[templine, j], 2] = tempid - j + mark[tempint, 2];
                                        mark[headLine[templine, j], 3] = mark[tempint, 3] + 1;
                                    }
                                }
                            }
                        }
                    }
                }
                result[0, 0] = mark[end, 2];
                for (int i = mark[end, 2]+1; i >= 1;)
                {
                    result[i, 0] = end;
                    if (i == 1)
                        break;
                    int per = mark[end, 1];
                    int now_line = -1;
                    for(int j = 0; j < station[end].getSumLine(); j++)
                    {
                        int k;
                        for(k = 0; k < station[per].getSumLine(); k++)
                        {
                            if (station[end].getLine(j) == station[per].getLine(k))
                            {
                                now_line = station[end].getLine(j);
                                break;
                            }
                        }
                        if (k != station[per].getSumLine())
                            break;
                    }
                    int s = -1, e = -1;
                    for(int j = headLine[now_line, 0]; j >= 1; j--)
                    {
                        if (headLine[now_line, j] == end)
                            s = j;
                        if (headLine[now_line, j] == per)
                            e = j;
                    }
                    if(now_line == 17)
                    {
                        result[i, 0] = end;
                        result[--i, 1] = 17;
                    }
                    else if(now_line == 1 || now_line == 8)
                    {
                        int m = 1;
                        if (s > e && s - e < e - s + headLine[now_line, 0])
                            m = -1;
                        else if (s < e && e - s > s - e + headLine[now_line, 0])
                        {
                            m = -1;
                        }
                        else if (s < e && e - s < s - e + headLine[now_line, 0])
                            m = 1;
                        else if (s > e && s - e > e - s + headLine[now_line, 0])
                        {
                            m = 1;
                        }
                        while (true)
                        {
                            result[i, 0] = headLine[now_line, s];
                            result[--i, 1] = now_line;
                            if (m > 0)
                            {
                                s++;
                                if (s == headLine[now_line, 0]+1)
                                    s = 1;
                            }
                            else
                            {
                                s--;
                                if (s == 0)
                                    s = headLine[now_line, 0];
                            }
                            if (s == e)
                                break;    
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            result[i, 0] = headLine[now_line,s];
                            result[--i, 1] = now_line;
                            if (s > e)
                                s--;
                            else
                                s++;
                            if(s == e)
                            {
                                break;
                            }
                        }
                    }
                    end = mark[end, 1];
                }
            }

        }

    }
}
