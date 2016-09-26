using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subway_GUI
{

    public struct BackUp
    {
        public Point LT;
        public int width;
        public int height;
        public Bitmap fragment;
    }
    class HandleImage
    {
        private MyConsole con = MyConsole.GetInstance();

        private int radius = 40;
        private int radius2;

        private int backUpTop = 0;
        public BackUp[] backUpList = new BackUp[100];

        private int colorRange = 10;

        private int sihuiId = 21;
        private int sihuidongId = 22;
        private int sanyuqiaoId = 158;
        private int sanhaoId = 263;
        private int erhaoId = 264;

        private object lockHandleImage = new object();

        public void init()
        {
            radius = (int)(radius * con.getScale());

            radius2 = radius * radius;
        }
        
        public Bitmap reduceBitmap(Bitmap src, int mode)
        {
            if (mode == 1)
            {
                while (backUpTop > 2)
                {
                    src = reduceBitmapOnce(src);
                }
            }
            else if (mode == 0)
            {
                while (backUpTop > 0)
                {
                    src = reduceBitmapOnce(src);
                }
            }
            return src;
        }
        public Bitmap reduceBitmapOnce(Bitmap src)
        {
            backUpTop--;
            for (int i = 0; i < backUpList[backUpTop].width; i++)
            {
                for (int j = 0; j < backUpList[backUpTop].height; j++)
                {
                    Color color = backUpList[backUpTop].fragment.GetPixel(i, j);
                    src.SetPixel(i + backUpList[backUpTop].LT.X, j + backUpList[backUpTop].LT.Y, color);
                }
            }
            return src;
        }
        public void handleImage(int handleNo, Bitmap b)
        {
            Point t = new Point();
            Color c = new Color();
            int mode = 0;
            handleImage(handleNo, b, t, c, mode);
        }
        public void handleImage(int handleNo, Bitmap b, int mode)
        {
            Point t = new Point();
            Color c = new Color();
            handleImage(handleNo, b, t, c, mode);
        }
        public void handleImage(int handleNo, Bitmap b, int id1, int id2, int id3, Color c)
        {
            Point t = new Point();
            int mode = 0;
            handleImage(handleNo, b, t, c, mode, id1, id2, id3,b);
        }
        public void handleImage(int handleNo, Bitmap b, Point t, Color c, int mode)
        {
            int id1 = 0;
            int id2 = 0;
            int id3 = 0;
            handleImage(handleNo, b, t, c, mode, id1, id2, id3,b);
        }
        public void handleImage(int handleNo, Bitmap b, Point t, int mode)
        {
            int id1 = 0;
            int id2 = 0;
            int id3 = 0;
            Color c = new Color();
            handleImage(handleNo, b, t, c, mode, id1, id2, id3,b);
        }
        public void handleImage(int handleNo, Bitmap b, Point t, Bitmap p, int mode)
        {
            Color c = new Color();
            int id1 = 0;
            int id2 = 0;
            int id3 = 0;
            handleImage(handleNo, b, t, c, mode, id1, id2, id3, p);
        }
        private void handleImage(int handleNo, Bitmap b, Point t,
            Color c, int mode, int id1, int id2, int id3,Bitmap p)
        {
            lock (lockHandleImage)
            {
                if (handleNo == 0)
                {
                    //show(b);
                }
                else if (handleNo == 1)
                {
                    reduceBitmap(b, mode);
                }
                else if (handleNo == 2)
                {
                    drowPoint(b, t, c, mode);
                }
                else if (handleNo == 3)
                {
                    reduceBitmapOnce(b);
                }
                else if (handleNo == 4)
                {
                    drawLine(b, id1, id2, id3, c);
                }
                else if(handleNo == 5)
                {
                    drawPicture(b, t,p);
                }
                else if (handleNo == 6)
                {
                    record(b, t, mode);
                }
                else if (handleNo == 7)
                {
                    record(b, t,p,mode);
                }

            }
        }
        private void record(Bitmap src, Point t, Bitmap picture, int mode)
        {
            Point small = new Point(0, 0);
            Point big = new Point(0, 0);
            small.X = t.X - picture.Width / 2;
            big.X = t.X + picture.Width / 2;
            small.Y = t.Y - picture.Height;
            big.Y = t.Y;
            record(src, small, big, mode);
        }
        private void record(Bitmap src, Point small, Point big, int mode)
        {
            if (mode == 0)
            {
                Bitmap des = new Bitmap(big.X - small.X, big.Y - small.Y);

                backUpList[backUpTop].LT.X = small.X;
                backUpList[backUpTop].LT.Y = small.Y;
                backUpList[backUpTop].width = big.X - small.X;
                backUpList[backUpTop].height = big.Y - small.Y;

                for (int i = small.X; i < big.X; i++)
                {
                    for (int j = small.Y; j < big.Y; j++)
                    {
                        Color color = src.GetPixel(i, j);
                        des.SetPixel(i - small.X, j - small.Y, color);
                    }
                }

                backUpList[backUpTop].fragment = des;
                backUpTop++;
            }
            else if (mode == 1)
            {

            }

        }
        private void record(Bitmap src, Point t, int mode)
        {
            if (mode == 0)
            {

            }
            else if (mode == 1)
            {
                Bitmap des = new Bitmap(2 * radius, 2 * radius);

                //Graphics g = Graphics.FromImage(src);
                //Rectangle rg = new Rectangle(t.X-radius,t.Y-radius, 2 * radius, 2 * radius);
                //g.DrawImage(des, rg);

                backUpList[backUpTop].LT.X = t.X - radius;
                backUpList[backUpTop].LT.Y = t.Y - radius;
                backUpList[backUpTop].width = 2 * radius;
                backUpList[backUpTop].height = 2 * radius;
                ///*
                for (int i = 0; i < backUpList[backUpTop].width; i++)
                {
                    for (int j = 0; j < backUpList[backUpTop].height; j++)
                    {
                        Color color = src.GetPixel(i + backUpList[backUpTop].LT.X, j + backUpList[backUpTop].LT.Y);
                        des.SetPixel(i, j, color);
                    }
                }
                //*/

                backUpList[backUpTop].fragment = des;
                backUpTop++;
            }
        }
        private void drawPicture(Bitmap src,Point t,Bitmap picture)
        {
            t.X -= picture.Width / 2;
            t.Y -= picture.Height;
            int scale = (int)(con.getScaleC());
            for(int i = 0;i < picture.Width;i++)
            {
                for(int j = 0;j < picture.Height;j++)
                {
                    Color color = picture.GetPixel(i, j);
                    if (color.R < 10 && color.B < 10 && color.G < 10)
                    {
                        
                    }
                    else
                    {
                        src.SetPixel(i+t.X, j+t.Y, color);
                    }
                }
            }

        }
        private void drowPoint(Bitmap src, Point t, Color color, int mode)
        {
            if (mode == 1)
            {
                for (int i = -radius; i < radius; i++)
                {
                    for (int j = -radius; j < radius; j++)
                    {
                        if (i * i + j * j < radius2)
                        {
                            src.SetPixel(i + t.X, j + t.Y, color);
                        }
                    }
                }
            }
        }
        private void drawLine(Bitmap src, int tId, int bId, int lId, Color c)
        {
            int scale = (int)(con.getScaleC());

            Point t = con.getId(tId);
            Point b = con.getId(bId);

            Point big = new Point(0, 0);
            Point small = new Point(0, 0);
            big.X = (t.X > b.X) ? t.X : b.X;
            big.Y = (t.Y > b.Y) ? t.Y : b.Y;

            small.X = (t.X <= b.X) ? t.X : b.X;
            small.Y = (t.Y <= b.Y) ? t.Y : b.Y;

            big.Y += radius;
            big.X += radius;
            small.Y -= radius;
            small.X -= radius;

            if ((tId == sihuiId && bId == sihuidongId)
                || (tId == sihuidongId && bId == sihuiId)
                || (tId == sanhaoId && bId == erhaoId))
            {
                Bitmap temp;
                if (lId == 0)
                {
                    temp = con.getBit(2);
                }
                else if(lId == 13)
                {
                    temp = con.getBit(3);
                }
                else// if (tId == sanhaoId)
                {
                    temp = con.getBit(5);

                    big.Y += (int)(359*con.getScale());

                }
                record(src, small, big, 0);

                for (int i = small.X; i < big.X; i++)
                {
                    for (int j = small.Y; j < big.Y; j++)
                    {
                        Color color = temp.GetPixel(i - small.X, j - small.Y);
                        if (color.R < 10)
                        {
                            src.SetPixel(i, j, c);
                        }
                    }
                }
            }
            else if ((tId == sanyuqiaoId && bId == sanhaoId)
                || (tId == erhaoId && bId == sanyuqiaoId))
            {
                Bitmap temp;
                Point middleT = new Point(0, 0);
                Point middleB = new Point(0, 0);
                if (tId == sanyuqiaoId)
                {
                    temp = con.getBit(4);

                    middleB.X = big.X;
                    middleB.Y = (int)(2556*con.getScale());

                    middleT.X = small.X;
                    middleT.Y = (int)(2513 * con.getScale());

                    small.X = (int)(6459 * con.getScale()); ;
                    big.X = (int)(6459 * con.getScale()); ;

                }
                else//if(tId == erhaoId)
                {
                    temp = con.getBit(6);

                    middleB.X = big.X;
                    middleB.Y = (int)(2556 * con.getScale()); ;

                    middleT.X = small.X;
                    middleT.Y = (int)(2513 * con.getScale()); ;

                    small.X = (int)(6459 * con.getScale()); ;
                    big.X = (int)(6459 * con.getScale()); ;
                }
                record(src, small, middleB, 0);
                record(src, middleT, big, 0);
                for (int i = small.X; i < middleB.X; i++)
                {
                    for (int j = small.Y; j < middleB.Y; j++)
                    {
                        Color color = temp.GetPixel(i - middleT.X, j - small.Y);
                        if (color.R < 10)
                        {
                            src.SetPixel(i, j, c);
                        }
                    }
                }
                for (int i = middleT.X; i < big.X; i++)
                {
                    for (int j = middleT.Y; j < big.Y; j++)
                    {
                        Color color = temp.GetPixel(i - middleT.X, j - small.Y);
                        if (color.R < 10)
                        {
                            src.SetPixel(i, j, c);
                        }
                    }
                }
            }
            else
            {
                record(src, small, big, 0);

                int R = con.getLine(lId).R;
                int G = con.getLine(lId).G;
                int B = con.getLine(lId).B;
                for (int i = small.X; i < big.X; i++)
                {
                    for (int j = small.Y; j < big.Y; j++)
                    {
                        Color color = src.GetPixel(i, j);
                        if (color.R - R < colorRange && color.R - R > -colorRange
                            && color.G - G < colorRange && color.G - G > -colorRange
                            && color.B - B < colorRange && color.B - B > -colorRange)
                        {
                            src.SetPixel(i, j, c);
                        }
                    }
                }
            }
        }
    }
}
