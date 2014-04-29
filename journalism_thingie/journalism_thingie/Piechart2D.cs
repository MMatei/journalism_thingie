using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace journalism_thingie
{
    /// <summary>
    /// A simple 2D piechart, with a resolution of 1%
    /// </summary>
    class Piechart2D
    {
        //here we will keep the 100 points that make up our circle
        //the ideea behind making it static is providing for the situation when we have multiple piecharts on the same page
        private static Vector3[] circlePoints;
        private static Effect effect;//one effect to draw them all
        private static GraphicsDevice gdi;
        private static Texture2D circle;
        internal List<PieSlice> slices = new List<PieSlice>();
        private Matrix world;
        private Matrix view;
        private Matrix projection;

        public Piechart2D(GraphicsDevice gdi, ContentManager content, Texture2D circle, int screenW, int screenH)
        {
            /* How does one make a piechart? The answer is deceptively simple: given a circle made of 100 points, you
             * have 100 triangle strips. From here you just have to create a separate object for each category you want
             * to represent on the piechart.
             * For example, if I wish to represent a group of x%, then I need x triangles of the circle.
             * Because of the way vertices work, we can't have a continuous circle; two adjacent strips can't share a vertex.
             * In order to make this implementation as readable as possible, I have devised the following plan:
             * 1. Compute the 100 points that form the circle
             * 2. Whenever I wish to create a piechart, given a list of percentages, create vertices using the
             *      circlePoints array as support.
             * To understand why I'd get headaches if I had to compute the x and y for each vertex on the spot,
             * check the code below
             */
            if (circlePoints == null) //circlePoints is static <=> initialize only once
            {
                circlePoints = new Vector3[100];
                double x, y;//raza e 1
                //parcurgem cercul in sens trigonometric
                //cele 4 cadrane sunt simetrice, deci trebuie calculati x si y doar pentru cadranul I
                for (int i = 0; i < 25; i++)//100 puncte in total => 25 per cadran
                {
                    x = Math.Cos(i * Math.PI / 50);
                    y = Math.Sin(i * Math.PI / 50);
                    circlePoints[i] = new Vector3((float)x, (float)(y), 0);
                    circlePoints[49 - i] = new Vector3((float)-x, (float)y, 0);
                    circlePoints[50 + i] = new Vector3((float)-x, (float)-y, 0);
                    circlePoints[99 - i] = new Vector3((float)x, (float)-y, 0);
                }
            }
            if (effect == null)
                effect = content.Load<Effect>("SimpleEffect");
            if (Piechart2D.gdi == null)
                Piechart2D.gdi = gdi;
            if (Piechart2D.circle == null)
                Piechart2D.circle = circle;

            world = Matrix.CreateTranslation(0, 0, 0);
            view = Matrix.CreateLookAt(new Vector3(0, 0, 2), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), (float)screenW / (float)screenH, 0.1f, 100f);
        }

        public void setPiechart(List<int> slicePerecentage, List<Color> sliceColor, List<String> descr)
        {
            // first - we check for consistency - better safe than sorry!
            int sum = 0;
            foreach (int i in slicePerecentage)
                sum += i;
            if (sum != 100)
            {
                Console.WriteLine("Piechart2D Critical ERROR: sum of percentages is " + sum);
                return;
            }
            // and now for the main event
            int point = 0;//we start from the first point, 0
            for (int i = 0; i < slicePerecentage.Count; i++)
            {
                int percentage = slicePerecentage[i];
                if (percentage == 0)
                    continue;
                Color color = sliceColor[i];
                PieSlice slice = new PieSlice();
                VertexPositionColor[] vertices = new VertexPositionColor[percentage + 2];
                short[] indices = new short[3 * percentage];
                for (int j = 0; j < percentage; j++)
                {
                    vertices[j] = new VertexPositionColor(circlePoints[point++], color);
                    indices[3 * j] = (short)j;
                    indices[3 * j + 1] = (short)(percentage + 1);
                    indices[3 * j + 2] = (short)(j + 1);
                }
                vertices[percentage] = new VertexPositionColor(circlePoints[point==100?0:point], color);
                vertices[percentage + 1] = new VertexPositionColor(Vector3.Zero, color);
                slice.vb = new VertexBuffer(gdi, typeof(VertexPositionColor), vertices.Length, BufferUsage.None);
                slice.vb.SetData<VertexPositionColor>(vertices);
                slice.ib = new IndexBuffer(gdi, typeof(short), indices.Length, BufferUsage.None);
                slice.ib.SetData<short>(indices);
                slice.color = color;
                slice.description = descr[i];
                slices.Add(slice);
            }
        }

        public void draw()
        {
            gdi.Clear(Color.Transparent);
            //MVP matrixes are the same for all slices
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["Texture"].SetValue(circle);
            foreach (PieSlice slice in slices)
            {
                gdi.Indices = slice.ib;
                gdi.SetVertexBuffer(slice.vb);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gdi.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, slice.vb.VertexCount, 0, slice.ib.IndexCount / 3);
                }
            }
        }
    }
    class PieSlice
    {
        internal IndexBuffer ib;
        internal VertexBuffer vb;
        internal Color color;
        internal String description;
    }
}
