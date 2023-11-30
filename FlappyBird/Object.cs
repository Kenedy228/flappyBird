using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FlappyBird
{
    internal class Object
    {
        public void Bind(int textureId)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
        }

        public void Draw(float[,] texCoordinates, float[,] vertexCoordinates)
        {
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(texCoordinates[0, 0], texCoordinates[1, 0]);
            GL.Vertex2(vertexCoordinates[0, 0], vertexCoordinates[1, 0]);

            GL.TexCoord2(texCoordinates[0, 1], texCoordinates[1, 1]);
            GL.Vertex2(vertexCoordinates[0, 1], vertexCoordinates[1, 1]);

            GL.TexCoord2(texCoordinates[0, 2], texCoordinates[1, 2]);
            GL.Vertex2(vertexCoordinates[0, 2], vertexCoordinates[1, 2]);

            GL.TexCoord2(texCoordinates[0, 3], texCoordinates[1, 3]);
            GL.Vertex2(vertexCoordinates[0, 3], vertexCoordinates[1, 3]);

            GL.End();
        }

        public bool IsButton(Vector2 cursorPosition, float[,] coordinates)
        {
            if 
                (
                cursorPosition.X > coordinates[0, 0] && cursorPosition.X < coordinates[0, 1]
                && cursorPosition.Y > coordinates[1, 2] && cursorPosition.Y < coordinates[1, 0]
                )
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}