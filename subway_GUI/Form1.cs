using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace subway_GUI
{ 

    public partial class Form1 : Form
    {
        private HandleImage hi = new HandleImage();
        private MyConsole con = MyConsole.GetInstance();
        private Screen sc = new Screen();

        private Point mouseDownPoint;
        private bool isSelected = false;

        private double overrideNum;

        private int oldTop;
        private int oldLeft;

        private Point startPoint = new Point(0, 0);
        private Point endPoint = new Point(0, 0);

        private SearchLine[] ans = new SearchLine[100];
        private int SearchLineTop = 0;

        private string startStation = null;
        private string endStation = null;

        private string errorLanguage = "站点信息有误！！";

        private int startStationId = -1;
        private int endStationId = -1;
        private int searchMode = 0;

        private int heightMax = 1300;

        private int shiningTime = 3;

        private object lockButton = new object();

        private bool threadCartoon = false;

        private Color red = Color.FromArgb(207, 37, 37);
        private Color green = Color.FromArgb(119, 246, 16);
        private Color yellow = Color.FromArgb(255,252,0);

        private int changeSize = 100;

        private Bitmap HLsrc,HLsrc2;
        private int HLmode;
        private Point HLpoint;
        private int HLhandleNo;
        private Color HLcolor;
        private int HLid1, HLid2, HLid3;
        private bool HLmark = true;

        private bool lockException = false;

        private bool quickWatch = false;

        private Thread t;

        public Form1()
        {
            InitializeComponent();
            
            bool b = con.init();
            if (!b)
            {
                textBox3.Text = "程序运行必要信息读取错误，请检查文件后重试！！";
                lockException = true;
            }
            else
            {
                hi.init();
                sc.init();
            }
        }
        private void show(Bitmap WSB)
        {
            pictureBox1.Image = WSB;
        }
        private void showScreen()
        {
            pictureBox2.Image = con.getBit(9);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if(lockException)
            {
                return;
            }

            Bitmap bit = con.getBit();
            show(bit);

            showScreen();

            overrideNum = (double)bit.Width / bit.Height;
        }
        private void clear()
        {
            quickWatch = false;

            startPoint.X = 0;
            startPoint.Y = 0;
            endPoint.X = 0;
            endPoint.Y = 0;

            startStationId = -1;
            endStationId = -1;

            hi.handleImage(1, con.getBit(1),1);
            hi.handleImage(1, con.getBit(), 0);

            sc.clear();
            showScreen();

            show(con.getBit());

        }
        
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (lockException)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isSelected = true;

                oldTop = pictureBox1.Top;
                oldLeft = pictureBox1.Left;
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected && IsMouseInPanel())
            {
                //pictureBox1.Left = pictureBox1.Left + (Cursor.Position.X - mouseDownPoint.X);
                //pictureBox1.Top = pictureBox1.Top + (Cursor.Position.Y - mouseDownPoint.Y);
                
                picture_MoveNotOut(pictureBox1);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                
            }
        }
        private bool IsMouseInPanel()
        {
            if (this.panel2.Left < PointToClient(Cursor.Position).X
            && PointToClient(Cursor.Position).X < this.panel2.Left + this.panel2.Width
            && this.panel2.Top < PointToClient(Cursor.Position).Y
            && PointToClient(Cursor.Position).Y < this.panel2.Top + this.panel2.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (lockException)
            {
                return;
            }
            int want2Height = this.pictureBox1.Height + changeSize;
            want2Height = want2Height > heightMax ? heightMax : want2Height;
            int want2Width = (int)(overrideNum * want2Height);
            picture_WheelNotOut(pictureBox1, want2Width, want2Height);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lockException)
            {
                return;
            }
            int want2Height = this.pictureBox1.Height - changeSize;
            want2Height = want2Height > heightMax ? heightMax : want2Height;
            int want2Width = (int)(overrideNum * want2Height);
            picture_WheelNotOut(pictureBox1, want2Width, want2Height);
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (lockException)
            {
                return;
            }
            //Point temp = new Point(0,0);
            //temp.Y = this.pictureBox1.Height + e.Delta;// * SystemInformation.MouseWheelScrollLines / 20;
            //temp.X  = (int)(overrideNum * temp.Y);
            //picMoveNotOut(pictureBox1, temp);
            int want2Height = this.pictureBox1.Height + e.Delta;
            want2Height = want2Height > heightMax ? heightMax : want2Height;
            int want2Width = (int)(overrideNum * want2Height);
            picture_WheelNotOut(pictureBox1,want2Width, want2Height);
        }
        private void picture_WheelNotOut(PictureBox picBox, int want2Width, int want2Height)
        {

            int right, bottom,left,top;


            if (want2Width < picBox.Parent.Width) want2Width = picBox.Parent.Width;
            if (want2Height < picBox.Parent.Height) want2Height = picBox.Parent.Height;

            int differentX = picBox.Right - picBox.Left - want2Width;
            int differentY = picBox.Bottom - picBox.Top - want2Height;

            right = picBox.Parent.Width - (picBox.Right - differentX);
            bottom = picBox.Parent.Height - (picBox.Bottom - differentY);

            if (right > 0) left = picBox.Parent.Width - want2Width;
            else left = picBox.Left;
            if (bottom > 0) top = picBox.Parent.Height - want2Height;
            else top = picBox.Top;

            
            picBox.Left = left;
            picBox.Top = top;
            picBox.Height = want2Height;
            picBox.Width = want2Width;

        }
        private void picture_MoveNotOut(PictureBox picBox)
        {
            int right, bottom;

            int differentX = Cursor.Position.X - mouseDownPoint.X;
            int differentY = Cursor.Position.Y - mouseDownPoint.Y;

            int want2Lef = picBox.Left + differentX;
            int want2Top = picBox.Top + differentY;

            right = picBox.Parent.Width - (picBox.Right + differentX);
            bottom = picBox.Parent.Height - (picBox.Bottom + differentY);

            if (want2Lef > 0) want2Lef = picBox.Left;
            if (want2Top > 0) want2Top = picBox.Top;
            if (right > 0 && differentX < 0) want2Lef = picBox.Left;
            if (bottom > 0 && differentY < 0) want2Top = picBox.Top;

            picBox.Left = want2Lef;
            picBox.Top = want2Top;

        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (lockException)
            {
                return;
            }
            isSelected = false;

            if (threadCartoon == false && oldTop == pictureBox1.Top && oldLeft == pictureBox1.Left)
            {
                Point temp = similarPosition(e);
                //textBox3.Text = "X:" + temp.X.ToString() + "\r\n";
                //textBox3.Text += "Y:" + temp.Y.ToString() + "\r\n";
                int r = con.accuratePosition(temp, out temp);
                if (r != 1)
                {
                    clear();
                }
                else
                {
                    //textBox3.Text += "X:" + temp.X.ToString() + "\r\n";
                    //textBox3.Text += "Y:" + temp.Y.ToString() + "\r\n";
                    if (startPoint.X == 0 && startPoint.Y == 0)
                    {
                        hi.handleImage(7, con.getBit(), temp,con.getBit(7),0);
                        hi.handleImage(5, con.getBit(), temp,con.getBit(7),0);
                        show(con.getBit());

                        startPoint = temp;
                        startStationId = con.searchStation(temp);

                        //调用查询站名函数
                        startStation = Program.station[startStationId].getName();
                        textBox1.Text = startStation;


                    }
                    else if (endPoint.X == 0 && endPoint.Y == 0)
                    {
                        //hi.handleImage(2, con.getBit(),temp, red, 1);
                        //show(con.getBit());
                        ///*
                        endStationId = con.searchStation(temp);

                        if (endStationId != startStationId)
                        {
                            hi.handleImage(7, con.getBit(), temp, con.getBit(8), 0);
                            hi.handleImage(5, con.getBit(), temp, con.getBit(8), 0);
                            show(con.getBit());

                            endPoint = temp;

                            //调用查询站名函数
                            endStation = Program.station[endStationId].getName();
                            textBox2.Text = endStation;
                            //调用搜索函数
                            int[,] result = new int[100, 2];
                            Program.Search(searchMode, startStationId, endStationId, out result);
                            exchange(result);
                            Print();

                            //启用线程
                            threadStart(); 
                        }
                        else
                        {
                            clear();
                        }
                        //*/

                    }
                    else
                    {
                        clear();
                    }
                }
            }
        }
        private void threadStart()
        {
            threadCartoon = true;
            t = new Thread(new ThreadStart(cartoon));
            t.Start();
            t.IsBackground = true;
            button4.Visible = true;
            button5.Visible = true;
        }
        private void exchange(int[,] result)
        {
            
            SearchLineTop = result[0, 0]+1;
            for(int i = 0;i < SearchLineTop;i++)
            {
                ans[i].stationId = result[i + 1, 0];
                ans[i].lineId = result[i + 1, 1];
            }
            ans[SearchLineTop - 1].lineId = -1;

            
        }
        private void Print()
        {
            textBox3.Text = "";
            int lineId = -1;
            for (int i = 0; i < SearchLineTop; i++)
            {
                textBox3.Text += Program.station[ans[i].stationId].getName();
                if (lineId == -1)
                {
                    lineId = ans[i].lineId;
                }
                else if (ans[i].lineId != -1 && lineId != ans[i].lineId)
                {
                    lineId = ans[i].lineId;
                    textBox3.Text += "换乘" + Program.nameLine[lineId];
                }
                textBox3.Text += "\r\n";
            }
        }
        
        private Point similarPosition(MouseEventArgs e)
        {
            int originalWidth = this.pictureBox1.Image.Width;
            int originalHeight = this.pictureBox1.Image.Height;

            System.Reflection.PropertyInfo rectangleProperty = this.pictureBox1.GetType().GetProperty("ImageRectangle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Rectangle rectangle = (Rectangle)rectangleProperty.GetValue(this.pictureBox1, null);

            int currentWidth = rectangle.Width;
            int currentHeight = rectangle.Height;

            double rate = (double)currentHeight / (double)originalHeight;

            int black_left_width = (currentWidth == this.pictureBox1.Width) ? 0 : (this.pictureBox1.Width - currentWidth) / 2;
            int black_top_height = (currentHeight == this.pictureBox1.Height) ? 0 : (this.pictureBox1.Height - currentHeight) / 2;

            int zoom_x = e.X - black_left_width;
            int zoom_y = e.Y - black_top_height;

            Point original = new Point(0,0);
            original.X = (int)(zoom_x / rate);
            original.Y = (int)(zoom_y / rate);

            return original;
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (lockException)
            {
                return;
            }
            lock (lockButton)
            {
                if (threadCartoon == false)
                {
                    startStation = textBox1.Text;
                    endStation = textBox2.Text;
                    startStationId = con.searchStation(startStation);
                    endStationId = con.searchStation(endStation);
                    

                    if (startStationId != -1 && endStationId != -1)
                    {
                        startPoint = con.getPos(startStationId);
                        endPoint = con.getPos(endStationId);

                        hi.handleImage(7, con.getBit(), startPoint, con.getBit(7), 0);
                        hi.handleImage(7, con.getBit(), endPoint, con.getBit(8), 0);
                        if (startStationId != endStationId)
                        {
                            //调用搜索函数
                            int[,] result = new int[100, 2];
                            Program.Search(searchMode, startStationId, endStationId, out result);
                            exchange(result);
                            Print();
                        }
                        else
                        {
                            textBox3.Text = errorLanguage;
                            clear();
                        }

                    }
                    else
                    {
                        clear();
                    }
                    if (SearchLineTop == 0)
                    {
                        textBox3.Text = errorLanguage;
                    }
                    else
                    {
                        //启用线程
                        threadStart();
                    }
                }
            }
        }
        
        private void cartoon()
        {
            Bitmap src = con.getBit(1);

            while (!HLmark) ;

            HLhandleNo = 7;
            HLsrc = src;
            HLpoint = startPoint;
            HLmode = 0;
            HLmark = false;
            HLsrc2 = con.getBit(7);
            BeginInvoke(new Action(HLloadImage));

            while (!HLmark) ;

            HLhandleNo = 5;
            HLsrc = src;
            HLpoint = startPoint;
            HLmode = 1;
            HLsrc2 = con.getBit(7);
            HLmark = false;
            BeginInvoke(new Action(HLloadImage));

            while (!HLmark) ;

            HLhandleNo = 7;
            HLsrc = src;
            HLpoint = endPoint;
            HLmode = 0;
            HLmark = false;
            HLsrc2 = con.getBit(8);
            BeginInvoke(new Action(HLloadImage));

            while (!HLmark) ;

            HLhandleNo = 5;
            HLsrc = src;
            HLpoint = endPoint;
            HLmode = 1;
            HLsrc2 = con.getBit(8);
            HLmark = false;
            BeginInvoke(new Action(HLloadImage));

            while (!HLmark) ;

            HLhandleNo = 0;
            HLsrc = src;
            HLmark = false;
            BeginInvoke(new Action(HLshow));

            while (!HLmark) ;
            
            for (int i = 0;i < SearchLineTop;i++)
            {

                while (!HLmark) ;

                HLmark = false;
                BeginInvoke(new Action(HLscreen));

                while (!HLmark) ;

                pointCartoon(ans[i].stationId,src);
                if(ans[i].lineId != -1)
                {
                    while (!HLmark) ;
                    HLhandleNo = 4;
                    HLsrc = src;
                    HLid1 = ans[i].stationId;
                    HLid2 = ans[i + 1].stationId;
                    HLid3 = ans[i].lineId;
                    HLcolor = green;
                    HLmark = false;
                    BeginInvoke(new Action(HLdrowLine));

                    while (!HLmark) ;

                    HLhandleNo = 0;
                    HLsrc = src;
                    HLmark = false;
                    BeginInvoke(new Action(HLshow));

                    while (!HLmark) ;
                }
                
                while (!HLmark) ;

                HLhandleNo = 5;
                HLsrc = src;
                HLpoint = startPoint;
                HLmode = 1;
                HLsrc2 = con.getBit(7);
                HLmark = false;
                BeginInvoke(new Action(HLloadImage));

                while (!HLmark) ;

                HLhandleNo = 5;
                HLsrc = src;
                HLpoint = endPoint;
                HLmode = 1;
                HLsrc2 = con.getBit(8);
                HLmark = false;
                BeginInvoke(new Action(HLloadImage));

                while (!HLmark) ;

                HLhandleNo = 0;
                HLsrc = src;
                HLmark = false;
                BeginInvoke(new Action(HLshow));

                while (!HLmark) ;

                HLhandleNo = 0;
                HLsrc = src;
                HLmark = false;
                BeginInvoke(new Action(HLshow));

                while (!HLmark) ;
            }
            while (!HLmark) ;

            Thread.Sleep(6000);

            HLmark = false;
            BeginInvoke(new Action(HLclear));

            while (!HLmark) ;

            threadCartoon = false;
        }
        
        private void pointCartoon(int id,Bitmap src)
        {
            Point temp = con.getId(id);

            while (!HLmark) ;

            HLhandleNo = 6;
            HLsrc = src;
            HLpoint = temp;
            HLmode = 1;
            HLmark = false;
            BeginInvoke(new Action(HLrecord));

            while (!HLmark) ;
            if (!quickWatch)
            {
                HLhandleNo = 2;
                HLsrc = src;
                HLpoint = temp;
                HLcolor = yellow;
                HLmode = 1;
                HLmark = false;
                BeginInvoke(new Action(HLdrowPoint));

                while (!HLmark) ;

                HLhandleNo = 0;
                HLsrc = src;
                HLmark = false;
                BeginInvoke(new Action(HLshow));

                while (!HLmark) ;

                Thread.Sleep(500);

                for (int i = 0; i < shiningTime && !quickWatch; i++)
                {
                    Thread.Sleep(500);

                    while (!HLmark) ;

                    HLhandleNo = 6;
                    HLsrc = src;
                    HLpoint = temp;
                    HLmode = 1;
                    HLmark = false;
                    BeginInvoke(new Action(HLrecord));

                    while (!HLmark) ;

                    HLhandleNo = 2;
                    HLsrc = src;
                    HLpoint = temp;
                    HLcolor = red;
                    HLmode = 1;
                    HLmark = false;
                    BeginInvoke(new Action(HLdrowPoint));

                    while (!HLmark) ;

                    HLhandleNo = 0;
                    HLsrc = src;
                    HLmark = false;
                    BeginInvoke(new Action(HLshow));

                    while (!HLmark) ;

                    Thread.Sleep(500);

                    while (!HLmark) ;

                    HLhandleNo = 3;
                    HLsrc = src;
                    HLmark = false;
                    BeginInvoke(new Action(HLbackUp));

                    while (!HLmark) ;

                    HLhandleNo = 0;
                    HLsrc = src;
                    HLmark = false;
                    BeginInvoke(new Action(HLshow));

                    while (!HLmark) ;
                }
            }

            Thread.Sleep(500);

            while (!HLmark) ;

            HLhandleNo = 2;
            HLsrc = src;
            HLpoint = temp;
            HLcolor = green;
            HLmode = 1;
            HLmark = false;
            BeginInvoke(new Action(HLdrowPoint));

            while (!HLmark) ;

            HLhandleNo = 0;
            HLsrc = src;
            HLmark = false;
            BeginInvoke(new Action(HLshow));

            while (!HLmark) ;
        }
        private void HLbackUp()
        {
            hi.handleImage(HLhandleNo, HLsrc);
            HLmark = true;
        }
        private void HLclear()
        {
            button4.Visible = false;
            button5.Visible = false;
            clear();
            HLmark = true;
        }
        private void HLscreen()
        {
            sc.add();
            showScreen();
            HLmark = true;
        }
        private void HLshow()
        {
            show(HLsrc);
            HLmark = true;
        }
        private void HLloadImage()
        {
            hi.handleImage(HLhandleNo, HLsrc, HLpoint, HLsrc2, HLmode);
            HLmark = true;
        }
        private void HLrecord()
        {
            hi.handleImage(HLhandleNo, HLsrc, HLpoint, HLmode);
            HLmark = true;
        }
        private void HLdrowPoint()
        {
            hi.handleImage(HLhandleNo, HLsrc, HLpoint, HLcolor, HLmode);
            HLmark = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            t.Abort();
            threadCartoon = false;
            button4.Visible = false;
            button5.Visible = false;
            clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            quickWatch = true;
        }

        private void HLdrowLine()
        {
            hi.handleImage(HLhandleNo, HLsrc, HLid1, HLid2, HLid3,HLcolor);
            HLmark = true;
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            searchMode = 1;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            searchMode = 0;
        }

        
    }
}
