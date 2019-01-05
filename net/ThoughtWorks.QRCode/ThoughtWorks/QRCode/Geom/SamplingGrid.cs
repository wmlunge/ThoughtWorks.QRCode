namespace ThoughtWorks.QRCode.Geom
{
    using System;

    public class SamplingGrid
    {
        private AreaGrid[][] grid;

        public SamplingGrid(int sqrtNumArea)
        {
            this.grid = new AreaGrid[sqrtNumArea][];
            for (int i = 0; i < sqrtNumArea; i++)
            {
                this.grid[i] = new AreaGrid[sqrtNumArea];
            }
        }

        public virtual void adjust(Point adjust)
        {
            int x = adjust.X;
            int y = adjust.Y;
            for (int i = 0; i < this.grid[0].Length; i++)
            {
                for (int j = 0; j < this.grid.Length; j++)
                {
                    for (int k = 0; k < this.grid[j][i].XLines.Length; k++)
                    {
                        this.grid[j][i].XLines[k].translate(x, y);
                    }
                    for (int m = 0; m < this.grid[j][i].YLines.Length; m++)
                    {
                        this.grid[j][i].YLines[m].translate(x, y);
                    }
                }
            }
        }

        public virtual int getHeight()
        {
            return this.grid.Length;
        }

        public virtual int getHeight(int ax, int ay)
        {
            return this.grid[ax][ay].Height;
        }

        public virtual int getWidth()
        {
            return this.grid[0].Length;
        }

        public virtual int getWidth(int ax, int ay)
        {
            return this.grid[ax][ay].Width;
        }

        public virtual int getX(int ax, int x)
        {
            int num = x;
            for (int i = 0; i < ax; i++)
            {
                num += this.grid[i][0].Width - 1;
            }
            return num;
        }

        public virtual Line getXLine(int ax, int ay, int x)
        {
            return this.grid[ax][ay].getXLine(x);
        }

        public virtual Line[] getXLines(int ax, int ay)
        {
            return this.grid[ax][ay].XLines;
        }

        public virtual int getY(int ay, int y)
        {
            int num = y;
            for (int i = 0; i < ay; i++)
            {
                num += this.grid[0][i].Height - 1;
            }
            return num;
        }

        public virtual Line getYLine(int ax, int ay, int y)
        {
            return this.grid[ax][ay].getYLine(y);
        }

        public virtual Line[] getYLines(int ax, int ay)
        {
            return this.grid[ax][ay].YLines;
        }

        public virtual void initGrid(int ax, int ay, int width, int height)
        {
            this.grid[ax][ay] = new AreaGrid(this, width, height);
        }

        public virtual void setXLine(int ax, int ay, int x, Line line)
        {
            this.grid[ax][ay].setXLine(x, line);
        }

        public virtual void setYLine(int ax, int ay, int y, Line line)
        {
            this.grid[ax][ay].setYLine(y, line);
        }

        public virtual int TotalHeight
        {
            get
            {
                int num = 0;
                for (int i = 0; i < this.grid[0].Length; i++)
                {
                    num += this.grid[0][i].Height;
                    if (i > 0)
                    {
                        num--;
                    }
                }
                return num;
            }
        }

        public virtual int TotalWidth
        {
            get
            {
                int num = 0;
                for (int i = 0; i < this.grid.Length; i++)
                {
                    num += this.grid[i][0].Width;
                    if (i > 0)
                    {
                        num--;
                    }
                }
                return num;
            }
        }

        private class AreaGrid
        {
            private SamplingGrid enclosingInstance;
            private Line[] xLine;
            private Line[] yLine;

            public AreaGrid(SamplingGrid enclosingInstance, int width, int height)
            {
                this.InitBlock(enclosingInstance);
                this.xLine = new Line[width];
                this.yLine = new Line[height];
            }

            public virtual Line getXLine(int x)
            {
                return this.xLine[x];
            }

            public virtual Line getYLine(int y)
            {
                return this.yLine[y];
            }

            private void InitBlock(SamplingGrid enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public virtual void setXLine(int x, Line line)
            {
                this.xLine[x] = line;
            }

            public virtual void setYLine(int y, Line line)
            {
                this.yLine[y] = line;
            }

            public SamplingGrid Enclosing_Instance
            {
                get
                {
                    return this.enclosingInstance;
                }
            }

            public virtual int Height
            {
                get
                {
                    return this.yLine.Length;
                }
            }

            public virtual int Width
            {
                get
                {
                    return this.xLine.Length;
                }
            }

            public virtual Line[] XLines
            {
                get
                {
                    return this.xLine;
                }
            }

            public virtual Line[] YLines
            {
                get
                {
                    return this.yLine;
                }
            }
        }
    }
}

