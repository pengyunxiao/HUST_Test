using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace HUST_OutPut
{
    /**
     * 用于解析LDC表
     */
    class LDCTable
    {
        DataTable sourceData;
        String baseFilter;
        Int32 year;
        DataTable tableStruct;
        private Int32 maxValue = 0;
        /**
         * @para sourceData     MAP表
         * @para baseFilter     用户选择的过滤条件
         * @para year           年份
         * @para tableStruct    表的结构
         */
        public LDCTable(DataTable sourceData, String baseFilter,Int32 year, DataTable tableStruct)
        {
            this.sourceData = sourceData;
            this.baseFilter = baseFilter;
            this.year = year;
            this.tableStruct = tableStruct;
        }

        /**
         * 获取一年的LDC表数据保存到List<DataTable>
         */
        public List<DataTable> getLDCForDispaly()
        {
            List<DataTable> result = new List<DataTable>(12);
            for (Int32 i = 0; i < 12; i++)
            {
                Int32 mIDMin = i * 100;
                Int32 mIDMax = i * 100 + MyDate.getMaxDay(this.year, i);
                String mIDFilter = " and mID >= " + mIDMin + " and mID <= " + mIDMax;
                DataRow[] curMonthRows = this.sourceData.Select(
                    this.baseFilter + mIDFilter, "mID ASC, Flg ASC");
                //获取当前月的数据
                result.Add(getMonthTable(i, curMonthRows));
            }
            return result;
        }

        //保存每个Flg的起始数值
        private Int32[] flgStart = new Int32[] {
            0, 100, 200, 300, 400, 500, 
            2100, 2200, 2300, 2400, 2500, 
            2600, 2700, 2800
        };
        /*
         * 获取每个月的数据保存到DataTable中
         * @para month          当前月份
         * @para sourceRows     当前月所有的数据
         */
        private DataTable getMonthTable(Int32 month, DataRow[] sourceRows)
        {
            DataTable result = this.tableStruct.Clone();
            Int32 index = 0;
            Int32 maxDay = MyDate.getMaxDay(this.year, month);
            for (int i = 0; i < maxDay; i++)
            {           
                Int32 curMID = month * 100 + i;     //保存当前要添加天的mID
                Int32 sourceMID = Int32.Parse(sourceRows[index]["mID"].ToString()); //保存数据行中的mID
                //根据不同情况添加一天的数据
                addOneDayRows(result, curMID, sourceMID, ref index, sourceRows);
            }

            //TODO 修改TableName
            result.TableName = "表1^2010年 枯水年 Test_Sys 4月 3日24小时电力平衡示意图^^万kW/亿kWh";
            return result;
        }
        /*
         * 添加一天的数据到表
         * @para    result          保存数据到此表
         * @para    curMID          当前要保存行的mID
         * @para    sourceMID       当前操作数据源行的mID
         * @para    sourceRowIndex  当前操作到的数据源的下标
         * @para    sourceRows      数据源
         */
        private void addOneDayRows( DataTable result, Int32 curMID, Int32 sourceMID,
            ref Int32 sourceRowIndex, DataRow[] sourceRows)
        {
            //若mID相等，则添加每个flg下的数据
            if (sourceMID == curMID)
            {
                //暂存一天的数据，之后保存到result中，使DataTable中的行顺序为可以按天排列
                for (int flgIndex = 0; flgIndex < flgStart.Length; flgIndex++)
                {
                    //添加每个Flg下的数据
                    for (int min = 0; min < 60; min += 5)
                    {
                        //根据不同情况添加一行数据
                        Int32 curFlag = Int32.Parse(sourceRows[sourceRowIndex]["Flg"].ToString());
                        //若源数据存在与当前Flg相等的则直接添加，且index自加
                        if (curFlag == flgStart[flgIndex] + min)
                        {
                            DataRow newRow = result.NewRow();
                            newRow["LDC.Flg"] = sourceRows[sourceRowIndex]["Flg"];
                            newRow["LDC.RR"] = sourceRows[sourceRowIndex]["RR"];
                            newRow["LDC.SR"] = sourceRows[sourceRowIndex]["SR"];
                            for (int tmpV = 1; tmpV <= 24; tmpV++)
                            {
                                newRow["LDC.H" + tmpV] = sourceRows[sourceRowIndex]["H" + tmpV];
                                try
                                {
                                    float value = float.Parse(newRow["LDC.H" + tmpV].ToString());
                                    if ( value> maxValue)
                                        maxValue = (Int32)(float.Parse(newRow["LDC.H" + tmpV].ToString()));
                                }
                                catch (Exception )
                                {
                                    continue;
                                }

                            }
                            sourceRowIndex++;
                            result.Rows.Add(newRow);
                        }
                        //若源数据小于当前的Flg则说明添加出错，抛出异常
                        else if (curFlag < flgStart[flgIndex] + min)
                        {
                            throw new Exception("添加数据顺序出错");
                        }
                        //若大于则说明需要添加默认数据
                        else
                        {
                            addOneDefaultRow(result, flgStart[flgIndex] + min);
                        }
                    }
                }
            }
            //若数据源mID大于当前要添加mID，说明需要添加默认数据
            else if (sourceMID > curMID)
            {
                addOneDayDefalutRows(result);
            }
            //若数据源mID小于当前要添加mID，说明添加顺序出错，抛出异常
            else
            {
                throw new Exception("添加天数据顺序出错");
            }
        }
        //添加一天的默认数据
        private void addOneDayDefalutRows(DataTable table)
        {
            for (int i = 0; i < this.flgStart.Length; i++)
                for (int j = 0; j < 12; j++ )               //12为每隔5分钟有数据，一小时有12个数据
                    addOneDefaultRow(table, flgStart[i] + 5 * j);
        }
        //添加一行默认数据
        private void addOneDefaultRow(DataTable table, Int32 flg)
        {
            DataRow newRow = table.NewRow();

            newRow[0] = flg;
            for (int i = 1; i < newRow.ItemArray.Length; i++)
                newRow[i] = 0;
            table.Rows.Add(newRow);
        }

        //获得一行中的最大值
        public Int32 getMaxValue()
        {
            return maxValue;
        }
    }

    class MyDate
    {
        /*
         * 获得当前年，当前月的最多天数
         * @para    year        当前年
         * @para    month       当前月
         */
        public static Int32 getMaxDay(Int32 year, Int32 month)
        {
            if (month > 11 || month < 0)
                throw new Exception("月份错误");
            switch (month)
            {
                case 0: case 2: case 4: case 6: 
                    case 7: case 9: case 11:
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
    }
}
