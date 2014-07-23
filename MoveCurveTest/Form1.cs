using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MoveCurveTest
{
    public partial class frmMain : Form
    {
        private Point? moveFrom;
        private Point? moveTo;
        private Point? moveFinish;
        
        private Thread continueThread; 
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            moveFrom = new Point(10, 10);
            moveTo = new Point(80, 300);
            moveFinish = new Point(600, 10);
            
            continueThread = new Thread(ContinueThread);
            continueThread.IsBackground = true;
            continueThread.Start();
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle
            (
                Brushes.Blue,
                new Rectangle(moveFrom.Value, new Size(4, 4))
            );
            
            e.Graphics.FillRectangle
            (
                Brushes.Red,
                new Rectangle(moveTo.Value, new Size(4, 4))
            );
        }
        
        private void ContinueThread ()
        {
            Int32 beginTick = Environment.TickCount;
            Int32 retainTick = 500;
            
            Point moveDistance = new Point 
            (
                moveTo.Value.X - moveFrom.Value.X,
                moveTo.Value.Y - moveFrom.Value.Y
            );
            
            while (true)
            {
                Int32 deltaTick = Environment.TickCount - beginTick;
                 
                if (deltaTick > 2 * retainTick)
                {
                    break;;
                }
                
                Single ratio = Convert.ToSingle(deltaTick) / Convert.ToSingle(retainTick);
                
                PointF currentPosition;
                
                if (deltaTick < retainTick)
                {
                    currentPosition = new PointF
                    (
                        moveFrom.Value.X + moveDistance.X * ratio,
                        moveFrom.Value.Y + (2.0f - ratio) * ratio * moveDistance.Y
                    );
                }
                else
                {
                    currentPosition = new PointF
                    (
                        moveTo.Value.X + moveDistance.X * (ratio - 1.0f) * (Convert.ToSingle(moveFinish.Value.X) / Convert.ToSingle(moveTo.Value.X)),
                        moveFrom.Value.Y + (2.0f - ratio) * ratio * moveDistance.Y
                    );
                }
                
                InvokeUIThread
                (
                    () => 
                    {
                        this.Text = moveTo + " => " + currentPosition + " => " + moveFrom;
                        
                        using(Graphics g = this.CreateGraphics())
                        {
                            g.DrawRectangle
                            (
                                Pens.Green,
                                new Rectangle
                                (
                                    new Point 
                                    (
                                        Convert.ToInt32(currentPosition.X),
                                        Convert.ToInt32(currentPosition.Y)
                                    ),
                                    new Size(2, 2)
                                )
                            );
                        }
                    }
                );

                Thread.Sleep(1);
            }
        }
        
        private void InvokeUIThread(MethodInvoker action)
        {
            if (this.InvokeRequired == true)
            {
                this.Invoke(action);
            }
            else
            {
                action.DynamicInvoke();
            }
        }
    }
}
