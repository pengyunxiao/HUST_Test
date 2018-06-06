using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HUST_OutPut
{
    //公用到的操作
    public class MyTools
    {


        /// <summary>
        /// 判断testFile是否比targetFile更早生成
        /// </summary>
        /// <param name="testFile"></param>
        /// <param name="otherFile"></param>
        /// <returns></returns>
        static private bool IsOrderFile(string testFile,
            string targetFile)
        {
            bool result = true;
            try
            {
                System.IO.FileInfo testFileInfo =
                    new System.IO.FileInfo(testFile);

                System.IO.FileInfo targetFileInfo =
                    new System.IO.FileInfo(targetFile);

                result = testFileInfo.LastWriteTime 
                    < targetFileInfo.LastWriteTime;
            }
            catch (Exception )
            {
                return false;
            }



            return result;
        }
        /// <summary>
        /// 判断小时级输出文件是否较输入文件旧，
        /// 若较新的话提示用户可能修改输入文件后没有重新生成输出文件
        /// 提示用户可能出错
        /// </summary>
        /// <param name="startpath"></param>
        /// <returns></returns>
        static public bool HourWarning(string startpath)
        {
            string[] filepath = startpath.Split('\\');
            string filename = filepath[filepath.Length - 1];

            if (MyTools.IsOrderFile(startpath + "_RST.xml",
                    startpath + ".xml"))
            {
                if (MessageBox.Show(
                    "输入数据文件" + filename + ".xml"
                    + "的生成时间晚于结果数据文件" + filename
                    + "_RST.xml"
                    + "\n可能会运行出错！"
                     + "建议重新执行模拟计算后再选择输出。\n是否停止计算？"
                    , "警告", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            if (MyTools.IsOrderFile(startpath + "_GEN.xml",
                    startpath + ".xml"))
            {
                if (MessageBox.Show(
                    "输入数据文件" + filename + ".xml"
                    + "的生成时间晚于结果数据文件" + filename
                    + "_GEN.xml"
                    + "\n可能会运行出错！"
                     + "建议重新执行模拟计算后再选择输出。\n是否停止计算？"
                     , "警告", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            if (MyTools.IsOrderFile(startpath + "_MAP.xml",
                    startpath + ".xml"))
            {
                if (MessageBox.Show(
                    "输入数据文件" + filename + ".xml"
                    + "的生成时间晚于结果数据文件" + filename
                    + "_MAP.xml"
                    + "\n可能会运行出错！"
                     + "建议重新执行模拟计算后再选择输出。\n是否停止计算？"
                     , "警告", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            return false;
        }


        /// <summary>
        /// 判断分钟级输出文件是否较输入文件旧，
        /// 若较新的话提示用户可能修改输入文件后没有重新生成输出文件
        /// 提示用户可能出错
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public bool MinuteWarning(string startpath)
        {
            string[] filepath = startpath.Split('\\');
            string filename = filepath[filepath.Length - 1];
            if (MyTools.IsOrderFile(startpath + "_RST5.xml",
                    startpath + ".xml"))
            {
                if (MessageBox.Show(
                    "输入数据文件" + filename + ".xml"
                    + "的生成时间晚于结果数据文件" + filename
                    + "_RST5.xml"
                    + "\n可能会运行出错！"
                     + "建议重新执行模拟计算后再选择输出。\n是否停止计算？"
                     , "警告", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            if (MyTools.IsOrderFile(startpath + "_GEN5.xml",
                    startpath + ".xml"))
            {
                if (MessageBox.Show(
                    "输入数据文件" + filename + ".xml"
                    + "的生成时间晚于结果数据文件" + filename
                    + "_GEN5.xml"
                    + "\n可能会运行出错！"
                     + "建议重新执行模拟计算后再选择输出。\n是否停止计算？"
                     , "警告", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            if (MyTools.IsOrderFile(startpath + "_MAP5.xml",
                    startpath + ".xml"))
            {
                if (MessageBox.Show(
                    "输入数据文件" + filename + ".xml"
                    + "的生成时间晚于结果数据文件" + filename
                    + "_MAP5.xml"
                    + "\n可能会运行出错！"
                     + "建议重新执行模拟计算后再选择输出。\n是否停止计算？"
                     , "警告", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            return false;
        }
    }
}
