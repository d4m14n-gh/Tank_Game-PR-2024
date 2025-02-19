using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace projektRozproszone
{
    internal abstract class GameObject
    {
        public string ObjectName { get; set; }
        public Vector2 Position { get; set; } = new Vector2(1000, 0);
        public Vector2 Velocity { get; set; } = new Vector2 (0, 0);
        public double Drag { get; set; } = 1;
        public int Layer { get; set; } = 0;
        protected Game Game { get; set; }
        
        public GameObject(Game game, string name = "gameObject")
        {
            Game = game;
            ObjectName = name;
        }
        public virtual void Start() { }
        public virtual void Update(double time) { }
        public virtual void Go(double time)
        {
            Velocity *= Math.Max(1 - (Drag*time / 1000.0), 0);
            Position += Velocity * time / 1000.0;
        }
        public abstract void Draw(Graphics g, Vector2 camera, Size size, double scale = 1.0);
        public Vector2 GetScreenPosition(Vector2 camera, Size size, double scale = 1.0)
        {
            double x = size.Width / 2.0 - scale * (camera.x - Position.x);
            double y = size.Height / 2.0 + scale * (camera.y - Position.y);
            return new Vector2(x, y);
        }
        public void Spawn()
        {
            Game.AddGameObject(this);
        }
        public void Destroy()
        {
            Game.RemoveGameObject(this);
        }

        public class LayerComparer : IComparer<GameObject>
        {
            public int Compare(GameObject x, GameObject y)
            {
                return x.Layer.CompareTo(y.Layer);
            }
        }
    }
}
