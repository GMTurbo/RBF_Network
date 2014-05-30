using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RBF_Network
{
    public partial class Form1 : Form
    {
        List<List<Satellite>> satellites = new List<List<Satellite>>();
        Dictionary<int, Color> ColorLookup = new Dictionary<int, Color>();
        List<Cluster> parents = new List<Cluster>();
        List<Cluster> parentsPrev = new List<Cluster>();
        List<Point> allPoints = new List<Point>();

        Timer t = new Timer();

        public int ClusterRadius = 105;
        public int parentCount = 10;
        public int interval = (int)(1000.0 / 30.0);
        public int clusterCount = 30;
        public int satCount = 20;

        Icon target;
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            Bitmap bmp = new Bitmap(RBF_Network.Properties.Resources.target);
            target = Icon.FromHandle(bmp.GetHicon());
            t.Interval = interval;
            t.Tick += t_Tick;
            makeboard();
        }

        void t_Tick(object sender, EventArgs e)
        {
            satellites.Clear();
            for (int i = 0; i < parentCount; i++)
                satellites.Add(new List<Satellite>());

            Parallel.ForEach(allPoints, (pnt) =>
            {
                double min = double.MaxValue;
                int index = -1;
                for (int j = 0; j < parents.Count; j++)
                {
                    double distance = getDistance(parents[j].Location, pnt);
                    if (distance < min)
                    {
                        min = distance;
                        index = j;
                    }
                }
                //if (DEMO_MODE)
                lock (satellites) { satellites[index].Add(new Satellite(new Point(pnt.X, pnt.Y), parents[index].ID)); }

                //else if (activateField)
                //{
                //    if (min < parents[index].Radius)
                //        lock (sats) { sats[index].Add(new System.Drawing.Point(pnt.X, pnt.Y)); }
                //}
                //else
                //    lock (sats) { sats[index].Add(new System.Drawing.Point(pnt.X, pnt.Y)); }

            });

            #region SEQUENCIAL METHOD

            //double min = double.MaxValue;
            //int index = -1;
            //double distance = 0;

            //for (int i = 0; i < allPoints.Count; i++)
            //{
            //    min = double.MaxValue;
            //    index = -1;
            //    for (int j = 0; j < parents.Count; j++)
            //    {
            //        distance = getDistance(parents[j].Location, allPoints[i]);
            //        if (distance < min)
            //        {
            //            min = distance;
            //            index = j;
            //        }
            //    }
            //    if (min < parents[index].Radius)
            //        sats[index].Add(new System.Drawing.Point(allPoints[i].X, allPoints[i].Y));
            //}

            #endregion SEQUENCIAL METHOD

            for (int i = 0; i < parents.Count; i++)
            {
                System.Drawing.Point p = getAvgPoint(satellites[i]);
                if (p.X != -1000 && p.Y != -1000)
                    //if (parents[i].ID != parentGUID && !parents[i].Frozen)
                    parents[i].Location = p;
            }

            //List<Kluster> deleteMe = new List<Kluster>();
            //for (int i = 0; i < parents.Count; i++)
            //{
            //    for (int j = 0; j < parents.Count; j++)
            //    {
            //        if (i != j)
            //        {
            //            double distance = getDistance(parents[i].Location, parents[j].Location);
            //            if (distance < 50)
            //            {
            //                if (parents[j].ID != parentGUID && !deleteMe.Contains(parents[i]) && !deleteMe.Contains(parents[j]))
            //                {
            //                    parents[i].Radius += parents[j].Radius / 3.0;
            //                    parents[i].Mass *= 2;
            //                    deleteMe.Add(parents[j]);
            //                }
            //            }
            //        }
            //    }
            //}

            //deleteMe.ForEach(k => { RemoveParent(k.ID); });

            bool same = true;
            if (parentsPrev.Count > 0)
                for (int i = 0; i < parents.Count; i++) { same &= parents[i].Location == parentsPrev[i].Location; }
            else
                same = false;

            if (same)
            {
                
                //Status = "K-Means Finished";
                Converged = true;
                button1.Text = !Converged ? "Stop" : "Start";
                //t.Enabled = false;
            }
            else
                Converged = false;

            //mutex.ReleaseMutex();
            parentsPrev = new List<Cluster>(parents);
            Invalidate();
        }

        public double getDistance(System.Drawing.Point parent, System.Drawing.Point sat)
        {
            return Math.Sqrt(Math.Pow(sat.X - parent.X, 2) + Math.Pow(sat.Y - parent.Y, 2));
        }

        public System.Drawing.Point getAvgPoint(List<Satellite> Points)
        {
            if (Points.Count == 0)
                return new System.Drawing.Point(-1000, -1000);

            return new System.Drawing.Point((int)Points.Average(pnt => pnt.X), (int)Points.Average(pnt => pnt.Y));
        }

        Random rand = new Random();

        int GetAvailableGUID()
        {
            int GUID = rand.Next(0, 1000);
            while (ParentsContainsGUID(GUID))
                GUID = GetAvailableGUID();

            return GUID;
        }

        bool ParentsContainsGUID(int key)
        {
            Cluster k = parents.Find(p => { return p.ID == key; });
            return k != null;
        }

        double GetRandomPercentage()
        {
            return (double)(rand.Next(0, int.MaxValue) / (double)int.MaxValue);
        }

        Color GetRandomColor()
        {
            return Color.FromArgb(100 + (int)(GetRandomPercentage() * 155), (int)(GetRandomPercentage() * 255), (int)(GetRandomPercentage() * 255), (int)(GetRandomPercentage() * 255));
        }

        public void makeboard()
        {
            // assign parents some xy points
            // fill the satellites with xy points

            parents.Clear();
            satellites.Clear();
            parentsPrev.Clear();
            ColorLookup.Clear();

            for (int i = 0; i < clusterCount; i++)
                satellites.Add(new List<Satellite>());

            System.Drawing.Size size = this.Size;

            System.Drawing.Point random = new System.Drawing.Point(-1000, -1000);

            for (int i = 0; i < parentCount; i++)
            {
                random = new System.Drawing.Point(-1000, -1000);

                while (random.X > size.Width - 10 || random.X < 0 || random.Y > size.Height - 32 || random.Y < 0)
                    random = new System.Drawing.Point((int)(GetRandomPercentage() * Size.Width), (int)(GetRandomPercentage() * Size.Height));

                parents.Add(new Cluster(random, GetAvailableGUID()));
            }

            System.Drawing.Point RandomClusterPoint = new System.Drawing.Point(-1000, -1000);

            allPoints.Clear();

            for (int i = 0; i < satellites.Count; i++)
            {
                random = new System.Drawing.Point(-1000, -1000);

                while (random.X > size.Width - 10 || random.X < 0 || random.Y > size.Height - 32 || random.Y < 0)
                    random = new System.Drawing.Point((int)(GetRandomPercentage() * Size.Width), (int)(GetRandomPercentage() * Size.Height));
                for (int j = 0; j < satCount; j++)
                {
                    RandomClusterPoint = new System.Drawing.Point(-1000, -1000);
                    while (RandomClusterPoint.X > size.Width - 10 || RandomClusterPoint.X < 0 || RandomClusterPoint.Y > size.Height - 10 || RandomClusterPoint.Y < 0)
                        RandomClusterPoint = new System.Drawing.Point((int)(random.X + Math.Cos(2 * Math.PI * GetRandomPercentage()) * GetRandomPercentage() * ClusterRadius), (int)(random.Y + Math.Sin(2 * Math.PI * GetRandomPercentage()) * GetRandomPercentage() * ClusterRadius));
                    satellites[i].Add(new Satellite(RandomClusterPoint, -1));
                    if(allPoints.Find(pnt=>{return pnt.X == RandomClusterPoint.X && pnt.Y == RandomClusterPoint.Y;}) != null)
                        allPoints.Add(RandomClusterPoint);
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj = e.Graphics;

            graphicsObj.Clear(Color.Black);

            if (satellites.Count > 0)
            {
                Pen myPen = new Pen(System.Drawing.Color.Green, 5);
                for (int i = 0; i < satellites.Count; i++)
                {
                    if (i < parents.Count)
                    {
                        if (!ColorLookup.ContainsKey(parents[i].ID))
                            ColorLookup.Add(parents[i].ID, GetRandomColor());
                        myPen = new Pen(ColorLookup[parents[i].ID], 5);
                    }
                    else
                        myPen = new Pen(GetRandomColor(), 5);

                    if (satellites[i].Count > 0)
                    {
                        foreach (Satellite sat in satellites[i])
                        {
                            graphicsObj.DrawEllipse(myPen, new Rectangle(sat.X, sat.Y, 2, 2));
                        }
                    }
                }
            }

            for (int i = 0; i < parentCount; i++)
            {
                if (!ColorLookup.ContainsKey(parents[i].ID))
                    ColorLookup.Add(parents[i].ID, GetRandomColor());
                graphicsObj.DrawIcon(target, new Rectangle(parents[i].X - 25, parents[i].Y - 25, 50, 50));
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            makeboard();
        }

        bool Converged = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (!t.Enabled)
            {
                if (Converged)
                    makeboard();
                //FrameRater.Start();
                t.Start();
            }
            else
            {
                //FrameRater.Stop();
                //t.Stop();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            t.Enabled = false;

            RBFBframe rbfframe = new RBFBframe();
            rbfframe.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height);
            rbfframe.SetNetworkVariables(satellites, parents);
            rbfframe.Show();

            MultRBF rbfframe2 = new MultRBF();
            rbfframe2.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height);
            rbfframe2.SetNetworkVariables(satellites, parents);
            rbfframe2.Show();
        }

        

    }

    public class Satellite
    {
        Point position;
        int parentGUID = -1;

        public int ParentGUID
        {
            get { return parentGUID; }
            set { parentGUID = value; }
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public int X
        {
            get { return Position.X; }
            set { position.X = value; }
        }

        public int Y
        {
            get { return Position.Y; }
            set { position.Y = value; }
        }

        public Satellite(Point xy, int GUID)
        {
            position = new Point(xy.X, xy.Y);
            ParentGUID = GUID;
        }

    }

    public class Cluster
    {
        Point location;

        bool frozen = false;

        public bool Frozen
        {
            get { return frozen; }
            set { frozen = value; }
        }

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public int X
        {
            get { return location.X; }
            set { location.X = value; }
        }

        public int Y
        {
            get { return location.Y; }
            set { location.Y = value; }
        }

        int guid = -1;

        public int ID
        {
            get { return guid; }
            set { guid = value; }
        }

        public Cluster(Point pnt, int GUID)
        {
            location = new Point(pnt.X, pnt.Y);
            ID = GUID;
        }
    }
}
