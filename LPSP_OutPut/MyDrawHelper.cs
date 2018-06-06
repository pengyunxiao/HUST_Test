using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace HUST_OutPut
{
    class MyDrawHelper
    {
        public double xInterval { get; set; }                               //保存x轴两点间的间隔
        public double yInterval { get; set; }                                //保存y轴两点间的间隔
        private Int32 disDays;                                            //保存要显示的天数
        private Int32 curDay;                                   //当前显示起始日
        private Int32 curMonth;                                //当前显示起始月
        private Int32 curYear;
        private Int32 maxValueOfData;                         //保存数据中最大的值
        private Int32 pictureHeight;                            //保存画图区域的高度
        private Int32 pictureWidth;                             //保存画图区域的宽度
        // public Point zeroPoint { get; set; }                            //保存画图的零点
        private Int32 numOfDisPoint;                           //保存每行数据的点数 
        private Point zeroPoint;                                             //保存画图的零点,即坐标左下角
        private bool isDefaultUnit;                               //保存是否以默认单位显示
        private bool isDisGird;                                  //是否显示网格
        private String pictureTip;                              //保存图片的一些信息
        Dictionary<Int32, Int32> flgToPriority = null;         //Flg/100与priorities对应，即与画刷列表下标的对应
     
        //保存下标的常量
        private const Int32 oneDayPoints = 12*24;                             //保存一天一个Flg数据的点数
        private const Int32 oneRowPoints = 24;                                //保存一个Flg一行数据的点数
        private const Int32 oneDayRows = 12 * 14;                             //保存一天所含数据的行数12为每个Flg每天12行，14为共有14个Flg
        private const Int32 columnsOfOneRow = 27;                             //一行数据的列数
        private const Int32 numOfFlgToFill = 14;                              //保存要填充的Flg数
        private const Int32 numOfFlg = 14;                                    //保存所有Flg的个数
        private const Int32 rowsOfOneFlg = 12;                                //保存Flg下的行数
        private const Int32 oneYearPoints = 12 * 24 * 365;

        public MyDrawHelper(Int32 curYear, Int32 curMonth, Int32 curDay, Int32 maxValueOfData, 
            bool isDefaultUnit, String pictureTip, Boolean isDisGird)
        {
            this.curDay = curDay;
            this.curMonth = curMonth;
            this.curYear = curYear;
            this.maxValueOfData = maxValueOfData;           
            this.xInterval = 0;
            this.disDays = 1;
            this.numOfDisPoint = disDays * oneDayPoints;
            this.isDefaultUnit = isDefaultUnit;
            this.pictureTip = pictureTip;
            this.isDisGird = isDisGird;
            flgToPriority = creteFlgToPriorityDic();
        }

        public void setZeroPoint(Int32 x, Int32 y)
        {
            this.zeroPoint = new Point(x, y);
        }
        public void setDisDays(int days)
        {
            this.disDays = days;
            this.numOfDisPoint = disDays * oneDayPoints; 
        }
        public void setXInterval(Int32 pictureWide)
        {
            this.pictureWidth = pictureWide;
            this.xInterval = pictureWide * 0.87 / (disDays * oneDayPoints);
        }
        public void setYInterval(Int32 pictureHeight)
        {
            this.pictureHeight = pictureHeight;
            this.yInterval = (maxValueOfData / (pictureHeight * 0.9 / 10.2 * 10));
        }
        public void setXYInterval(Int32 pictureWide, Int32 pictureHeight)
        {
            setXInterval(pictureWide);
            setYInterval(pictureHeight);
        }

        //构造Dictionary
        //根据LDC数据的Flg/100使其与Config文件中selectItems的Priority对应
        //因为画刷的列表是按照Priority，所以即对Flg/100与画刷列表下标相对应
        //因为Priority从1开始，故Priority添加时都要减一
        private Dictionary<int, int> creteFlgToPriorityDic()
        {
            Dictionary<int, int> flgToPriority = new Dictionary<int, int>();
            flgToPriority.Add(1, 6 - 1); flgToPriority.Add(2, 10 - 1);
            flgToPriority.Add(3, 9 - 1); flgToPriority.Add(4, 6 - 1);
            flgToPriority.Add(5, 8 - 1); flgToPriority.Add(22, 1 - 1);
            flgToPriority.Add(21, 7 - 1); flgToPriority.Add(23, 5 - 1);
            flgToPriority.Add(24, 2 - 1); flgToPriority.Add(25, 4 - 1); //水电弃水、调峰不足不显示
            flgToPriority.Add(26, 3 - 1);
            return flgToPriority;
        }
        //返回各个Flg对应的画刷下标
        private int[] getBrushArray()
        {
            Int32[] result = new Int32[numOfFlgToFill];

            int index = 0;
            for (int i = 1; i <= 5; i++)
                flgToPriority.TryGetValue(i, out result[index++]);
            for (int i = 21; i <= 26; i++ )
                flgToPriority.TryGetValue(i, out result[index++]);
            return result;
        }
        //根据Flg/100获得画刷下标
        public Int32 getBrushArrayIndex(Int32 flg)
        {
            Int32 result=-1;
            flgToPriority.TryGetValue(flg, out result);
            return result;
        }
# region 废弃代码
        ///*
        // * 将一天的数据画出
        // * @para dayDrift 保存偏移最开始天的天数
        // * @para prePoints  保存上一个Flg或者上一天的最后点，用来连接不同天
        // */
        //private Int32 drawOneDay(MyFunPictureBox picture, Graphics g, Pen thinPen, Pen fatPen,
        //    int drawMonth, int drawDay, List<MyBrush> fillBrushes, List<Point> prePoints, Point startPoint)
        //{
        //    Int32 maxRight = 0;

        //    //TODO 硬编码 12为LDC表中Flg以5分钟为间隔，每小时12个，14为LDC表中的不同Flg个数        
        //    int dayStartIndex = drawDay * 12 * 14;                         //保存当前天在源数据下的开始坐标
        //    int flgIndexDiff = 12;                                         //不同Flg的下标间隔

        //    ////保存上一个Flg的点，用于连接不同Flg，14为Flg的个数
        //    //List<Point> prePoin = new List<Point>(14);                     
        //    //因为对应Flg下有0,5,10.....55共12个，即每隔5分钟有一份数据
        //    for (int num = 0; num < flgIndexDiff; num++)
        //    {
        //        //从Flg为100开始，即dayStartIndex偏移12开始， 是因为Flg = 0，27,28时划线，不画方形图
        //        //num表示第num个五分钟的数据
        //        DataRow topLine = picture.LevelLines[drawMonth].Rows[dayStartIndex + flgIndexDiff+num];
        //        DataRow bottomLine = createBottomRow(picture.LevelLines[0]);

        //        int flg = Int32.Parse(topLine["LDC.Flg"].ToString());
        //        int brushIndex = 0;
        //        if (num == 0 && flgToPriority.TryGetValue(flg / 100, out brushIndex))
        //        { 
        //            g.FillPolygon(
        //            fillBrushes[brushIndex].myBrush,
        //            new Point[]{prePoints[1],
        //            new Point(startPoint.X,Int32.Parse(topLine["LDC.H1"].ToString())),
        //            new Point(startPoint.X,Int32.Parse(bottomLine["LDC.H1"].ToString())),
        //            prePoints[0]}
        //            );
        //        }
        //        else if (num == flgIndexDiff - 1)
        //        {
        //            prePoints[0] = new Point((int)(startPoint.X + 24 * xInterval * 12), 
        //                Int32.Parse(bottomLine["LDC.H1"].ToString()));
        //            prePoints[1] = new Point((int)(startPoint.X + 24 * xInterval * 12),
        //                Int32.Parse(topLine["LDC.H1"].ToString()));
        //        }
        //        fillInterval(topLine, bottomLine, g, fillBrushes, num, startPoint);
        //        //TODO 硬编码 11 为画方形图的Flg个数，即Flg = 1,2,3,4,5,21,22,23,24,25,26共11个
        //        //其中Flg为1的方形图由上面代码画出
        //        for (int i = 1; i < 11; i++)
        //        {
        //            bottomLine = picture.LevelLines[drawMonth].Rows[dayStartIndex + i * flgIndexDiff + num];
        //            topLine = picture.LevelLines[drawMonth].Rows[dayStartIndex + (i + 1) * flgIndexDiff + num];
        //            fillInterval(topLine, bottomLine, g, fillBrushes, num, startPoint);

        //            flg = Int32.Parse(topLine["LDC.Flg"].ToString());
        //            if (num == 0 && flgToPriority.TryGetValue(flg / 100, out brushIndex))
        //            {
        //                g.FillPolygon(
        //                fillBrushes[brushIndex].myBrush,
        //                new Point[]{prePoints[1],
        //                new Point(startPoint.X,Int32.Parse(topLine["LDC.H1"].ToString())),
        //                new Point(startPoint.X,Int32.Parse(bottomLine["LDC.H1"].ToString())),
        //                prePoints[0]}
        //                );
        //            }
        //            else if (num == 12)
        //            {
        //                prePoints[i+1] = new Point((int)(startPoint.X + 24 * xInterval * 12),
        //                    Int32.Parse(topLine["LDC.H1"].ToString()));
        //            }
        //        }
        //    }

        //    return maxRight;
        //}
        ///*
        // * 将所给两行的数据中间填充颜色
        // * @para indexOfMin         表示一天中的第几个5分钟的数据
        // */
        //private void fillInterval(DataRow topLine, DataRow bottomLine, Graphics g,
        //    List<MyBrush> fillBrushes, int indexOfMin, Point startPoint)
        //{
        //    int flg = Int32.Parse(topLine["LDC.Flg"].ToString());
        //    int brushIndex = 0;
        //    //TODO 硬编码 此处24位 LDC表中的H1、H2..........H24
        //    for (int i = 1; i < 24; i++)
        //    {
        //        int topLeftLineVal = this.zeroPoint.Y - (int)(float.Parse(topLine["LDC.H" + i].ToString())/yInterval);
        //        int bottomLeftLineVal = this.zeroPoint.Y - (int)(float.Parse(bottomLine["LDC.H" + i].ToString()) / yInterval);
        //        if (bottomLeftLineVal < topLeftLineVal)
        //        {
        //            topLeftLineVal = bottomLeftLineVal;
        //            topLine["LDC.H" + i] = bottomLine["LDC.H" + i];
        //        }

        //        int topRightLineVal = this.zeroPoint.Y - (int)(float.Parse(topLine["LDC.H" + (i + 1)].ToString()) / yInterval);
        //        int bottomRightLineVal = this.zeroPoint.Y - (int)(float.Parse(bottomLine["LDC.H" + (i + 1)].ToString()) / yInterval);
        //        if (topRightLineVal > bottomRightLineVal)
        //        {
        //            topRightLineVal = bottomLeftLineVal;
        //            topLine["LDC.H" + (i + 1)] = bottomLine["LDC.H" + (i + 1)];
        //        }               
        //        if (flgToPriority.TryGetValue(flg/100, out brushIndex))
        //        {
        //            g.FillPolygon(
        //            fillBrushes[brushIndex].myBrush,
        //            new Point[]{new Point((int)((i-1)*this.xInterval + startPoint.X + indexOfMin*24*xInterval), topLeftLineVal),
        //            new Point((int)((i)*this.xInterval+ startPoint.X + indexOfMin*24*xInterval),topRightLineVal),
        //            new Point((int)((i)*this.xInterval+ startPoint.X + indexOfMin*24*xInterval),bottomRightLineVal),
        //            new Point((int)((i-1)*this.xInterval+ startPoint.X + indexOfMin*24*xInterval),bottomLeftLineVal)}
        //            );
        //        }
        //    }
        //}

        ///*
        // * 构造坐标的最低下的一行数据
        // */
        //private DataRow createBottomRow(DataTable mytable)
        //{
        //    DataRow newRow = mytable.NewRow();
        //    for (int i = 1; i <= 24; i++)
        //    {
        //        newRow["LDC.H" + i] = 0;
        //    }
        //    return newRow;
        //}

        ///*
        // * 构造默认的连接点
        // */
        //public List<Point> createPrePoints()
        //{
        //    List<Point> prePoints = new List<Point>(14);
        //    for (int i = 0; i < prePoints.Count; i++)
        //        prePoints[i] = new Point(zeroPoint.X, zeroPoint.Y);
        //    return prePoints;
        //}

        ///*
        // * 将一年的点全部画出
        // */
        //public void drawOneYear(MyFunPictureBox picture, Graphics g, Pen thinPen,
        //    Pen fatPen, List<MyBrush> fillBrushes)
        //{
        //    List<Point> leftPoints = new List<Point>(numOfFlgToFill+1);      //保存在左面的一列点
        //    List<Point> rightPoints = new List<Point>(numOfFlgToFill + 1);   //保存在右面的一列点

        //    for (int i = 0; i < numOfFlgToFill + 1; i++)
        //    {
        //        rightPoints.Add(new Point(zeroPoint.X, zeroPoint.Y));
        //    }


        //    int numOfXInterval = 0;
        //    int[] indexOfBrush = new int[12];                   //保存画笔下标
        //    //每行数据确定画笔颜色下标
        //    indexOfBrush = getBrushArray();

        //    for (int i = 0; i < picture.LevelLines.Count; i++)
        //    {
        //        int numOfCol = picture.LevelLines[i].Rows.Count / numOfFlg;
        //        for (int j = 0; j < numOfCol; j++)
        //        {
        //            for (int h = 0; h < oneRowPoints; h++)
        //            {
        //                leftPoints = new List<Point>(rightPoints);
        //                int xVal = (Int32)(numOfXInterval * xInterval) + zeroPoint.X;
        //                int preYVal = zeroPoint.Y;
        //                rightPoints[0] = new Point(xVal, zeroPoint.Y);
        //                for (int k = 1; k < rightPoints.Count; k++)
        //                {
        //                    Int32 yVal = zeroPoint.Y-(Int32)(float.Parse(picture.LevelLines[i].Rows[j * numOfFlg + k * rowsOfOneFlg][h + 1].ToString())/yInterval);
                            
        //                    //用于确保Flg递增时其大小递增
        //                    if (yVal > preYVal)
        //                        yVal = preYVal;

        //                    rightPoints[k] = new Point(xVal, yVal);
        //                    g.FillPolygon(
        //                            fillBrushes[indexOfBrush[k-1]].myBrush,
        //                            new Point[] { leftPoints[k], rightPoints[k], 
        //                        rightPoints[k - 1], leftPoints[k - 1] });

        //                    preYVal = yVal;
        //                }

        //                numOfXInterval++;
        //            }

        //        }
        //    }
        //}
#endregion


        private int getDayNum(int year, int month)
        {
            switch (month)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                case 7:
                case 9:
                case 11:
                    return 31;
                case 1:

                    if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
                        return 29;
                    else
                        return 28;
                default:
                    return 30;
            }
        }
        private Int32 getStartIndex(Int32 curYear,Int32 curMonh, Int32 curDay)
        {
            Int32 result = 0;
            result += curDay;
            result *= oneDayRows;
            return result;
        }
        /*
         * 画出一天的数据
         * Flg = 0,27,28对应的点startPoint的下标为12,13,14
         * @return 衔接点
         */
        private List<MyPoint> drawOneDay(MyFunPictureBox picture, Graphics g, Pen thinPen,
            Pen fatPen, List<MyBrush> fillBrushes, Int32 drawMonth, Int32 startIndex, List<MyPoint> startPoint)
        {
            List<MyPoint> leftPoints = startPoint;                             //保存在左面的一列点
            List<MyPoint> rightPoints = new List<MyPoint>(startPoint);   //保存在右面的一列点

            int[] indexOfBrush = new int[12];                   //保存画笔下标
            //每行数据确定画笔颜色下标
            indexOfBrush = getBrushArray();
            //绘制Flg=0,27，28曲线是的画笔
            Pen dashPen0 = new Pen(Color.Black, 1.0f);
            dashPen0.DashStyle = DashStyle.Dash;
            Pen dashPen27 = new Pen(Color.Orange, 1.0f);
            dashPen27.DashStyle = DashStyle.Dash;
            Pen dashPen28 = new Pen(Color.HotPink, 1.0f);
            dashPen28.DashStyle = DashStyle.Dash;

            Int32 numOfInterval = 1;
            double xStartVal = startPoint[0].xVal;
            //打印各个Flg一行数据中所包含的点
            for (int curH = 0; curH < oneRowPoints; curH++)
            {

                //打印一天的点
                for (int curRow = 0; curRow < rowsOfOneFlg; curRow++)
                {
                    double xVal = xStartVal + xInterval * numOfInterval;
                    int preYVal = zeroPoint.Y;

                    //leftPoints保存rightPoints的值
                    leftPoints = new List<MyPoint>(rightPoints);


                    rightPoints[0] = new MyPoint(xVal, zeroPoint.Y);
                    //打印一个竖列的点
                    for (int curFlg = 1; curFlg < rightPoints.Count; curFlg++)
                    {
                        Int32 yVal = 0;
                        
                        //curFlg = 12,13,14的虚线对应Flg=0,27,28
                        if (curFlg == 12)
                        {
                            yVal = zeroPoint.Y - (Int32)(float.Parse(picture.LevelLines[drawMonth].Rows[startIndex + curRow][curH + 1].ToString()) / yInterval);
                            rightPoints[curFlg] = new MyPoint(xVal, yVal);
                            g.DrawLine(dashPen0, leftPoints[curFlg].getPoint(), rightPoints[curFlg].getPoint());
                            continue;
                        }
                        else if (curFlg == 13 || curFlg == 14)
                        {
                            yVal = zeroPoint.Y - (Int32)(float.Parse(picture.LevelLines[drawMonth].Rows[startIndex + curRow + (curFlg -1)* rowsOfOneFlg][curH + 1].ToString()) / yInterval); 
                            rightPoints[curFlg] = new MyPoint(xVal, yVal);
                            if(curFlg == 13)
                                g.DrawLine(dashPen27, leftPoints[curFlg].getPoint(), rightPoints[curFlg].getPoint());
                            else if(curFlg == 14)
                                g.DrawLine(dashPen28, leftPoints[curFlg].getPoint(), rightPoints[curFlg].getPoint());

                            continue;
                        }

                        yVal = zeroPoint.Y - (Int32)(float.Parse(picture.LevelLines[drawMonth].Rows[startIndex + curRow + curFlg * rowsOfOneFlg][curH + 1].ToString()) / yInterval); 
                        ////用于确保Flg递增时其大小递增 Flg=21（对应于curFlg=6）时除外
                        if (yVal > preYVal && curFlg != 6)
                            yVal = preYVal;

                        rightPoints[curFlg] = new MyPoint(xVal, yVal);

                        //Flg为22和5的构成电力不足
                        if (curFlg == 7)
                        {
                           // if (rightPoints[curFlg].yVal < rightPoints[curFlg-2].yVal)
                            g.FillPolygon(
                                    fillBrushes[indexOfBrush[curFlg - 1]].myBrush,
                                    new Point[] { leftPoints[curFlg].getPoint(), rightPoints[curFlg].getPoint(), 
                                rightPoints[curFlg - 2].getPoint(), leftPoints[curFlg - 2].getPoint() });
                        }
                        else if (curFlg == 6)
                        {
                            g.FillPolygon(
                                    fillBrushes[indexOfBrush[curFlg - 1]].myBrush,
                                    new Point[] { leftPoints[curFlg-1].getPoint(), rightPoints[curFlg-1].getPoint(), 
                                rightPoints[curFlg].getPoint(), leftPoints[curFlg].getPoint() });
                        }
                        else
                        {
                            g.FillPolygon(
                                    fillBrushes[indexOfBrush[curFlg - 1]].myBrush,
                                    new Point[] { leftPoints[curFlg].getPoint(), rightPoints[curFlg].getPoint(), 
                                rightPoints[curFlg - 1].getPoint(), leftPoints[curFlg - 1].getPoint() });
                        }


                        preYVal = yVal;
                    }
                    numOfInterval++;
                }

            }
            return rightPoints;
        }

        /*
         * 画出一段间隔的数据
         * @return 衔接点
         */
        public void drawPicture(MyFunPictureBox picture, Graphics g, Pen thinPen,
            Pen fatPen, List<MyBrush> fillBrushes)
        {
            List<MyPoint> startPoint = new List<MyPoint>(numOfFlgToFill + 1);      //保存开始的一列点
            for (int i = 0; i < numOfFlgToFill + 1; i++)
            {
                startPoint.Add(new MyPoint(zeroPoint.X, zeroPoint.Y));
            }

            Int32 startIndex = getStartIndex(curYear, curMonth, curDay);
            Int32 drawMonth = curMonth;
            Int32 maxDayVal = getDayNum(curYear, curMonth)-1;
            for (int drawDay = 0; drawDay < disDays; drawDay++)
            {
                //判断用户所选天是否超过当月最多天数，添加by孙凯 2016.3.21
                if (startIndex >= picture.LevelLines[drawMonth].Rows.Count)
                {
                    //MessageBox.Show("没有数据");
                    //如果超过当前月则进入下月
                    if (startIndex >= picture.LevelLines[drawMonth].Rows.Count)
                    {
                        drawMonth++;
                        if (drawMonth > 11)
                            return;
                        startIndex = 0;
                        for (int i = 0; i < numOfFlgToFill + 1; i++)
                        {
                            startPoint[i]=new MyPoint(zeroPoint.X + xInterval * oneDayPoints, zeroPoint.Y);
                        }
                    }
                    continue;
                }

                startPoint = drawOneDay(picture, g, thinPen, fatPen, fillBrushes, drawMonth, startIndex, startPoint);
                startIndex += oneDayRows;

                //如果超过当前月则进入下月
                if (startIndex >= picture.LevelLines[drawMonth].Rows.Count)
                {
                    drawMonth++;
                    if (drawMonth > 11)
                        return;
                    startIndex = 0;
                } 
             
            }
        }


        /*
         * 绘制X轴、Y轴
         */
        public void drawAxes(MyFunPictureBox picture, Graphics g)
        {
            Pen Axes = new Pen(Brushes.Black, 3.5F);
            Pen drawLinePen = new Pen(Brushes.Black, 3F);
            Point yAxesPoint = new Point(zeroPoint.X, picture.drawArea.Top);
            Point xAxesPoint = new Point(picture.drawArea.Right, zeroPoint.Y);
            
            //画出X轴横线
            g.DrawLine(Axes, zeroPoint, xAxesPoint);
            g.DrawLine(drawLinePen, xAxesPoint, new Point(xAxesPoint.X - 10, xAxesPoint.Y - 5));
            g.DrawLine(drawLinePen, xAxesPoint, new Point(xAxesPoint.X - 10, xAxesPoint.Y + 5));
            //画出Y轴数线
            g.DrawLine(Axes, zeroPoint, yAxesPoint);
            g.DrawLine(drawLinePen, yAxesPoint, new Point(yAxesPoint.X - 5, yAxesPoint.Y + 10));
            g.DrawLine(drawLinePen, yAxesPoint, new Point(yAxesPoint.X + 5, yAxesPoint.Y + 10));

            double yStep = drawYIdentity(picture, g);

            //画出X轴的标识
            drawXIdentity(picture, g, yStep);
            
        }
        /*
         * 绘制Y轴的标注
         * 返回Y轴标注间隔距离
         */
        private double drawYIdentity(MyFunPictureBox picture, Graphics g)
        {
            Point yAxesPoint = new Point(zeroPoint.X, picture.drawArea.Top);
            //字符字体及颜色
            Font drawFont = new Font("宋体", picture.smallFontSize);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            StringFormat stringFormat = new StringFormat();
            //画出Y轴标识
            stringFormat.Alignment = StringAlignment.Far;
            double ystep = picture.drawArea.Height * 0.9 / 10.2;
            for (int i = 1; i < 11; i++)
            {
               // g.DrawLine(drawAxisPen, zeroPoint.X, zeroPoint.Y - (int)(i * ystep), zeroPoint.X - 5, zeroPoint.Y - (int)(i * ystep));

                Rectangle rect = new Rectangle(
                    zeroPoint.X - 70, zeroPoint.Y - (int)(i * ystep) - drawFont.Height / 2, 60, drawFont.Height);
                if (isDefaultUnit)
                    g.DrawString(((int)(yInterval * i * ystep) / 10 * 10).ToString(), drawFont, drawBrush, rect, stringFormat);
                else
                    g.DrawString(((int)(yInterval * i * ystep * 0.1)).ToString(), drawFont, drawBrush, rect, stringFormat);
            }
            if (isDefaultUnit)
                g.DrawString("MW", drawFont, drawBrush, zeroPoint.X - 2 * drawFont.SizeInPoints, yAxesPoint.Y - drawFont.Height / 2);
            else
                g.DrawString("万kW", drawFont, drawBrush, zeroPoint.X - 4 * drawFont.SizeInPoints, yAxesPoint.Y - drawFont.Height / 2);


            Font nameFont = new Font("宋体", picture.largeFontSize + 1, FontStyle.Bold);

            String[] tips = pictureTip.Split('\n');
            for (int i = 0; i < tips.Length; i++)
            {
                g.DrawString(tips[i], nameFont, drawBrush,
                    (float)(picture.drawArea.Left + picture.drawArea.Width * 0.1 + (picture.drawArea.Width * 0.9 + (tips[i].Length*2) / 2 * 15) / 2)
                    , (float)(picture.drawArea.Top + picture.drawArea.Height * 0.93 + i * 15), stringFormat);
            }
            nameFont.Dispose();
            return ystep;
        }
        /************************************************************************/
        /* 画出X轴的标识                                                                     */
        /************************************************************************/
        private void drawXIdentity(MyFunPictureBox picture, Graphics g, double yStep)
        {
            //字符字体及颜色
            Font drawFont = new Font("宋体", picture.smallFontSize);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            Pen drawAxisPen = new Pen(Brushes.Black, 1.5F);                     //X轴小竖线

            //日期
            Int32 day = curDay;
            Int32 month = curMonth;
            //绘制网格
            Pen girdPen = new Pen(Brushes.Gray, 1);
            girdPen.DashStyle = DashStyle.Dash;
            //分钟、天、月的间隔像素
            double minStep = xInterval * rowsOfOneFlg;
            double dayStep = xInterval * oneDayPoints;
            double monthStep = xInterval * oneDayPoints * getDayNum(curYear, curMonth);
            g.DrawString((month + 1) + "月" + (day + 1) + "日", drawFont, drawBrush,
                    zeroPoint.X - 2 * drawFont.SizeInPoints, zeroPoint.Y + 3, stringFormat);
            //绘制网格横线
            for (int i = 1; isDisGird && i < 11; i++ )
            {
                g.DrawLine(girdPen, zeroPoint.X, zeroPoint.Y - (int)(yStep * i), 
                    zeroPoint.X + (int)(disDays * dayStep), zeroPoint.Y - (int)(yStep * i));
            }
            if (minStep > 25)
            {
                for (int numOfDay = 1; numOfDay <= disDays; numOfDay++)
                {
                    for (int numOfMin = 1; numOfMin < oneRowPoints; numOfMin++)
                    {
                        //绘制网格
                        if (isDisGird)
                            g.DrawLine(girdPen, zeroPoint.X + (int)(numOfMin * minStep), zeroPoint.Y,
                                zeroPoint.X + (int)(numOfMin * minStep), zeroPoint.Y - (int)(yStep * 10));

                        g.DrawLine(drawAxisPen, zeroPoint.X + (int)(numOfMin * minStep), zeroPoint.Y,
                            zeroPoint.X + (int)(numOfMin * minStep), zeroPoint.Y - 10);
                        g.DrawString(numOfMin.ToString(), drawFont, drawBrush,
                            zeroPoint.X + (int)(numOfMin * minStep), zeroPoint.Y + 3, stringFormat);
                    }
                    day++;
                    if (day >= getDayNum(curYear, month))
                    {
                        month++;
                        day = 0;
                        //绘制网格
                        if(isDisGird)
                            g.DrawLine(girdPen, zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y,
                                zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y - (int)(yStep * 10));

                        g.DrawLine(drawAxisPen, zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y,
                            zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y - 10);
                        g.DrawString((month + 1) + "月" + (day + 1) + "日", drawFont, drawBrush,
                            zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y + 3, stringFormat);
                    }
                    else
                    {
                        //绘制网格
                        if (isDisGird)
                            g.DrawLine(girdPen, zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y,
                            zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y - (int)(yStep * 10));

                        g.DrawLine(drawAxisPen, zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y,
                            zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y - 10);
                        g.DrawString((day + 1) + "日", drawFont, drawBrush,
                            zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y + 3, stringFormat);
                    }


                }

            }
            else
            {
                int numOfday;
                for(numOfday = 1; numOfday <= disDays/2 && numOfday <= getDayNum(curYear, month)/2; numOfday++)
                {
                    double xStep = numOfday*dayStep;
                    int numOfxStep = 0;
                    if(xStep > 30)
                    {
                        for (int i = 1; i <= disDays; i+=numOfday)
                        {
                            day += numOfday;
                            numOfxStep++;

                            if (day >= getDayNum(curYear, month))
                            {
                                month++;
                                day = 0;
                                //绘制网格
                                if (isDisGird)
                                    g.DrawLine(girdPen,  zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y,
                                    zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y - (int)(yStep * 10));

                                g.DrawLine(drawAxisPen, zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y,
                                    zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y - 10);
                                if(month < 12)
                                    g.DrawString((month + 1) + "月" + (day + 1) + "日", drawFont, drawBrush,
                                        zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y + 3, stringFormat);
                            }
                            else
                            {
                                //绘制网格
                                if (isDisGird)
                                    g.DrawLine(girdPen, zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y,
                                    zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y - (int)(yStep * 10));

                                g.DrawLine(drawAxisPen, zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y,
                                    zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y - 10);
                                if (month < 12)
                                    g.DrawString((day + 1) + "日", drawFont, drawBrush,
                                        zeroPoint.X + (int)(xStep*numOfxStep), zeroPoint.Y + 3, stringFormat);
                            }

                        }
                        break;
                    }
                }
                if (numOfday > disDays/2 || numOfday > getDayNum(curYear, month) / 2)
                {
                    Int32 startDay = -1;
                    for (int numOfDay = 0; numOfDay <= disDays && month < 11; numOfDay++)
                    {
                        if ((day + numOfDay) >= (startDay + getDayNum(curYear, month)))
                        {
                            startDay = day + numOfDay;
                            day = 0; month++;
                            //绘制网格
                            if (isDisGird)
                                g.DrawLine(girdPen, zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y,
                                zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y - (int)(yStep * 10));

                            g.DrawLine(drawAxisPen, zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y,
                                zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y - 10);
                            if (month < 12)
                                g.DrawString((month + 1) + "月" + (day + 1) + "日", drawFont, drawBrush,
                                    zeroPoint.X + (int)(numOfDay * dayStep), zeroPoint.Y + 3, stringFormat);
                        }
                    }
                }
            }
        }
#region 按键事件用到的函数
        /*
         * 左移一天
         */
        public Boolean dayLeftMove()
        {
            if (curDay > 0)
            {
                curDay--;
                return true;
            }
            else if(curDay == 0 && curMonth == 0)
                return false;
            else
            {
                curDay = getDayNum(curYear, curMonth - 1) - 1;
                curMonth--;
                return true;
            }
        }
        /*
         * 右移一天
         */
        public Boolean dayRightMove()
        {
            Int32 maxDay = getDayNum(curYear, curMonth);
            if (curDay < maxDay - 1)
            {
                curDay++;
                return true;
            }
            else if (curMonth < 11)
            {
                curDay = 0;
                curMonth++;
                return true;
            }
            else
                return false;
        }
        /*
         * 左移一月
         */
        public Boolean monthLeftMove()
        {
            if (curMonth > 0)
            {
                curMonth--;
                return true;
            }
            else
                return false;
        }
        /*
         * 右移一月
         */
        public Boolean monthRightMove()
        {
            if (curMonth < 11)
            {
                curMonth++;
                return true;
            }
            else
                return false;
        }
        /*
         * 增加X轴间距
         */
        public Boolean addDisDays()
        {
            disDays++;
            if (disDays > 356)
            {
                disDays--;
                return false;
            }
            else
            {
                this.xInterval = this.pictureWidth * 0.87 / (disDays * oneDayPoints);
                return true;
            }
        }
        /*
         * 减少X轴间距
         */
        public Boolean redDisDays()
        {
           
            disDays--;
            if (disDays < 1)
            {
                disDays++;
                return false;
            }
            else
            {
                this.xInterval = this.pictureWidth * 0.87 / (disDays * oneDayPoints);
                return true;
            }
        }
#endregion 按键事件用到的函数


    }
}

class MyPoint
{
    public double xVal { get; set; }
    public double yVal { get; set; }


    public MyPoint(double xVal, double yVal)
    {
        this.xVal = xVal;
        this.yVal = yVal;
    }

    public MyPoint(int xVal, int yVal)
    {
        this.xVal = xVal;
        this.yVal = yVal;
    }

    public Point getPoint()
    {
        return new Point((int)xVal, (int)yVal);
    }
}
