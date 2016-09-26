using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subway_GUI
{
    class Screen
    {
        private MyConsole con = MyConsole.GetInstance();
        private Bitmap bg;
        private Bitmap gray;
        private Bitmap red;

        private int ten;
        private int single;

        private Point tenLT;
        private Point singleLT;
        private int length;

        private int offset = 3;

        public void init()
        {
            ten = 0;
            single = 0;

            bg = con.getBit(9);
            gray = con.getBit(10);
            red = con.getBit(11);

            tenLT = new Point(0, 0);
            singleLT = new Point(bg.Width / 2 - 20, 0);
            length = gray.Width;

            tenLT = reset(tenLT);
            singleLT = reset(singleLT);
            

            show();
        }
        private Point reset(Point p)
        {
            p.X += bg.Width / 4 - length / 2;
            p.Y += bg.Height / 2 - length;
            return p;
        }
        private void show()
        {
            show(ten, tenLT);
            show(single, singleLT);
        }
        private void show(int no, Point p)
        {
            /*
             *         -----   1
             *        |     |
             *    2   |     |  3
             *        |     |
             *         -----   4
             *        |     |
             *    5   |     |  6
             *        |     |
             *         -----   7
            */
            Point temp = new Point(p.X,p.Y);
            //1
            if (no == 0 || no == 2 || no == 3 || no == 5 || no == 6
                || no == 7 || no == 8 || no == 9)
            {
                temp.X = p.X;
                temp.Y = p.Y;
                drowPicture(bg, temp, red, 0);
            }
            else
            {
                temp.X = p.X;
                temp.Y = p.Y;
                drowPicture(bg, temp, gray, 0);
            }
            //2
            if(no == 0||no == 4 || no == 5 || no == 6 || no == 8 || no == 9)
            {
                temp.X = p.X;
                temp.Y = p.Y;
                drowPicture(bg, temp, red, 1);
            }
            else
            {
                temp.X = p.X;
                temp.Y = p.Y;
                drowPicture(bg, temp, gray, 1);
            }
            //3
            if (no == 0 ||no == 1 || no == 2 || no == 3 || no == 4 || no == 7
                ||no == 8 || no == 9)
            {
                temp.X = p.X + length;
                temp.Y = p.Y;
                drowPicture(bg, temp, red, 1);
            }
            else
            {
                temp.X = p.X + length;
                temp.Y = p.Y;
                drowPicture(bg, temp, gray, 1);
            }
            //4
            if (no == 2 || no == 3 || no == 4 || no == 5 || no == 6
                || no == 8 || no == 9)
            {
                temp.X = p.X;
                temp.Y = p.Y + length;
                drowPicture(bg, temp, red, 0);
            }
            else
            {
                temp.X = p.X;
                temp.Y = p.Y + length;
                drowPicture(bg, temp, gray, 0);
            }
            //5
            if (no == 0 ||no == 2 || no == 6 || no == 8)
            {
                temp.X = p.X;
                temp.Y = p.Y + length;
                drowPicture(bg, temp, red, 1);
            }
            else
            {
                temp.X = p.X;
                temp.Y = p.Y + length;
                drowPicture(bg, temp, gray, 1);
            }
            //6
            if (no == 0||no == 1 || no == 3 ||no == 4|| no == 5 || no == 6 || no == 7
                || no == 8 || no == 9)
            {
                temp.X = p.X + length;
                temp.Y = p.Y + length;
                drowPicture(bg, temp, red, 1);
            }
            else
            {
                temp.X = p.X + length;
                temp.Y = p.Y + length;
                drowPicture(bg, temp, gray, 1);
            }
            //7
            if (no == 0 ||no == 2 || no == 3 || no == 5 || no == 6 || no == 8
                || no == 9)
            {
                temp.X = p.X;
                temp.Y = p.Y + 2*length;
                drowPicture(bg, temp, red, 0);
            }
            else
            {
                temp.X = p.X;
                temp.Y = p.Y + 2*length;
                drowPicture(bg, temp, gray, 0);
            }
        }
        private void drowPicture(Bitmap bg,Point p,Bitmap c,int mode)
        {
            for(int i = 0;i < c.Width;i++)
            {
                for(int j = 0;j < c.Height;j++)
                {
                    Color color = c.GetPixel(i, j);
                    if (color.R < 10 && color.B < 10 && color.G < 10)
                    {

                    }
                    else
                    {
                        if(mode == 0)
                        {
                            bg.SetPixel(i + p.X+ offset, j + p.Y, color);
                        }
                        else// if(mode == 1)
                        {
                            bg.SetPixel(j + p.X, i + p.Y+ offset, color);
                        }
                        
                    }
                }
            }
        }
        private void calculator()
        {
            single++;
            if(single == 10)
            {
                single = 0;
                ten++;
                if(ten == 10)
                {
                    ten = 0;
                    //超出计算范围
                }
            }
        }
        public void clear()
        {
            single = 0;
            ten = 0;
            show();
        }
        public void add()
        {
            calculator();
            show();
        }
    }
}
