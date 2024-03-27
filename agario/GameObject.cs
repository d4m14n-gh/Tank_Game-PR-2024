﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agario
{
    internal abstract class GameObject
    {
        public int Id {  get; set; }
        public bool IsSyncing { get; set; } = true;
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public double Drag {  get; set; }
        public int Layer {  get; set; }
        private static List<GameObject> gameObjects = new List<GameObject>();
        public static GameObject[] GameObjects{ get { lock(gameObjects) return gameObjects.ToArray<GameObject>(); } }
        static int idc = 1;
        public GameObject(string name="gameObject") {
            this.Id = idc++;
            this.Name = name;
            this.Position = new Vector2(0, 0);
            this.Velocity = new Vector2(0, 0);
            this.Drag = 0;
            this.Layer = 0;
            
        }

        protected void Initialize()
        {
            lock (gameObjects)
                gameObjects.Add(this);
        }
        public virtual void Start() { }
        public virtual void Update(double time) { }
        public virtual void Go(double time)
        {
            Velocity *= Math.Max(1 - Drag * (time / 1000.0), 0);
            Position += Velocity * (time/1000.0);
        }
        public abstract void Draw(Graphics g, double camerax, double cameray, Size size, double scale = 1.0);
        public Point GetScreenPosition(double camerax, double cameray, Size size, double scale = 1.0)
        {
            double x = (size.Width / 2.0 - scale * (camerax - Position.x));
            double y = (size.Height / 2.0 + scale * (cameray - Position.y));
            return new Point((int)Math.Round(x), (int)Math.Round(y));
        }
        public void Destroy()
        {
            //Position = new Vector2(1000, 1000);
            if( gameObjects.Contains(this)) 
                lock(gameObjects) 
                    gameObjects.Remove(this);
        }

        public class LayerComparer : IComparer<GameObject>
        {
            public int Compare(GameObject x, GameObject y)
            {
                return x.Layer.CompareTo(y.Layer);
            }
        }
        public class SizeComparer : IComparer<GameObject>
        {
            public int Compare(GameObject x, GameObject y)
            {
                if (new LayerComparer().Compare(x, y) != 0)
                    return new LayerComparer().Compare(x, y);
                if(x == null||y== null)
                    return 0;
                if(x.GetType() != y.GetType())
                {
                    if (x.GetType() == typeof(Player))
                        return 1;
                    else if(y.GetType() == typeof(Player))
                        return -1;
                    else
                        return 0;
                }
                else if(x.GetType() == typeof(Player))
                {
                    Player pl = (Player) x;
                    return pl.Size.CompareTo(((Player)y).Size);
                }
                return 0;
            }
        }

    }
}
