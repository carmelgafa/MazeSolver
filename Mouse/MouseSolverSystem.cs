using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mouse
{
    class MouseSolverSystem
    {
         
        Grid m_Grid = new Grid();
        MouseSolver m_Solver;


        internal MouseSolverSystem()
        {
            m_Solver = new WallFollowerMazeSolver(m_Grid);

            executionThread = new System.Threading.Thread(m_Solver.Solve);
        }

        internal void Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            m_Grid.Paint(sender, e);
        }


        System.Threading.Thread executionThread;


        internal void Solve()
        {
            executionThread.Start();
        }


        public override string ToString()
        {
            return m_Grid.ToString();
        }


        internal void GenerateNewMaze()
        {
            m_Grid.GenerateMaze();
        }

        internal void Stop()
        {
            executionThread.Suspend();
        }
    }
}
