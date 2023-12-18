using System;

namespace FlappyBird
{
    internal class Background : TextureDrawing
    {
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

            base.Draw(
                new float[,] { { 0f, 1f, 1f, 0f }, { 1f, 1f, 0f, 0f } },
                coordinates
            );
        }
    }
}