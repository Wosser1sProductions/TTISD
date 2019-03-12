﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace TTISDassignment2
{
    class Ball : Mover
    {
        private Texture texture = new Texture();
        private bool initialized = false;

        public Ball(double x, double y, double z, double w, double h, double b = 1)
            : base(x, y, z, w, h, b)
        {

        }

        public Ball(Point3D pos, Size size, Color color, double b = 1)
            : base(pos, size, color, b)
        {

        }

        public bool collidesWith(Block b)
        {
            if (base.collidesWith(b))
            {
                b.Hit();
                return true;
            }

            return false;
        }

        public void init(OpenGL gl)
        {
            var img = (BitmapImage)App.Current.Resources["texBall"];
            texture.Create(gl, Util.BitmapImage2Bitmap(img));
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.TextureName);

            initialized = true;
        }

        public override void drawFilled(OpenGL gl)
        {
            if (!initialized)
            {
                init(gl);
            }

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.TextureName);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(Pos.X, Pos.Y, Pos.Z);                    // Bottom Left Of The Texture and Quad
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(Pos.X + Size.X, Pos.Y, Pos.Z);           // Bottom Right Of The Texture and Quad
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(Pos.X + Size.X, Pos.Y + Size.Y, Pos.Z);  // Top Right Of The Texture and Quad
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(Pos.X, Pos.Y + Size.Y, Pos.Z);           // Top Left Of The Texture and Quad
            gl.End();

            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);


            //gl.Begin(OpenGL.GL_LINE_LOOP);
            //gl.Color(FillColor.R, FillColor.G, FillColor.B, FillColor.A);

            //for (int i = 0; i <= 10; i++)
            //{
            //    double angle = 2 * Math.PI * i / 10;
            //    double x = Math.Cos(angle) - Pos.X;
            //    double y = Math.Sin(angle) - Pos.Y;

            //    gl.Vertex(x, y, Pos.Z);
            //}

            //gl.End();
        }

        public override void drawBorder(OpenGL gl)
        {

        }
    }
}
