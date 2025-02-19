using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRozproszone
{
    internal class Bullet : GameObject
    {
        public Player Owner { get; set; }
        public Bullet(Game game, Player owner) : base(game, owner.PlayerName+" bullet"){ 
            Owner = owner;
            Layer = -100;
            Drag = 0.5;
        }
        public override void Go(double time)
        {
            base.Go(time);
            if(Velocity.Magnitude<1)
                Destroy();
            //foreach (GameObject go in Game.GameObjects)
            //    if (go.GetType() == typeof(Player))
            //        if ((go.Position - Position).Magnitude < 6)
            //            Velocity = new Vector2(0, 0);
        }
        public override void Draw(Graphics g, Vector2 camera, Size size, double scale = 1)
        {
            float x = (float)GetScreenPosition(camera, size, scale).x;
            float y = (float)GetScreenPosition(camera, size, scale).y;
            const float p = 2f;
            float r = (float)(p * scale);

            using (Pen borderPen = new Pen(GameMethods.Stroke2Color, 1))
            using (SolidBrush circleBrush = new SolidBrush(Owner.PlayerColor))
            {
                borderPen.Width = (float)(scale * 5 * 0.075);
                g.FillEllipse(circleBrush, x - r, y - r, 2 * r, 2 * r);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(x - r, y - r, 2 * r, 2 * r);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        Color centerColor = Color.FromArgb(45, GameMethods.StrokeColor);
                        Color edgeColor = Color.FromArgb(75, GameMethods.StrokeColor);
                        brush.CenterColor = centerColor;
                        brush.SurroundColors = new Color[] { edgeColor };
                        g.FillPath(brush, path);
                    }
                }
                g.DrawEllipse(borderPen, x - r, y - r, 2 * r, 2 * r);
            }
        }
    }
}
