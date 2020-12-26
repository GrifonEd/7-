using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Даукаев_Лаба7ООП
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        Color current_color = Color.DarkBlue;
        Color basic_color = Color.DarkBlue;
        Color selected_color = Color.Red;
        static int kolvo_elem = 0;
        static int sizeStorage = 1;
        static int index_sozdania = 0;
        Graphics graphics;
        Storage storage = new Storage(1);
        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(picture.Width, picture.Height);
            graphics = Graphics.FromImage(bitmap);
            picture.Image = bitmap;

        }
        public class Observer
        {
            public virtual void SubjectChanged() { return; }
        }
      /*  class Tree : Observer
        {
            private Storage objects1;
            private TreeView tree;

            public Tree(Storage objects1, TreeView tree)
            {
                objects1 = objects1;
                this.tree = tree;
            }

            public void Print()
            {
                tree.Nodes.Clear();
                if (storage.currentSize > 1)
                {
                    int SelectedIndex = 0;
                    TreeNode start = new TreeNode("Фигуры");

                    for (int i = 0; i < storage.currentSize; i++)
                    {
                        if (objects1.objects[i] != null) SelectedIndex = i;
                        PrintNode(start, objects1.objects[i]);
                    }
                    tree.Nodes.Add(start);

                    for (int i = 0; i < storage.currentSize; i++)
                    {

                        tree.SelectedNode = tree.Nodes[0].Nodes[i];
                        if (objects1.objects[i].choose == true)
                            tree.SelectedNode.ForeColor = Color.Red;
                        else tree.SelectedNode.ForeColor = Color.Black;
                    }
                }
                tree.ExpandAll();

            }

            private void PrintNode(TreeNode node, Shape shape)
            {
                if (shape is Sgroup)
                {
                    TreeNode tn = new TreeNode(shape.x.ToString());
                    if (objects1.currentSize != 1)
                    {
                        for (int i = 0; i < objects1.currentSize; i++)
                            PrintNode(tn, ((Sgroup)shape).objects2.objects[i]);
                    }
                    node.Nodes.Add(tn);
                }
                else
                {

                    node.Nodes.Add((shape.x.ToString()));
                }
            }

            public override void SubjectChanged()
            {
                Print();
            }
        }*/
        public class ObjObserved
        {
            public Storage storage;
            public void AddStorage(Storage sto)
            {
                storage = sto;
            }
        }

        public class Observed
        {
            private List<Observer> observers;
            public Observed()
            {
                observers = new List<Observer>();
            }
            public void AddObserver(Observer o)
            {
                observers.Add(o);
            }
            public void Notify()
            {
                foreach (Observer observer in observers) observer.SubjectChanged();
            }
        }

        abstract public class Shape
        {
            public Rectangle rect;
            public PointF[] polygonPoints = new PointF[3];
            protected string ClassName;
            public int R;
            public Color color;
            public int x, y;
            public bool choose = false;
            public bool narisovana = true;

            abstract public string Class_Name();
            abstract public void Move(Storage storage, int dx, int dy);
            abstract public void New_Color();
            abstract public void Resize(Storage storage, int dx);
            abstract public void Save(StreamWriter stream);
            abstract public void Load(StreamReader stream);
            // abstract public string Show();
            abstract public void Draw(Pen pen, Graphics graphics);




        }
        public class Square : Shape
        {

            public Square()
            {
                ClassName = "Square";
                x = 0;
                y = 0;
                R = 0;

            }
            public Square(int x, int y, Color color, int Rad)
            {
                ClassName = "Square";
                this.x = x - Rad;
                this.y = y - Rad;
                R = Rad;
                this.color = color;
            }
            ~Square()
            {
            }
            public override string Class_Name()
            {
                return ClassName;
            }
            public override void Move(Storage storage, int dx, int dy)
            {
                for (int i = 0; i < storage.currentSize; i++)

                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            if (storage.objects[i] is Sgroup)
                            {
                                Sgroup Hi = (Sgroup)storage.objects[i];
                                for (int j = 0; j < Hi.objects2.currentSize - 1; j++)
                                {
                                    Hi.objects2.objects[j].y += dy;
                                    Hi.objects2.objects[j].x += dx;
                                }
                                storage.objects[i] = Hi;
                            }

                            storage.objects[i].rect = new Rectangle(storage.objects[i].x + dx, storage.objects[i].y + dy, storage.objects[i].rect.Width, storage.objects[i].rect.Height);
                            storage.objects[i].y += dy;
                            storage.objects[i].x += dx;

                        }
            }
            public override void New_Color()
            {
                ;
            }
            public override void Resize(Storage storage, int dx)
            {
                for (int i = 0; i < storage.currentSize; i++)
                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            storage.objects[i].y -= dx;
                            storage.objects[i].x -= dx;
                            storage.objects[i].R += dx;
                        }
            }
            public override void Save(StreamWriter stream)
            {
                stream.WriteLine("Square");
                stream.WriteLine((x + R) + " " + (y + R) + " " + R + " " + color.R + " " + color.G + " " + color.B);
            }
            public override void Load(StreamReader stream)
            {
                string[] data = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                x = Convert.ToInt32(data[0]);
                y = Convert.ToInt32(data[1]);
                R = Convert.ToInt32(data[2]);
                color = Color.FromArgb(Convert.ToInt32(data[3]), Convert.ToInt32(data[4]), Convert.ToInt32(data[5]));
                //Resize();
            }
            /*  public override string Show()
         {
             ;
         }*/
            public override void Draw(Pen pen, Graphics graphics)
            {
                graphics.DrawRectangle(pen, x, y, R * 2, R * 2);
            }
        }

        public class Triangle : Shape
        {


            public Triangle()
            {
                ClassName = "Triangle";
                x = 0;
                y = 0;
                R = 0;
            }

            public Triangle(int x, int y, Color color, int Rad)
            {
                ClassName = "Triangle";
                this.x = x;
                this.y = y;
                this.R = Rad;
                this.color = color;
                polygonPoints[0] = new PointF(this.x, (int)(this.y - 2 * R * Math.Sqrt(3) / 3));
                polygonPoints[1] = new PointF(this.x - R, (int)(this.y + R * Math.Sqrt(3) / 3));
                polygonPoints[2] = new PointF(this.x + R, (int)(this.y + R * Math.Sqrt(3) / 3));

            }
            public override string Class_Name()
            {
                return ClassName;
            }
            public override void Move(Storage storage, int dx, int dy)
            {
                for (int i = 0; i < storage.currentSize; i++)

                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            if (storage.objects[i] is Sgroup)
                            {
                                Sgroup Hi = (Sgroup)storage.objects[i];
                                for (int j = 0; j < Hi.objects2.currentSize - 1; j++)
                                {
                                    Hi.objects2.objects[j].y += dy;
                                    Hi.objects2.objects[j].x += dx;
                                }
                                storage.objects[i] = Hi;
                            }

                            storage.objects[i].rect = new Rectangle(storage.objects[i].x + dx, storage.objects[i].y + dy, storage.objects[i].rect.Width, storage.objects[i].rect.Height);
                            storage.objects[i].y += dy;
                            storage.objects[i].x += dx;

                        }
            }
            public override void New_Color()
            {
                ;
            }
            public override void Resize(Storage storage, int dx)
            {
                for (int i = 0; i < storage.currentSize; i++)
                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            storage.objects[i].y -= dx;
                            storage.objects[i].x -= dx;
                            storage.objects[i].R += dx;
                        }
            }
            public override void Save(StreamWriter stream)
            {
                stream.WriteLine("Triangle");
                stream.WriteLine((x + R) + " " + (y + R) + " " + R + " " + color.R + " " + color.G + " " + color.B);
            }
            public override void Load(StreamReader stream)
            {
                string[] data = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                x = Convert.ToInt32(data[0]);
                y = Convert.ToInt32(data[1]);
                R = Convert.ToInt32(data[2]);
                color = Color.FromArgb(Convert.ToInt32(data[3]), Convert.ToInt32(data[4]), Convert.ToInt32(data[5]));
            }
            /*  public override string Show()
         {
             ;
         }*/
            public override void Draw(Pen pen, Graphics graphics)
            {
                ;
            }
            ~Triangle()
            {
            }

        }
        public class CCircle : Shape
        {

            public CCircle()
            {
                ClassName = "CCircle";
                x = 0;
                y = 0;
                R = 0;
            }
            public CCircle(int x, int y, Color color, int Rad)
            {
                ClassName = "CCircle";
                this.x = x - Rad;
                this.y = y - Rad;
                this.R = Rad;
                this.color = color;
            }
            public override string Class_Name()
            {
                return ClassName;
            }
            public override void Move(Storage storage, int dx, int dy)
            {
                for (int i = 0; i < storage.currentSize; i++)

                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            if (storage.objects[i] is Sgroup) 
                            {
                                Sgroup Hi = (Sgroup)storage.objects[i];
                                for (int j = 0; j < Hi.objects2.currentSize - 1; j++)
                                {
                                    Hi.objects2.objects[j].y += dy;
                                    Hi.objects2.objects[j].x += dx;
                                }
                                storage.objects[i] = Hi;
                            }
                            
                            storage.objects[i].rect = new Rectangle(storage.objects[i].x + dx, storage.objects[i].y + dy, storage.objects[i].rect.Width, storage.objects[i].rect.Height);
                            storage.objects[i].y += dy;
                            storage.objects[i].x += dx;

                        }
            }
            public override void New_Color()
            {
                ;
            }
            public override void Resize(Storage storage, int dx)
            {
                for (int i = 0; i < storage.currentSize; i++)
                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            storage.objects[i].y -= dx;
                            storage.objects[i].x -= dx;
                            storage.objects[i].R += dx;
                        }
            }
            public override void Save(StreamWriter stream)
            {
                stream.WriteLine("CCircle");
                stream.WriteLine((x + R) + " " + (y + R) + " " + R + " " + color.R + " " + color.G + " " + color.B);
            }
            public override void Load(StreamReader stream)
            {
                string[] data = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                x = Convert.ToInt32(data[0]);
                y = Convert.ToInt32(data[1]);
                R = Convert.ToInt32(data[2]);
                color = Color.FromArgb(Convert.ToInt32(data[3]), Convert.ToInt32(data[4]), Convert.ToInt32(data[5]));
            }
            /*  public override string Show()
         {
             ;
         }*/
            public override void Draw(Pen pen, Graphics graphics)
            {
                graphics.DrawEllipse(pen, x, y, R * 2, R * 2);
            }

            ~CCircle()
            {
            }
        }
        public class Sgroup : Shape
        {
            int width, height;
            public Storage objects2;
            int newsize = 1;
            public Sgroup()
            {
                objects2 = new Storage(1);
                ClassName = "Sgroup";

            }
            public Sgroup(int Width, int Height)
            {
                objects2 = new Storage(1);
                ClassName = "Sgroup";
                width = Width;
                height = Height;
            }
            ~Sgroup()
            {
            }
            public void Add(Shape s)
            {


                objects2.add_object(ref objects2.currentSize, s, ref objects2.currentchislo, ref objects2.currentpozicion);
                //  sto.get().sticky = false;
                if (objects2.currentSize == 2)
                {
                    rect = new Rectangle(s.x, s.y, 2 * s.R, 2 * s.R);
                    // Point newPoint = new Point(s.x, s.y);
                    //rect.Location = newPoint;
                }
                else if ((s.Class_Name()) != "Sgroup")
                {
                    if (s.x < rect.Left)
                    {
                        int tmp = rect.Right;
                        rect.X = s.x;
                        x = s.x;
                        rect.Width = tmp - rect.X;

                    }
                    if (s.x + (s.R * 2) > rect.Right)
                    {
                        rect.Width = s.x + s.R * 2 - rect.X;
                    }

                    if (s.y < rect.Top)
                    {
                        int tmp = rect.Bottom;
                        rect.Y = s.y;
                        y = s.y;
                        rect.Height = tmp - rect.Y;
                    }
                    if (s.y + s.R * 2 > rect.Bottom)
                        rect.Height = s.y + s.R * 2 - rect.Y;
                }
                else
                {
                    if (s.rect.X < rect.Left)
                    {
                        int tmp = rect.Right;
                        rect.X = s.rect.X;
                        rect.Width = tmp - rect.X;

                    }
                    if (s.rect.X + (s.rect.Width) > rect.Right)
                    {
                        rect.Width = s.rect.X + s.rect.Width - rect.X;
                    }

                    if (s.rect.Y < rect.Top)
                    {
                        int tmp = rect.Bottom;
                        rect.Y = s.rect.Y;
                        rect.Height = tmp - rect.Y;
                    }
                    if (s.rect.Y + s.rect.Height > rect.Bottom)
                        rect.Height = s.rect.Y + s.rect.Height - rect.Y;
                }
            }
            public override string Class_Name()
            {
                return ClassName;
            }
            public override void Move(Storage storage, int dx, int dy)
            {

                for (int i = 0; i < storage.currentSize; i++)

                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            Sgroup Hi = (Sgroup)storage.objects[i];
                            for (int j = 0; j < Hi.objects2.currentSize - 1; j++)
                            {
                                Hi.objects2.objects[j].y += dy;
                                Hi.objects2.objects[j].x += dx;
                            }
                            storage.objects[i] = Hi;
                            storage.objects[i].rect = new Rectangle(storage.objects[i].x + dx, storage.objects[i].y + dy, storage.objects[i].rect.Width, storage.objects[i].rect.Height);
                            storage.objects[i].y += dy;
                            storage.objects[i].x += dx;

                        }
            }
            public override void New_Color()
            {
                ;
            }
            public override void Resize(Storage storage, int dx)
            {
                for (int i = 0; i < storage.currentSize; i++)
                    if (!storage.proverka(i))
                        if (storage.objects[i].choose == true)
                        {
                            storage.objects[i].y -= dx;
                            storage.objects[i].x -= dx;
                            storage.objects[i].R += dx;
                        }
            }
            public override void Save(StreamWriter stream)
            {
                stream.WriteLine("Sgroup");
                stream.WriteLine(objects2.currentSize + " " + width + " " + height);
                if (objects2.currentSize != 1)
                {

                    for (int i = 0; i < objects2.currentSize - 1; i++)
                        objects2.objects[i].Save(stream);
                }
            }
            public override void Load(StreamReader stream)
            {
                ShapeFactory factory = new ShapeFactory();
                string[] data = stream.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int i = Convert.ToInt32(data[0]) - 1;
                width = Convert.ToInt32(data[1]);
                height = Convert.ToInt32(data[2]);
                for (; i > 0; i--)
                {
                    Shape tmp = factory.createShape(stream.ReadLine());
                    tmp.Load(stream);
                    Add(tmp);
                }
                //Resize();
            }
            /*  public override string Show()
         {
             ;
         }*/
            public override void Draw(Pen pen, Graphics graphics)
            {
                ;
            }
        }
        private int Check_In(ref Storage storage, int size, int x, int y, int R)
        {
            if (storage.kolvo_zanyatix(size) != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    if (!storage.proverka(i))
                    {
                        if (storage.objects[i].Class_Name() == "CCircle")
                        {

                            if (((x + R) - (storage.objects[i].x + storage.objects[i].R)) * ((x + R) - (storage.objects[i].x + storage.objects[i].R)) + ((y + R) - (storage.objects[i].y + storage.objects[i].R)) * ((y + R) - (storage.objects[i].y + storage.objects[i].R)) <= storage.objects[i].R * storage.objects[i].R)
                                return i;
                            if (storage.objects[i].Class_Name() == "Triangle")
                            {
                                if (((x - R) - (storage.objects[i].x + storage.objects[i].R)) * ((x - R) - (storage.objects[i].x + storage.objects[i].R)) + ((y - R) - (storage.objects[i].y + storage.objects[i].R)) * ((y - R) - (storage.objects[i].y + storage.objects[i].R)) <= storage.objects[i].R * storage.objects[i].R)
                                    return i;
                            }
                            if (storage.objects[i].Class_Name() == "Sgroup")
                            {
                                if (x < storage.objects[i].rect.Right && x > storage.objects[i].rect.Left && y < storage.objects[i].rect.Bottom && y > storage.objects[i].rect.Top)
                                    return i;
                            }
                        }
                        else if (storage.objects[i].Class_Name() == "Square")
                        {
                            if (Math.Abs((storage.objects[i].x + storage.objects[i].R) - (x + R)) < storage.objects[i].R && Math.Abs((storage.objects[i].y + storage.objects[i].R) - (y + R)) < storage.objects[i].R)
                                return i;
                        }
                        else if (storage.objects[i].Class_Name() == "Triangle")
                        {


                            if ((((x < storage.objects[i].polygonPoints[0].X) && (x > storage.objects[i].polygonPoints[1].X) && ((y > storage.objects[i].polygonPoints[0].Y) && (y < storage.objects[i].polygonPoints[1].Y))) || (((x < storage.objects[i].polygonPoints[2].X) && (x > storage.objects[i].polygonPoints[0].X)) && ((y < storage.objects[i].polygonPoints[2].Y) && (y > storage.objects[i].polygonPoints[0].Y)))))
                                return i;

                        }
                        if (storage.objects[i].Class_Name() == "Sgroup")
                        {
                            if (x + R < storage.objects[i].rect.Right && x + R > storage.objects[i].rect.Left && y + R < storage.objects[i].rect.Bottom && y + R > storage.objects[i].rect.Top)
                                return i;
                        }
                    }
                }

            }
            return -1;
        }
        private void MyPaint(int kolvo_elem, ref Storage storage)
        {
            if (storage.objects[kolvo_elem] != null)
            {

                //Доработать if(storage.objects[kolvo_elem].x < 0 && storage.objects[kolvo_elem].x + 2 * storage.objects[kolvo_elem].R > 711)
                if (storage.objects[kolvo_elem].Class_Name() != "Triangle")
                {
                    if (storage.objects[kolvo_elem].x + 2 * storage.objects[kolvo_elem].R > 711)
                        storage.objects[kolvo_elem].x = 711 - 2 * storage.objects[kolvo_elem].R;
                    if (storage.objects[kolvo_elem].x < 0)
                        storage.objects[kolvo_elem].x = 0;
                    if (storage.objects[kolvo_elem].x < 0 || storage.objects[kolvo_elem].x + 2 * storage.objects[kolvo_elem].R > 711)
                    {
                        storage.objects[kolvo_elem].R = 711 / 2;


                    }
                    if (storage.objects[kolvo_elem].y + 2 * storage.objects[kolvo_elem].R > 420)
                        storage.objects[kolvo_elem].y = 420 - 2 * storage.objects[kolvo_elem].R;
                    if (storage.objects[kolvo_elem].y < 0)
                        storage.objects[kolvo_elem].y = 0;
                    if (storage.objects[kolvo_elem].y + 2 * storage.objects[kolvo_elem].R > 420 || storage.objects[kolvo_elem].y < 0)
                    {
                        storage.objects[kolvo_elem].x += 1;
                        storage.objects[kolvo_elem].R = 420 / 2;
                    }
                }
                if (storage.objects[kolvo_elem].Class_Name() == "Triangle")
                {
                    if ((int)(storage.objects[kolvo_elem].y - 2 * storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3) < 0)
                    {
                        storage.objects[kolvo_elem].y = (int)(2 * storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3);
                        storage.objects[kolvo_elem].polygonPoints[0] = new PointF(storage.objects[kolvo_elem].x, (int)(storage.objects[kolvo_elem].y - 2 * storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3));
                        storage.objects[kolvo_elem].polygonPoints[1] = new PointF(storage.objects[kolvo_elem].x - storage.objects[kolvo_elem].R, (int)(storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3));
                        storage.objects[kolvo_elem].polygonPoints[2] = new PointF(storage.objects[kolvo_elem].x + storage.objects[kolvo_elem].R, (int)(storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3));
                    }
                    if ((int)(storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3) > 420)
                    {
                        storage.objects[kolvo_elem].y = 420 - (int)(storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3);
                        storage.objects[kolvo_elem].polygonPoints[0] = new PointF(storage.objects[kolvo_elem].x, (int)(storage.objects[kolvo_elem].y - 2 * storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3));
                        storage.objects[kolvo_elem].polygonPoints[1] = new PointF(storage.objects[kolvo_elem].x - storage.objects[kolvo_elem].R, (int)(storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3));
                        storage.objects[kolvo_elem].polygonPoints[2] = new PointF(storage.objects[kolvo_elem].x + storage.objects[kolvo_elem].R, (int)(storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R * Math.Sqrt(3) / 3));
                    }


                }

                Pen pen = new Pen(storage.objects[kolvo_elem].color, 4);

                Pen pen1 = new Pen(current_color, 10);
                if (storage.objects[kolvo_elem].Class_Name() == "CCircle")
                {
                    if (storage.objects[kolvo_elem].choose == true)
                    {
                        graphics.DrawEllipse(pen1, storage.objects[kolvo_elem].x + storage.objects[kolvo_elem].R, storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R, 1, 1);
                        graphics.DrawEllipse(pen1, storage.objects[kolvo_elem].x, storage.objects[kolvo_elem].y, storage.objects[kolvo_elem].R * 2, storage.objects[kolvo_elem].R * 2);
                        graphics.DrawEllipse(pen, storage.objects[kolvo_elem].x, storage.objects[kolvo_elem].y, storage.objects[kolvo_elem].R * 2, storage.objects[kolvo_elem].R * 2);
                        if (storage.objects[kolvo_elem].choose == true) ;


                    }
                    else
                        graphics.DrawEllipse(pen, storage.objects[kolvo_elem].x, storage.objects[kolvo_elem].y, storage.objects[kolvo_elem].R * 2, storage.objects[kolvo_elem].R * 2);
                }
                else if (storage.objects[kolvo_elem].Class_Name() == "Square")
                {
                    if (storage.objects[kolvo_elem].choose == true)
                    {
                        graphics.DrawEllipse(pen1, storage.objects[kolvo_elem].x + storage.objects[kolvo_elem].R, storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R, 1, 1);
                        graphics.DrawRectangle(pen1, storage.objects[kolvo_elem].x, storage.objects[kolvo_elem].y, storage.objects[kolvo_elem].R * 2, storage.objects[kolvo_elem].R * 2);
                        if (storage.objects[kolvo_elem].choose == true)
                            graphics.DrawRectangle(pen, storage.objects[kolvo_elem].x, storage.objects[kolvo_elem].y, storage.objects[kolvo_elem].R * 2, storage.objects[kolvo_elem].R * 2);

                    }
                    else
                        graphics.DrawRectangle(pen1, storage.objects[kolvo_elem].x, storage.objects[kolvo_elem].y, storage.objects[kolvo_elem].R * 2, storage.objects[kolvo_elem].R * 2);
                }
                else if (storage.objects[kolvo_elem].Class_Name() == "Triangle")
                {
                    Triangle Hi = (Triangle)storage.objects[kolvo_elem];
                    if (storage.objects[kolvo_elem].choose == true)
                    {
                        graphics.DrawEllipse(pen1, storage.objects[kolvo_elem].x + storage.objects[kolvo_elem].R, storage.objects[kolvo_elem].y + storage.objects[kolvo_elem].R, 1, 1);
                        graphics.DrawPolygon(pen1, Hi.polygonPoints); if (storage.objects[kolvo_elem].choose == true)
                            graphics.DrawPolygon(pen, Hi.polygonPoints);
                    }
                    else
                        graphics.DrawPolygon(pen, Hi.polygonPoints);

                }
                if (storage.objects[kolvo_elem].Class_Name() == "Sgroup")
                {
                    Sgroup Hi = (Sgroup)storage.objects[kolvo_elem];
                    if (storage.objects[kolvo_elem].choose == true)
                    {
                        graphics.DrawRectangle(pen1, storage.objects[kolvo_elem].rect);
                    }
                    for (int i = 0; i < Hi.objects2.currentSize - 1; i++)
                        Hi.objects2.objects[i].Draw(pen, graphics);
                    graphics.DrawRectangle(pen, storage.objects[kolvo_elem].rect);

                }
            }
            picture.Image = bitmap;
        }
        private void Remove_Selection(ref Storage storage)
        {
            for (int i = 0; i < storage.currentSize; ++i)
                if (!storage.proverka(i))
                {
                    if (storage.objects[i].choose == true)
                    {
                        storage.objects[i].choose = false;
                        storage.objects[i].color = current_color;
                    }
                    if (storage.objects[i].narisovana == true)
                        MyPaint(i, ref storage);
                }
        }
        private void picture_MouseClick(object sender, MouseEventArgs e)
        {
            Shape krug = new CCircle(e.X, e.Y, current_color, 60);
            if (кругToolStripMenuItem.Checked)
            {
                krug = new CCircle(e.X, e.Y, current_color, 60);
            }
            else if (квадратToolStripMenuItem.Checked)
            {
                krug = new Square(e.X, e.Y, current_color, 60);
            }
            else if (треугольникToolStripMenuItem.Checked)
            {
                krug = new Triangle(e.X, e.Y, current_color, 60);
            }
            {
                if (Check_In(ref storage, storage.currentSize, krug.x, krug.y, krug.R) != -1)
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        int x = e.X - krug.R;
                        int y = e.Y - krug.R;

                        for (int i = 0; i < storage.currentSize; i++)
                            if (!storage.proverka(i))
                            {
                                if (storage.objects[i].choose == true)
                                {
                                    storage.objects[i].color = selected_color;
                                    storage.objects[i].choose = true;
                                    MyPaint(i, ref storage);
                                }
                                if (storage.objects[i].Class_Name() == "CCircle")
                                {
                                    if (((x + krug.R) - (storage.objects[i].x + storage.objects[i].R)) * ((x + krug.R) - (storage.objects[i].x + storage.objects[i].R)) + ((y + krug.R) - (storage.objects[i].y + storage.objects[i].R)) * ((y + krug.R) - (storage.objects[i].y + storage.objects[i].R)) <= storage.objects[i].R * storage.objects[i].R)
                                    {
                                        storage.objects[i].color = selected_color;
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);

                                    }
                                }
                                else if (storage.objects[i].Class_Name() == "Square")
                                {
                                    if (Math.Abs((storage.objects[i].x + storage.objects[i].R) - (x + krug.R)) < storage.objects[i].R && Math.Abs((storage.objects[i].y + storage.objects[i].R) - (y + krug.R)) < storage.objects[i].R)
                                    {
                                        storage.objects[i].color = selected_color;
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);

                                    }
                                }
                                else if (storage.objects[i].Class_Name() == "Triangle")
                                {
                                    if ((((x < storage.objects[i].polygonPoints[0].X) && (x > storage.objects[i].polygonPoints[1].X) && ((y > storage.objects[i].polygonPoints[0].Y) && (y < storage.objects[i].polygonPoints[1].Y))) || (((x < storage.objects[i].polygonPoints[2].X) && (x > storage.objects[i].polygonPoints[0].X)) && ((y < storage.objects[i].polygonPoints[2].Y) && (y > storage.objects[i].polygonPoints[0].Y)))))
                                    {
                                        storage.objects[i].color = selected_color;
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);

                                    }
                                }
                                if (storage.objects[i].Class_Name() == "Sgroup")
                                {
                                    if (x + storage.objects[i].R < storage.objects[i].rect.Right && x + storage.objects[i].R > storage.objects[i].rect.Left && y + storage.objects[i].R < storage.objects[i].rect.Bottom && y + storage.objects[i].R > storage.objects[i].rect.Top)
                                    {
                                        storage.objects[i].color = selected_color;
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);
                                    }
                                }
                            }
                    }
                    else
                    {
                        int x = e.X - krug.R;
                        int y = e.Y - krug.R;
                        if (krug.Class_Name() == "Triangle")
                        {
                            x = e.X;
                            y = e.Y;
                        }
                        Remove_Selection(ref storage);
                        for (int i = 0; i < storage.currentSize; i++)
                            if (!storage.proverka(i))
                            {
                                if (storage.objects[i].Class_Name() == "CCircle")
                                {
                                    if ((x - storage.objects[i].x) * (x - storage.objects[i].x) + (y - storage.objects[i].y) * (y - storage.objects[i].y) <= storage.objects[i].R * storage.objects[i].R)
                                    {
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);
                                        break;
                                    }
                                }
                                else if (storage.objects[i].Class_Name() == "Square")
                                {
                                    if (Math.Abs(storage.objects[i].x - x) < storage.objects[i].R && Math.Abs(storage.objects[i].y - y) < storage.objects[i].R)
                                    {
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);
                                        break;
                                    }
                                }
                                else if (storage.objects[i].Class_Name() == "Triangle")
                                {
                                    if ((((x < storage.objects[i].polygonPoints[0].X) && (x > storage.objects[i].polygonPoints[1].X) && ((y > storage.objects[i].polygonPoints[0].Y) && (y < storage.objects[i].polygonPoints[1].Y))) || (((x < storage.objects[i].polygonPoints[2].X) && (x > storage.objects[i].polygonPoints[0].X)) && ((y < storage.objects[i].polygonPoints[2].Y) && (y > storage.objects[i].polygonPoints[0].Y)))))
                                    {
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);
                                        break;
                                    }
                                }
                                else if (storage.objects[i].Class_Name() == "Sgroup")
                                {
                                    if (x + storage.objects[i].R < storage.objects[i].rect.Right && x + storage.objects[i].R > storage.objects[i].rect.Left && y + storage.objects[i].R < storage.objects[i].rect.Bottom && y + storage.objects[i].R > storage.objects[i].rect.Top)
                                    {
                                        storage.objects[i].choose = true;
                                        MyPaint(i, ref storage);
                                        break;
                                    }
                                }
                            }
                        storage.objects[Check_In(ref storage, storage.currentSize, krug.x, krug.y, krug.R)].color = selected_color;
                        storage.objects[Check_In(ref storage, storage.currentSize, krug.x, krug.y, krug.R)].choose = true;
                        MyPaint(Check_In(ref storage, storage.currentSize, krug.x, krug.y, krug.R), ref storage);
                        button3_Click(sender, e);
                    }
                    return;
                }
                storage.add_object(ref storage.currentSize, krug, ref kolvo_elem, ref index_sozdania);
                Remove_Selection(ref storage);
                storage.objects[index_sozdania].color = selected_color;
                storage.objects[index_sozdania].choose = true;
                MyPaint(index_sozdania, ref storage);

                button3_Click(sender, e);
            }


        }
        public abstract class Factory
        {
            public abstract Shape createShape(string name);
        }
        public class ShapeFactory : Factory
        {
            public override Shape createShape(string name)
            {
                Shape shape;
                switch (name)
                {
                    case "CCircle":
                        shape = new CCircle();
                        break;
                    case "Square":
                        shape = new Square();
                        break;
                    case "Triangle":
                        shape = new Triangle();
                        break;
                    case "Sgroup":
                        shape = new Sgroup();
                        break;
                    default:
                        shape = null;
                        break;
                }
                return shape;
            }
        }
        public class Storage
        {

            public Shape[] objects;
            public int currentSize = 1;
            public int currentchislo;
            public int currentpozicion;
            public Storage(int size)
            {
                objects = new Shape[size];
                for (int i = 0; i < size; i++)
                    objects[i] = null;
            }
            public void add_object(ref int size, Shape new_object, ref int ind, ref int index_sozdania)
            {
                Storage storage1 = new Storage(size + 1);
                for (int i = 0; i < size; i++)
                    storage1.objects[i] = objects[i];
                size = size + 1;
                videlenie(size);
                for (int i = 0; i < size; i++)
                    objects[i] = storage1.objects[i];
                objects[ind] = new_object;
                index_sozdania = ind;
                ind++;
            }
            public int getsize()
            {
                return currentSize;
            }
            public Shape GetObject(int index)
            {
                return objects[index];
            }
            public void videlenie(int size)
            {
                objects = new Shape[size];
                for (int i = 0; i < size; i++)
                    objects[i] = null;
            }
            public int kolvo_zanyatix(int size)
            {
                int count_zanyatih = 0;
                for (int i = 0; i < size; i++)
                {
                    if (!proverka(i))
                        count_zanyatih++;
                }
                return count_zanyatih;
            }
            public bool proverka(int kolvo_elem)
            {
                if (objects[kolvo_elem] == null)
                    return true;
                else return false;
            }
            public void Delte_obj(ref int kolvo_elem)
            {
                if (objects[kolvo_elem] != null)
                {
                    objects[kolvo_elem] = null;
                    kolvo_elem--;
                }
            }
            public void Delte_obj1(int pozicia)
            {
                if (objects[pozicia] != null)
                {
                    objects[pozicia] = null;
                    for (int i = pozicia; i < currentSize - 1; i++)
                    {
                        objects[i] = objects[i + 1];
                    }
                    currentSize--;

                }

            }

        }



        private void button2_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            for (int i = 0; i < storage.currentSize; i++)
            {
                if (!storage.proverka(i))
                {
                    storage.objects[i].narisovana = false;
                }
            }
            picture.Image = bitmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            if (storage.kolvo_zanyatix(storage.currentSize) != 0)
                for (int i = 0; i < storage.currentSize; ++i)
                {
                    MyPaint(i, ref storage); // Рисует круг
                    if (!storage.proverka(i))
                        storage.objects[i].narisovana = true; // Для элемента ставим флаг - нарисован
                }
            picture.Image = bitmap;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < storage.currentSize; i++)
            {
                storage.Delte_obj(ref i);
            }
            storage.currentSize = 1;
            storage = new Storage(storage.currentSize);
            kolvo_elem = 0;
            index_sozdania = 0;
        }

        private void ButtonDelThis_Click(object sender, EventArgs e)
        {
            if (storage.kolvo_zanyatix(storage.currentSize) != 0)
                for (int i = 0; i < storage.currentSize; i++)
                    if (storage.proverka(i) == false && (storage.objects[i].choose == true))
                        storage.Delte_obj1(i);

            button2_Click(sender, e);
            button3_Click(sender, e);
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {

        }

        private void picture_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = (e.X).ToString();
            label2.Text = (e.Y).ToString();
        }





        private void кругToolStripMenuItem_Click(object sender, EventArgs e)
        {
            кругToolStripMenuItem.Checked = true;
            квадратToolStripMenuItem.Checked = false;
            треугольникToolStripMenuItem.Checked = false;
        }

        private void квадратToolStripMenuItem_Click(object sender, EventArgs e)
        {
            кругToolStripMenuItem.Checked = false;
            квадратToolStripMenuItem.Checked = true;
            треугольникToolStripMenuItem.Checked = false;
        }

        private void треугольникToolStripMenuItem_Click(object sender, EventArgs e)
        {
            кругToolStripMenuItem.Checked = false;
            квадратToolStripMenuItem.Checked = false;
            треугольникToolStripMenuItem.Checked = true;
        }




        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                if (e.KeyChar == (char)Keys.S)
                {
                    storage.objects[storage.currentSize - 2].Move(storage, 0, 3);
                }
                else if (e.KeyChar == (char)Keys.A)
                {
                    storage.objects[storage.currentSize - 2].Move(storage, -3, 0);
                }
                else if (e.KeyChar == (char)Keys.D)
                {
                    storage.objects[storage.currentSize - 2].Move(storage, 3, 0);
                }
                else if (e.KeyChar == (char)Keys.W)
                {
                    storage.objects[storage.currentSize - 2].Move(storage, 0, -3);
                }
                else if (e.KeyChar == (char)Keys.L)
                {
                    storage.objects[storage.currentSize - 2].Resize(storage, 2);
                }

                else if (e.KeyChar == (char)Keys.K)
                {
                    storage.objects[storage.currentSize - 2 ].Resize(storage, -2);
                }
                /*
          if (e.KeyChar == (char)Keys.S)
          {


              {
                  storage.objects[i].y += 3;

                  storage.objects[i].polygonPoints[0].Y += 3;
                  storage.objects[i].polygonPoints[1].Y += 3;
                  storage.objects[i].polygonPoints[2].Y += 3;

                  MyPaint(i, ref storage);
                  // button3_Click(sender, e);
              }



          }
          if (e.KeyChar == (char)Keys.A)
          {


              {
                  storage.objects[i].x -= 3;
                  storage.objects[i].polygonPoints[0].X -= 3;
                  storage.objects[i].polygonPoints[1].X -= 3;
                  storage.objects[i].polygonPoints[2].X -= 3;

                  MyPaint(i, ref storage);
                  // button3_Click(sender, e);
              }



          }
          if (e.KeyChar == (char)Keys.D)
          {


              {
                  storage.objects[i].x += 3;
                  storage.objects[i].polygonPoints[0].X += 3;
                  storage.objects[i].polygonPoints[1].X += 3;
                  storage.objects[i].polygonPoints[2].X += 3;

                  MyPaint(i, ref storage);
                  // button3_Click(sender, e);
              }



          }
          if (e.KeyChar == (char)Keys.W)
          {


              {
                  storage.objects[i].y -= 3;
                  storage.objects[i].polygonPoints[0].Y -= 3;
                  storage.objects[i].polygonPoints[1].Y -= 3;
                  storage.objects[i].polygonPoints[2].Y -= 3;

                  MyPaint(i, ref storage);
                  // button3_Click(sender, e);
              }



          }
          else if (e.KeyChar == (char)Keys.L)
          {

              storage.objects[i].R += 1;
              storage.objects[i].polygonPoints[0] = new PointF(storage.objects[i].x, (int)(storage.objects[i].y - 2 * storage.objects[i].R * Math.Sqrt(3) / 3));
              storage.objects[i].polygonPoints[1] = new PointF(storage.objects[i].x - storage.objects[i].R, (int)(storage.objects[i].y + storage.objects[i].R * Math.Sqrt(3) / 3));
              storage.objects[i].polygonPoints[2] = new PointF(storage.objects[i].x + storage.objects[i].R, (int)(storage.objects[i].y + storage.objects[i].R * Math.Sqrt(3) / 3));
              MyPaint(i, ref storage);
          }

          else if (e.KeyChar == (char)Keys.K)
          {

              storage.objects[i].R -= 1;
              if (storage.objects[i].R < 1)
                  storage.objects[i].R = 1;
              storage.objects[i].polygonPoints[0] = new PointF(storage.objects[i].x, (int)(storage.objects[i].y - 2 * storage.objects[i].R * Math.Sqrt(3) / 3));
              storage.objects[i].polygonPoints[1] = new PointF(storage.objects[i].x - storage.objects[i].R, (int)(storage.objects[i].y + storage.objects[i].R * Math.Sqrt(3) / 3));
              storage.objects[i].polygonPoints[2] = new PointF(storage.objects[i].x + storage.objects[i].R, (int)(storage.objects[i].y + storage.objects[i].R * Math.Sqrt(3) / 3));
              MyPaint(i, ref storage);
          }
      }*/
            }
            button3_Click(sender, e);

            return;
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            синийToolStripMenuItem.Checked = true;
            желтыйToolStripMenuItem.Checked = false;
            черныйToolStripMenuItem.Checked = false;
            зеленыйToolStripMenuItem.Checked = false;

            current_color = Color.DarkBlue;
            button3_Click(sender, e);
        }

        private void желтыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            синийToolStripMenuItem.Checked = false;
            желтыйToolStripMenuItem.Checked = true;
            черныйToolStripMenuItem.Checked = false;
            зеленыйToolStripMenuItem.Checked = false;

            current_color = Color.Yellow;
            button3_Click(sender, e);
        }

        private void черныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            синийToolStripMenuItem.Checked = false;
            желтыйToolStripMenuItem.Checked = false;
            черныйToolStripMenuItem.Checked = true;
            зеленыйToolStripMenuItem.Checked = false;

            current_color = Color.Black;
            button3_Click(sender, e);
        }

        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            синийToolStripMenuItem.Checked = false;
            желтыйToolStripMenuItem.Checked = false;
            черныйToolStripMenuItem.Checked = false;
            зеленыйToolStripMenuItem.Checked = true;

            current_color = Color.Green;
            button3_Click(sender, e);
        }



        private void Saving_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream f = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                StreamWriter stream = new StreamWriter(f);
                stream.WriteLine(storage.currentSize);
                if (storage.currentSize != 1)
                {

                    for (int i = 0; i < storage.currentSize - 1; i++)
                        storage.objects[i].Save(stream);
                }
                stream.Close();
                f.Close();
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream f = new FileStream(openFileDialog1.FileName, FileMode.Open);
                StreamReader stream = new StreamReader(f);
                int i = Convert.ToInt32(stream.ReadLine()) - 1;
                Factory shapeFactory = new ShapeFactory();  //фабрика КОНКРЕТНЫХ объектов
                for (int j = 0; j < i; j++)
                {
                    string tmp = stream.ReadLine();
                    storage.add_object(ref sizeStorage, shapeFactory.createShape(tmp), ref kolvo_elem, ref index_sozdania);
                    storage.objects[j].Load(stream);
                }
                stream.Close();
                f.Close();
            }
            picture.Invalidate();
            button3_Click(sender, e);
            //tree.Print();
        }

        private void BTN_group_Click(object sender, EventArgs e)
        {

            if (storage.currentSize != 0)
            {
                Sgroup group = new Sgroup(picture.Width, picture.Height);
                int cnt = 0;
                for (int i = 0; i < storage.currentSize - 1; i++) //считаем количество отмеченных элементов
                    if (storage.objects[i].choose == true)
                        cnt++;
                while (cnt != 0)
                {
                    for (int i = 0; i < storage.currentSize - 1; i++)
                        if (storage.objects[i].choose == true)
                        {
                            group.Add(storage.objects[i]);
                            storage.Delte_obj1(i);
                            kolvo_elem--;
                            index_sozdania--;
                            cnt--;

                        }

                }
                storage.add_object(ref storage.currentSize, group, ref kolvo_elem, ref index_sozdania);
                Remove_Selection(ref storage);
                storage.objects[index_sozdania].color = selected_color;
                storage.objects[index_sozdania].choose = true;
            }
            picture.Invalidate();
            button3_Click(sender, e);

        }

        private void BTN_ungroup_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < storage.currentSize - 1; i++)
            {
                if (storage.objects[i] is Sgroup)
                {
                    while (((Sgroup)storage.objects[i]).objects2.currentSize != 1)
                    {
                        for (int j = ((Sgroup)storage.objects[i]).objects2.currentSize -2; j >=0 ; j--)
                        {
                            storage.add_object(ref storage.currentSize, (Shape)((Sgroup)storage.objects[i]).objects2.objects[j], ref kolvo_elem, ref index_sozdania);
                           
                            ((Sgroup)storage.objects[i]).objects2.Delte_obj1(j);
                            ((Sgroup)storage.objects[i]).objects2.currentSize--;
                            ((Sgroup)storage.objects[i]).objects2.currentpozicion--;
                            ((Sgroup)storage.objects[i]).objects2.currentchislo--;
                        }
                    }
                    storage.Delte_obj1(i);
                    picture.Invalidate();
                }
            }
        } 


        /*   private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
           {

                   if ((e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse) && e.Node.Text != "Фигуры")
                   {


                       TreeNode tmp = e.Node;

                       while (tmp.Parent.Text != "Фигуры") tmp = tmp.Parent;
                       treeView1.SelectedNode = tmp;
                       for (int i = 0; i < tmp.Index; i++)
                       {


                       }

                   }


           }

           private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
           {

                   TreeNode tmp = e.Node;
                   treeView1.SelectedNode = tmp;



                   for (int i = 0; i < treeView1.SelectedNode.Index; i++)
                   {

                   }



                   picture.Invalidate();

               }

           private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
           {

               TreeNode tmp = e.Node;

               if (e.Button == MouseButtons.Right)
               {
                   object
                   sto.setCurPTR();

                   for (int i = 0; i < tmp.Index; i++)
                   {
                       sto.nextCur();
                   }
                   if (sto.size() != 0 && sto.get() is SGroup)
                   {
                       while (((SGroup)sto.get()).size() != 0)
                       {
                           sto.addAfter(((SGroup)sto.get()).Out());
                           sto.prevCur();
                       }
                       sto.del();
                   }
               }
               if (e.Button == MouseButtons.Left)
               {

                   treeView1.SelectedNode = tmp;

                   sto.toFirst();
                   for (int i = 0; i <= treeView1.SelectedNode.Index; i++)
                   {
                       sto.next();
                   }
                   sto.check();

               }
               pictureBox1.Invalidate();




           }*/

    }
}
