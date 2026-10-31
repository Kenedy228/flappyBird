namespace FlappyBird
{
    internal sealed class GameSimulation
    {
        private const double Step = 1.0 / 60;
        private double accumulatedTime;

        public Bird Bird { get; } = new Bird();
        public Score Score { get; } = new Score();
        public List<Pipe> Pipes { get; } = new List<Pipe>
        {
            new Pipe(new float[] { 1f, 1.2f, 1.2f, 1f }),
            new Pipe(new float[] { 2f, 2.2f, 2.2f, 2f }),
        };

        public bool IsFinished => Bird.gameFinish || Pipes.Any(pipe => pipe.gameFinish);

        public void Update(double elapsed)
        {
            if (!double.IsFinite(elapsed) || elapsed < 0)
                throw new ArgumentOutOfRangeException(nameof(elapsed));

            if (IsFinished) return;

            accumulatedTime += elapsed;
            // Allow rounding error when the same duration is split into different frames.
            while (accumulatedTime + 1e-12 >= Step && !IsFinished)
            {
                accumulatedTime -= Step;
                foreach (var pipe in Pipes)
                    pipe.Move(Bird.xCoordinates, Bird.yCoordinates, Score);
                Bird.Update();
            }
        }
    }
}
