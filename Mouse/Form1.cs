using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mouse
{
    public partial class Form1 : Form
    {

        MouseSolverSystem m_System = new  MouseSolverSystem();

        System.Threading.Thread paintThread ;
        public Form1()
        {
            InitializeComponent();
            paintThread = new System.Threading.Thread(this.Repaint);
        }

        private void m_mainPanel_Paint(object sender, PaintEventArgs e)
        {
            m_System.Paint(sender, e);
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(m_System.ToString());
        }

        private void OnMazeGenerate(object sender, EventArgs e)
        {
            m_System.GenerateNewMaze();
            m_mainPanel.Invalidate();
            
        }

        private void OnMazeSolve(object sender, EventArgs e)
        {
            paintThread.Start();
            m_System.Solve();
            paintThread.Abort();
            m_mainPanel.Invalidate();
        }

        private void OnMazeStop(object sender, EventArgs e)
        {
            m_System.Stop();
            paintThread.Abort();
            m_mainPanel.Invalidate();
        }

        

        private void Repaint()
        {
            for (; ; )
            {
                m_mainPanel.Invalidate();
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
}
