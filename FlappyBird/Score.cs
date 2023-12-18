using System;

namespace FlappyBird
{
    internal class Score : TextureDrawing
    {
        private int counter = 0, n;
        private List<int> frameNumbers = new List<int> { 0 };

        private int rows = 1,
            columns = 10,
            choosenAnimationFrameCount = 10,
            choosenAnimationRowNumber = 0,
            choosenAnimationFrameNumber = 0;

        private float frameWidth, frameHeight;

        public Score()
        {
            frameWidth = 1.0f / columns;
            frameHeight = 1.0f / rows;
        }

        public void DrawScore(int textureId)
        {
            base.Bind(textureId);
            n = 0;

            for (int i = frameNumbers.Count - 1; i >= 0; i--)
            {
                choosenAnimationFrameNumber = frameNumbers[i];

                float x = choosenAnimationFrameNumber * frameWidth;
                float y = choosenAnimationRowNumber * frameHeight;
                float distance = 0.05f * n;

                base.Draw(
                    new float[,] 
                    {
                        { x, x + frameWidth, x + frameWidth, x },
                        { y, y, y + frameHeight, y + frameHeight }
                    },
                    new float[,] 
                    { 
                        { -0.05f + distance, 0f + distance, 0f + distance, -0.05f + distance },
                        { 0.9f, 0.9f, 0.8f, 0.8f } 
                    }
                );

                n++;
            }
        }

        public void IncrementCounter()
        {
            frameNumbers.Clear();

            counter++;
            int temp = counter;

            while (temp > 0)
            {
                frameNumbers.Add(temp % 10 % choosenAnimationFrameCount);
                temp /= 10;
            }
        }
    }
}