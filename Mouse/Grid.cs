using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace Mouse
{
    class Grid
    {
        private int m_iYCells = 20;
        internal int YCells
        {
            get { return m_iYCells; }
        }


        int m_iXCells = 20;
        internal int XCells
        {
            get { return m_iXCells; }
        }


        int m_iCellWidth = 20;

        float m_fBorder = 20;

        int[][] m_iMapCells;

        int[] m_iVisitedCells;

        Point m_StartPos;
        Point m_EndPos;
        
        Random m_Rand = new Random();

        List<Point> m_Path = new List<Point>();

        internal void AddPathPoint(int p_iXPos, int p_iYpos)
        {
            if (m_Path.Count > 5000)
                m_Path.Clear();

            m_Path.Add(new Point(p_iXPos, p_iYpos));
        }

        internal void AddPathPoint(Point p_Point)
        {
            if (m_Path.Count > 1000)
                m_Path.Clear();

            m_Path.Add(p_Point);
        }

        internal int[] GetBoundaries (int p_iXPos, int p_iYpos)
        {
            return m_iMapCells[(p_iXPos * m_iYCells) + p_iYpos];
        }

        internal Grid()
        {
            m_iMapCells = new int[m_iYCells * m_iXCells][];// order: Left, top, right, bottom
            for (int i = 0; i < (m_iYCells * m_iXCells); i++)
            {
                m_iMapCells[i] = new int[] { -1,-1,-1,-1};
            }
            m_iVisitedCells = new int[m_iYCells * m_iXCells];

            m_StartPos = new Point(0, 0);
            m_EndPos = new Point(m_iXCells - 1, m_iYCells - 1);

            InitializeMap();

            CreateDepthFirstSearchMaze();

            AddPathPoint(m_StartPos);

        }

        internal void GenerateMaze()
        {
            m_Path.Clear();
            
            m_iVisitedCells = new int[m_iYCells * m_iXCells];
            InitializeMap();
            CreateDepthFirstSearchMaze();

            AddPathPoint(m_StartPos);
        }

        public override string ToString()
        {
            StringBuilder sRet = new StringBuilder();

            for (int j = 0; j < m_iYCells; j++)
            {
                for (int i = 0; i < m_iXCells; i++)
                {
                    sRet.Append((j * m_iYCells) + i);
                    sRet.Append(" : ");
                    for (int k = 0; k < 4; k++)
                    {
                        sRet.Append(m_iMapCells[(j * m_iYCells) + i] [k]);
                        sRet.Append("  ");
                    }
                    sRet.Append("\n");
                }
            }
            return sRet.ToString();
        }

        private void InitializeMap()
        {
            int p_iCurrentCell = -1;

            for (int j = 0; j < m_iYCells; j++)
            {
                for (int i = 0; i < m_iXCells; i++)
                {
                    // fill neighbours left
                    p_iCurrentCell = (j * m_iYCells) + i;

                    if ((p_iCurrentCell + m_iYCells - 1) % m_iYCells != (m_iYCells - 1))
                    {
                        m_iMapCells[p_iCurrentCell][0] = p_iCurrentCell - 1;
                    }


                    // fill neighbours top
                    if ((p_iCurrentCell - m_iYCells) >= 0)
                    {
                        m_iMapCells[p_iCurrentCell][1] = p_iCurrentCell - m_iYCells;
                    }


                    // fill neighbours right
                    if (((p_iCurrentCell + 1) % m_iYCells) != 0)//  <= (m_iXCells - 1))
                    {
                        m_iMapCells[p_iCurrentCell][2] = p_iCurrentCell + 1;
                    }


                    // fill neighbours bottom
                    if ((p_iCurrentCell + m_iYCells) <= (m_iYCells * m_iXCells) - 1)
                    {
                        m_iMapCells[p_iCurrentCell][3] = p_iCurrentCell + m_iYCells;
                    }

                }
            }
        }

        private void ProcessDepthFirstSearchCell(int p_iCurrentCell, int p_iOriginatorCell)
        {
            // neighbours that are covered base case when all are -1
            int[] iVisitedNeighbours = new int[4]; // order: Left, top, right, bottom

            // it should contain the linked cells
            int[] iConnectedNeighbours = new int[4];

            // base case should be 0            
            int iNeighboursCount = 0;


            // set this cell as visited
            m_iVisitedCells[p_iCurrentCell] = -1;

            // fill neighbours but mark those already visited as unaccessible 
            // also mark  the originator cell as already visited
            for (int i = 0; i < 4; i++)
            {
                if (m_iMapCells[p_iCurrentCell][ i] == -1)
                {
                    iVisitedNeighbours[i] = -1;
                    iConnectedNeighbours[i] = -1;
                }
                else if (m_iVisitedCells[m_iMapCells[p_iCurrentCell][i]] == -1)
                {
                    iVisitedNeighbours[i] = -1;

                    if (m_iMapCells[p_iCurrentCell][ i] != p_iOriginatorCell)
                    {
                        iConnectedNeighbours[i] = -1;
                    }
                    else
                    {
                        iConnectedNeighbours[i] = p_iOriginatorCell;
                    }
                }
                else 
                {
                    iVisitedNeighbours[i] = m_iMapCells[p_iCurrentCell][ i];
                    iConnectedNeighbours[i] = m_iMapCells[p_iCurrentCell][ i];
                    iNeighboursCount++;
                }
            }

            int iSelectedNeighbour;
            int iRandomSelected;
            
            // base case all neighbours visited
            while (iNeighboursCount > 0)
            {
                // reset selected neighbour
                iSelectedNeighbour = -1;

                
                if (iNeighboursCount == 1)// last neighbour
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (iVisitedNeighbours[i] != -1)
                        {
                            iSelectedNeighbour = iVisitedNeighbours[i];
                            iVisitedNeighbours[i] = -1;
                            iNeighboursCount--;
                            break;
                        }
                    }
                }
                else // randomly select neighbour
                {
                    iRandomSelected = m_Rand.Next(1, iNeighboursCount+1);

                    for (int i = 0; i < 4; i++)
                    {
                        if (iVisitedNeighbours[i] != -1 
                            && iRandomSelected == 1)
                        {
                            iSelectedNeighbour = iVisitedNeighbours[i];
                            iVisitedNeighbours[i] = -1;
                            iNeighboursCount--;
                            break;
                        }
                        else if (iVisitedNeighbours[i] != -1)
                        {
                            iRandomSelected--;
                        }
                    }
                }

                System.Diagnostics.Debug.Assert(iSelectedNeighbour > -1);
                // process the selected cell
                ProcessDepthFirstSearchCell(iSelectedNeighbour, p_iCurrentCell);

                if (iNeighboursCount != 0)
                {
                    // verify if other neighbours have been visted
                    for (int i = 0; i < 4; i++)
                    {
                        if (iVisitedNeighbours[i] != p_iOriginatorCell
                            && iVisitedNeighbours[i] != -1
                            && m_iVisitedCells[iVisitedNeighbours[i]] == -1)
                        {
                            iVisitedNeighbours[i] = -1;
                            iConnectedNeighbours[i] = -1;
                            iNeighboursCount--;
                        }
                    }
                }

            }//endwhile

            //exit from recursion.. copy map with barriers
            for (int i = 0; i < 4; i++)
            {
                m_iMapCells[p_iCurrentCell][ i] = iConnectedNeighbours[i];
            }
        }

        private void CreateDepthFirstSearchMaze()
        {
            ProcessDepthFirstSearchCell(0,0);
        }

        internal void Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            int iCurrentCell;

            for (int j = 0; j < m_iYCells; j++)
            
            {
                for (int i = 0; i < m_iXCells; i++)    
                {
                    iCurrentCell = (j * m_iYCells) + i;

                    e.Graphics.TranslateTransform(m_fBorder + (i * m_iCellWidth), m_fBorder + (j * m_iCellWidth));

                    if (m_iMapCells[iCurrentCell][0] == -1)
                    {
                        e.Graphics.DrawLine(Pens.Blue, 0, 0, 0, m_iCellWidth);
                    }

                    if (m_iMapCells[iCurrentCell][1] == -1)
                    {
                        e.Graphics.DrawLine(Pens.Blue, 0, 0, m_iCellWidth, 0);
                    }

                    if (m_iMapCells[iCurrentCell][2] == -1)
                    {
                        e.Graphics.DrawLine(Pens.Blue, m_iCellWidth,0, m_iCellWidth, m_iCellWidth);
                    }

                    if (m_iMapCells[iCurrentCell][3] == -1)
                    {
                        e.Graphics.DrawLine(Pens.Blue,  0,m_iCellWidth, m_iCellWidth, m_iCellWidth);
                    }
                    e.Graphics.ResetTransform();
                }
            }

            // paint start pos
            e.Graphics.TranslateTransform(m_fBorder + (m_StartPos.X * m_iCellWidth),
                m_fBorder + (m_StartPos.Y * m_iCellWidth));

            e.Graphics.FillRectangle(Brushes.Green, new Rectangle(1, 1, m_iCellWidth - 1, m_iCellWidth - 1));

            e.Graphics.ResetTransform();


            // paint end pos
            e.Graphics.TranslateTransform(m_fBorder + (m_EndPos.X * m_iCellWidth),
                m_fBorder + (m_EndPos.Y * m_iCellWidth));

            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(1, 1, m_iCellWidth - 1, m_iCellWidth - 1));

            e.Graphics.ResetTransform();

            
            // paint path

            Pen pen = new Pen(Brushes.Gold, 1/m_iCellWidth);

            e.Graphics.TranslateTransform(m_fBorder + (m_iCellWidth / 2), m_fBorder + (m_iCellWidth / 2));
            e.Graphics.ScaleTransform(m_iCellWidth, m_iCellWidth);

            for (int i = 1; i < m_Path.Count; i++)
            {
                e.Graphics.DrawLine(pen,m_Path[i-1],  m_Path[i]);
            }


        }

        internal int[] GetBoundaries(Point p_CurrentPoint)
        {
            return m_iMapCells[(m_iXCells * p_CurrentPoint.Y) + p_CurrentPoint.X];
        }

        internal bool IsExit(Point p_CurrentPoint)
        {
            return m_EndPos.Equals(p_CurrentPoint);
        }
    }
}
