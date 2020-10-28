using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Mouse
{
    abstract class MouseSolver
    {
        protected Grid m_Maze;
        protected Point m_CurrentPoint = new Point(0, 0);
        protected int m_iOrientation = 0;
        protected BitArray m_iVisitedCells;
        protected int[] iBoundaries = new int[4];

        internal MouseSolver() { }

        internal MouseSolver(Grid p_Maze)
        {
            m_Maze = p_Maze;
            m_iVisitedCells = new BitArray(m_Maze.XCells * m_Maze.YCells);
            m_iVisitedCells.SetAll(false);
        }

        internal abstract void Solve();

        protected void SetCurrentPoint(int p)
        {
            m_CurrentPoint.X = p % m_Maze.XCells;
            m_CurrentPoint.Y = (int)((p - m_CurrentPoint.X) / m_Maze.XCells);
        }

        protected int GetCell(Point p_Point)
        {
            return (p_Point.Y * m_Maze.YCells) + p_Point.X;
        }

        protected void SetVisited(int p_Cell)
        {
            m_iVisitedCells[p_Cell] = true;
        }

        protected bool IsVisited(int p_Cell)
        {
            return (m_iVisitedCells[p_Cell] == true);
        }

    }



    class RandomMouseMazeSolver : MouseSolver 
    {
        Random m_Rand = new Random();


        private RandomMouseMazeSolver() { }

        internal RandomMouseMazeSolver(Grid p_Maze): base(p_Maze)
        {

        }



        internal override void Solve()
        {
        
            int iSelectedNeighbourIndex = m_iOrientation;
            double dSelectedScore = -1;
            double dScore;
            
            do
            {
                iBoundaries = m_Maze.GetBoundaries(m_CurrentPoint);

                if ((iBoundaries[m_iOrientation] != -1))
                {
                    SetCurrentPoint(iBoundaries[m_iOrientation]);
                    m_Maze.AddPathPoint(m_CurrentPoint);
                }
                else
                {
                    dSelectedScore = -1;

                    for (int i = 0; i < 4; i++)
                    {
                        if (iBoundaries[i] != -1)
                        {
                            dScore = m_Rand.NextDouble();

                            if (dScore > dSelectedScore)
                            {
                                iSelectedNeighbourIndex = i;
                                dSelectedScore = dScore;
                            }
                        }
                    }
                    m_iOrientation = iSelectedNeighbourIndex;
                    SetCurrentPoint(iBoundaries[iSelectedNeighbourIndex]);
                    m_Maze.AddPathPoint(m_CurrentPoint);
                }

                System.Threading.Thread.Sleep(1);

            }
            while (!m_Maze.IsExit(m_CurrentPoint));

        }

    }



    class WallFollowerMazeSolver:MouseSolver
    {

        


        private WallFollowerMazeSolver() { }

        internal WallFollowerMazeSolver(Grid p_Maze) : base(p_Maze)
        {

        }



        internal override void Solve()
        {

            int iFacingDirection = 0;

            int p = 0;

            //// assume start at zero
            SetVisited(0);


            do
            {
                iBoundaries = m_Maze.GetBoundaries(m_CurrentPoint);

                if ((iBoundaries[(m_iOrientation + 1) % 4] != -1))
                {
                    iFacingDirection = (m_iOrientation + 1) % 4;

                    SetCurrentPoint(iBoundaries[iFacingDirection]);
                    SetVisited(iBoundaries[iFacingDirection]);
                    m_Maze.AddPathPoint(m_CurrentPoint);
                    m_iOrientation = iFacingDirection;
                }
                else
                {
                    for (int i = m_iOrientation+4; i > m_iOrientation; i--)
                    {
                        iFacingDirection = i%4;// (i + m_iOrientation) % 4;
                        if ((iBoundaries[iFacingDirection] != -1))
                        {
                            SetCurrentPoint(iBoundaries[iFacingDirection]);
                            SetVisited(iBoundaries[iFacingDirection]);
                            m_Maze.AddPathPoint(m_CurrentPoint);
                            m_iOrientation = iFacingDirection;


                            break;
                        }
                    }
                }
                p++;
            }
            while (!m_Maze.IsExit(m_CurrentPoint));
        }



    }
}
