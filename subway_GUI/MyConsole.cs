using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subway_GUI
{
    public struct SearchLine
    {
        public int stationId;
        public int lineId;
    }
    public struct Station
    {
        public Point pos;
    };
    public struct Line
    {
        public int R;
        public int G;
        public int B;

    }
    class MyConsole
    {
        static string exePath1 = System.Environment.CurrentDirectory;//本程序所在路径
        static string exePath2 = new DirectoryInfo("../../").FullName;//当前应用程序路径的上上级目录

        private Bitmap bit;
        private Bitmap bitHide;
        private Bitmap bitSSYH;
        private Bitmap bitSSBT;
        private Bitmap bitS3;
        private Bitmap bit32;
        private Bitmap bit2S;
        private Bitmap bitPS;
        private Bitmap bitPE;

        private Bitmap bitScreenBg;
        private Bitmap bitScreenGray;
        private Bitmap bitScreenRed;


        private double scale;
        private double scaleC;

        private int stationTop = 0, lineTop = 0;
        private Station[] stationList = new Station[300];
        private Line[] lineList = new Line[30];

        private int searchAccuracy = 40;

        private static MyConsole instance;

        private MyConsole()
        {

        }

        public static MyConsole GetInstance()
        {
            if (instance == null)
            {
                instance = new MyConsole();
            }
            return instance;
        }

        public bool init()
        {
            bool r = loadImage();
            if (!r)
            {
                return r;
            }

            scale = bit.Width / 8866.0;
            scaleC = 8866.0 / bit.Width;

            searchAccuracy = (int)(searchAccuracy * scale);

            return readFile();
        }
        public double getScale()
        {
            return scale;
        }
        public double getScaleC()
        {
            return scaleC;
        }
        public Point getId(int id)
        {
            return stationList[id].pos;
        }
        public Line getLine(int id)
        {
            return lineList[id];
        }
        public Point getPos(int id)
        {
            return stationList[id].pos;
        }
        public Bitmap getBit(int i = 0)
        {
            if (i == 1)
            {
                return bitHide;
            }
            else if (i == 2)
            {
                return bitSSYH;
            }
            else if (i == 3)
            {
                return bitSSBT;
            }
            else if (i == 4)
            {
                return bitS3;
            }
            else if (i == 5)
            {
                return bit32;
            }
            else if (i == 6)
            {
                return bit2S;
            }
            else if (i == 7)
            {
                return bitPS;
            }
            else if (i == 8)
            {
                return bitPE;
            }
            else if (i == 9)
            {
                return bitScreenBg;
            }
            else if (i == 10)
            {
                return bitScreenGray;
            }
            else if (i == 11)
            {
                return bitScreenRed;
            }
            return bit;
        }
        private bool tryLoadImage(string s,out Bitmap des)
        {
            Bitmap src;
            try
            {
                src = new Bitmap(exePath1 + @"\beijing-subway_GUI\" + s);
            }
            catch (Exception)
            {
                try
                {
                    src = new Bitmap(exePath2 + @"\beijing-subway_GUI\" + s);
                }
                catch (Exception)
                {
                    des = new Bitmap(1,1);
                    return false;
                }
            }
            des = src;
            return true;
        }
        private bool loadImage()
        {
            return tryLoadImage("subway_map.bmp", out bit)
                && tryLoadImage("subway_map_hide.bmp", out bitHide)
                && tryLoadImage("subway_map_hide_sihui_sihuidong_yihao.jpg", out bitSSYH)
                && tryLoadImage("subway_map_hide_sihui_sihuidong_batong.jpg", out bitSSBT)
                && tryLoadImage("subway_map_hide_sanyuanqiao_3hao.jpg", out bitS3)
                && tryLoadImage("subway_map_hide_3hao_2hao.jpg", out bit32)
                && tryLoadImage("subway_map_hide_2hao_sanyuanqiao.jpg", out bit2S)
                && tryLoadImage("subway_map_pointstart.bmp", out bitPS)
                && tryLoadImage("subway_map_pointend.bmp", out bitPE)
                && tryLoadImage("screen_background.jpg", out bitScreenBg)
                && tryLoadImage("screen_gray.bmp", out bitScreenGray)
                && tryLoadImage("screen_red.bmp", out bitScreenRed);
        }

        public int accuratePosition(Point t, out Point t2)
        {
            int ans = 0;
            Station temp = new Station();
            for (int i = 0; i < stationTop; i++)
            {
                if (stationList[i].pos.X - t.X <= searchAccuracy
                    && stationList[i].pos.X - t.X >= -searchAccuracy
                    && stationList[i].pos.Y - t.Y <= searchAccuracy
                    && stationList[i].pos.Y - t.Y >= -searchAccuracy
                    )
                {
                    ans++;
                    temp = stationList[i];
                }
            }
            if (ans == 0)
            {
                t2 = t;
            }
            else
            {
                t2 = new Point(temp.pos.X, temp.pos.Y);
            }

            return ans;
        }
        public int searchStation(string s)
        {
            for (int i = 0; i < stationTop; i++)
            {
                string temp = Program.station[i].getName();
                if (temp == s)
                {
                    return i;
                }
            }
            return -1;
        }
        public int searchStation(Point t)
        {
            for (int i = 0; i < stationTop; i++)
            {
                if (stationList[i].pos.X == t.X
                    && stationList[i].pos.Y == t.Y)
                {
                    return i;
                }
            }
            return -1;
        }
        private bool tryLoadFile(string s, out string des)
        {
            string temp;
            try
            {
                temp = System.IO.File.ReadAllText(exePath1 + @"\beijing-subway_GUI\" + s);
            }
            catch (Exception)
            {
                try
                {
                    temp = System.IO.File.ReadAllText(exePath2 + @"\beijing-subway_GUI\" + s);
                }
                catch (Exception)
                {
                    des = "";
                    return false;
                }
            }
            des = temp;
            return true;
        }
        private bool readFile()
        {
            string line;
            string station;
            return tryLoadFile("beijing-subway_GUI_line.txt", out line) 
                && tryLoadFile("beijing-subway_GUI_station.txt", out station)
                && analysisFile(line, 3) && analysisFile(station, 2);
        }
        private bool analysisFile(string s, int no)
        {
            int mark = 0;
            int a = 0, b = 0, c = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ' || s[i] == '\r' || s[i] == '\n')
                {
                    continue;
                }
                if (mark == 0 && s[i] == '(')
                {
                    mark = 1;
                }
                else if (mark == 1 && s[i] <= '9' && s[i] >= '0')
                {
                    a = 10 * a + (s[i] - '0');
                }
                else if (mark == 1 && s[i] == ',')
                {
                    mark = 2;
                }
                else if (mark == 2 && s[i] <= '9' && s[i] >= '0')
                {
                    b = 10 * b + (s[i] - '0');
                }
                else if (mark == 2 && s[i] == ')' && no == 2)
                {
                    mark = 0;
                    stationList[stationTop].pos.X = (int)(a*scale);
                    stationList[stationTop].pos.Y = (int)(b*scale);
                    stationTop++;
                    a = 0;
                    b = 0;
                }
                else if (mark == 2 && s[i] == ',' && no == 3)
                {
                    mark = 3;
                }
                else if (mark == 3 && s[i] <= '9' && s[i] >= '0')
                {
                    c = 10 * c + (s[i] - '0');
                }
                else if (mark == 3 && s[i] == ')' && no == 3)
                {
                    mark = 0;
                    lineList[lineTop].R = a;
                    lineList[lineTop].G = b;
                    lineList[lineTop].B = c;
                    lineTop++;
                    a = 0;
                    b = 0;
                    c = 0;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
