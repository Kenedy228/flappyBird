using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;

namespace FlappyBird
{
    class Game : GameWindow
    {
        private int backgroundId, scoreId, playButtonId,
            settingsButtonId, exitButtonId, backId,
            menuId, restartId;

        private List<int> birds = new List<int>();
        private List<int> pipesTextures = new List<int>();
        private List<int> birdColors = new List<int>();
        private List<int> pipeColors = new List<int>();


        private GameSimulation simulation = new GameSimulation();
        private Background background = new Background();
        private Menu menu = new Menu();

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeSettings)
            : base(gameWindowSettings, nativeSettings)
        {
            birds.Add(GameTextures.LoadTexture("Textures/yellowbird.png"));
            birds.Add(GameTextures.LoadTexture("Textures/bluebird.png"));
            birds.Add(GameTextures.LoadTexture("Textures/redbird.png"));

            pipesTextures.Add(GameTextures.LoadTexture("Textures/pipe-green.png"));
            pipesTextures.Add(GameTextures.LoadTexture("Textures/pipe-red.png"));

            birdColors.Add(GameTextures.LoadTexture("Textures/yellowbirdcoloroption.png"));
            birdColors.Add(GameTextures.LoadTexture("Textures/bluebirdcoloroption.png"));
            birdColors.Add(GameTextures.LoadTexture("Textures/redbirdcoloroption.png"));

            pipeColors.Add(GameTextures.LoadTexture("Textures/greenpipeoption.png"));
            pipeColors.Add(GameTextures.LoadTexture("Textures/redpipeoption.png"));

            backgroundId = GameTextures.LoadTexture("Textures/background.jpeg");
            scoreId = GameTextures.LoadTexture("Textures/numbers.png");
            playButtonId = GameTextures.LoadTexture("Textures/playbutton.png");
            settingsButtonId = GameTextures.LoadTexture("Textures/settingsbutton.png");
            exitButtonId = GameTextures.LoadTexture("Textures/exitbutton.png");
            backId = GameTextures.LoadTexture("Textures/backoption.png");
            menuId = GameTextures.LoadTexture("Textures/menu.png");
            restartId = GameTextures.LoadTexture("Textures/restart.png");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (menu.restart && (menu.gameStatus == 0 || menu.gameStatus == 2)) Clear();

            if (menu.gameStatus == 2)
            {
                simulation.Update(args.Time);
                if (simulation.IsFinished) menu.gameStatus = 3;
            }

            if (menu.gameStatus == -1) Close();
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Keys.Space)
            {
                simulation.Bird.JumpHandler();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Button1)
            {
                menu.MouseClickHandler(MousePosition, ClientSize);
            }

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.Texture2D);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            background.DrawBackground(backgroundId);

            switch (menu.gameStatus)
            {
                case 0:
                    menu.DrawMenu(
                        new int[] { playButtonId, settingsButtonId, exitButtonId },
                        new string[] { "play", "settings", "exit" }
                    );

                    simulation.Bird.DrawBird(birds[menu.birdColorCounter]);
                    break;
                case 1:
                    menu.DrawMenu(
                        new int[] { birdColors[menu.birdColorCounter], pipeColors[menu.pipeColorCounter], backId },
                        new string[] { "birdColor", "pipeColor", "back" }
                    );

                    simulation.Bird.DrawBird(birds[menu.birdColorCounter]);
                    break;
                case 2:
                    for (int i = 0; i < simulation.Pipes.Count; i++)
                    {
                        simulation.Pipes[i].Draw(pipesTextures[menu.pipeColorCounter]);
                    }

                    simulation.Bird.DrawBird(birds[menu.birdColorCounter]);
                    simulation.Score.DrawScore(scoreId);
                    break;
                case 3:

                    simulation.Bird.DrawBird(birds[menu.birdColorCounter]);

                    for (int i = 0; i < simulation.Pipes.Count; i++)
                    {
                        simulation.Pipes[i].Draw(pipesTextures[menu.pipeColorCounter]);
                    }

                    simulation.Score.DrawScore(scoreId);

                    menu.DrawMenu(
                        new int[] { restartId, menuId },
                        new string[] { "restart", "menu" }
                    );

                    break;
            }

            SwapBuffers();

        }

        protected void Clear()
        {
            simulation = new GameSimulation();

            menu.restart = false;
        }
    }
}
