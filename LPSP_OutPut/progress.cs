/*************************************
*  进度条功能类
*  The class of process function
*  Time:2016-3-30
*  Modified by Jiming Yin
**************************************/

using System.Windows.Forms;
using System.Threading;

namespace HUST_OutPut
{
    public partial class progress : Form
    {
        public bool isOver = false;  //指示任务是否完成，如果完成，则关闭进度条

        public progress()
        {
            InitializeComponent();
        }

        //using thread to show the progress
        public void Start()
        {
            Thread th = new Thread(new ThreadStart(this.AddValue));
            th.Start();
        }

        public void AddValue()
        {
            while (true)
            {
                if (isOver == false)
                {
                    if (this.progressBar1.Value < this.progressBar1.Maximum)
                        this.progressBar1.Value++;
                    else
                        this.progressBar1.Value = progressBar1.Minimum;
                    Thread.Sleep(50);
                }
                else
                    break;
            }
            this.Close();
        }
    }
}
