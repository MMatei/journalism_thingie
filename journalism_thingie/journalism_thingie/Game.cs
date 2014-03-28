using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace journalism_thingie
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private int screenW, screenH;
        private Citizen[] population = new Citizen[100];

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            screenW = this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenH = this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            int i;
            uint nrmin = 0, nrmed = 0, nrmax = 0;
            Console.WriteLine("#POOR#");
            for (i = 0; i < 40; i++)
            {
                population[i] = new Citizen(Citizen.POOR, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("#MIDDLE#");
            for (i = 40; i < 90; i++)
            {
                population[i] = new Citizen(Citizen.MIDDLE, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("#RICH#");
            for (i = 90; i < 100; i++)
            {
                population[i] = new Citizen(Citizen.RICH, 0);
                Console.WriteLine(population[i].ideology + " " + population[i].fanaticism);
                if (population[i].fanaticism < 0.1) nrmin++;
                if (population[i].fanaticism > 0.45 && population[i].fanaticism < 0.55) nrmed++;
                if (population[i].fanaticism > 0.9) nrmax++;
            }
            Console.WriteLine("###\nOverly apathetic: " + nrmin);
            Console.WriteLine("Middle ground: " + nrmed);
            Console.WriteLine("Overly fanatic: " + nrmax);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyCurrent = Keyboard.GetState();
            MouseState mouseStateCurrent = Mouse.GetState();
            // Allows the game to exit
            if (keyCurrent.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
