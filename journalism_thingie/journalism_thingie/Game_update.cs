﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace journalism_thingie
{
    public partial class Game
    {
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyCurrent = Keyboard.GetState();
            MouseState mouseStateCurrent = Mouse.GetState();
            // Allows the game to exit
            if (keyCurrent.IsKeyDown(Keys.Escape))
                this.Exit();

            switch (gameState)
            {
                case NOTEPAD:
                    {
                        int ret = notepadChoice.update(mouseStateCurrent);
                        if (ret != -1)
                        {
                            gameState = WATCH_NEWS;
                            tvSpeech.setText(crrtNews.options[ret].newsArticle);
                        }
                    }
                    break;
                case FOCUS_GROUP:
                    {
                        int ret = focusGroupView.update(mouseStateCurrent, mousePrevious, keyCurrent, gameTime);
                        if (ret == 1)
                        {
                            gameState = NOTEPAD;
                        }
                    }
                    break;
                case WATCH_NEWS:
                    {
                        if (tvSpeech.update(keyCurrent, keyPrevious) == 1)
                            gameState = NOTEPAD;
                    }
                    break;
            }
            keyPrevious = keyCurrent;
            mousePrevious = mouseStateCurrent;
        }

        /// <summary>
        /// Called in order to provide the ending's text crawl.
        /// </summary>
        private String endgame()
        {
            int[] tally = new int[24];
            Array.Clear(tally, 0, 24);
            foreach (Citizen c in population)
            {
                tally[c.vote()]++;
            }
            int max = 0, result = 0;
            for (int i = 0; i < 24; i++)
            {
                if (tally[i] > max)
                {
                    max = tally[i];
                    result = i;
                }
            }

            StreamReader input = new StreamReader("endings.txt");
            for (int i = 0; i < result; i++)
            {
                input.ReadLine();
            }
            String rez = input.ReadLine();
            input.Close();
            return rez;
        }
    }
}