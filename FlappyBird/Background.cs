using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FlappyBird
{
    internal class Background : Object
    {
        public Color4 maskColor = Color4.White;
        private float[,] coordinates =
        {
            {-1, 1, 1, -1},
            {-1, -1, 1, 1}
        };

        public Background()
        {

        }

        public void DrawBackground(int textureId)
        {
            base.Bind(textureId);
            
            GL.Color4(maskColor);

            base.Draw(
                new float[,] { { 0f, 1f, 1f, 0f }, { 1f, 1f, 0f, 0f } },
                coordinates
            );
        }
    }
}