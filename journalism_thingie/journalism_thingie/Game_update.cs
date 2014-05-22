using System;
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
        internal void SaveGame(String saveFileName)
        {
            StreamWriter savefile = new StreamWriter(saveFileName);
            savefile.WriteLine(day);
            foreach (Citizen c in population)
            {
                savefile.WriteLine(c.lifeQuality + ";" + c.fanaticism + ";" + c.ideology + ";" + c.nationalist + ";" +
                    c.minorityRights + ";" + c.isolationism + ";" + c.socialJustice + ";" + c.mediaTrust);
            }
        }
        internal void LoadGame(String saveFileName)
        {
            StreamReader savefile = new StreamReader(saveFileName);
            day = Convert.ToInt32(savefile.ReadLine());
            foreach (Citizen c in population)
            {
                String[] word = savefile.ReadLine().Split(';');
                c.lifeQuality = Convert.ToDouble(word[0]);
                c.fanaticism = Convert.ToDouble(word[1]);
                c.ideology = Convert.ToDouble(word[2]);
                c.nationalist = Convert.ToDouble(word[3]);
                c.minorityRights = Convert.ToDouble(word[4]);
                c.isolationism = Convert.ToDouble(word[5]);
                c.socialJustice = Convert.ToDouble(word[6]);
                c.mediaTrust = Convert.ToDouble(word[7]);
            }
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyCurrent = Keyboard.GetState();
            MouseState mouseStateCurrent = Mouse.GetState();

            switch (gameState)
            {
                case NOTEPAD:
                {
                    int ret = notepadChoice.update(mouseStateCurrent, keyCurrent);
                    if (ret != -1)
                    {
                        gameState = WATCH_NEWS;
                        tvSpeech.setText(crrtNews.options[ret].newsArticle);
                    }
                    else
                    {
                        if (keyCurrent.IsKeyDown(Keys.Enter))
                        {
                            gameState = ROOM;
                        }
                    }
                }
                break;
                case FOCUS_GROUP:
                {
                    int ret = focusGroupView.update(mouseStateCurrent, mousePrevious, keyCurrent, gameTime);
                    if (ret == 1)
                    {
                        gameState = ROOM;
                    }
                }
                break;
                case WATCH_NEWS:
                {
                    if (tvSpeech.update(keyCurrent, keyPrevious) == 1)
                    {
                        day++;//the choice has been made, we move on to the next day
                        if (!File.Exists("news/" + day + ".txt"))
                        {//endgame
                            tvSpeech.setText(endgame());
                            gameState = ENDGAME;
                        }
                        else
                        {
                            crrtNews = new News("news/" + day + ".txt");
                            String[] choices = new String[crrtNews.options.Length];
                            int j = 0;
                            foreach (Option o in crrtNews.options)
                            {
                                choices[j++] = o.description;
                            }
                            notepadChoice.setText(crrtNews.situationDescription, choices);
                            focusGroupView.prepare(population);
                            gameState = ROOM;
                        }
                    }
                }
                break;
                case ROOM:
                {
                    if (keyCurrent.IsKeyDown(Keys.Escape))
                        this.Exit();
                    if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                    {
                        if (snapshotRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                        {//am dat click pe tabla cu focus view
                            gameState = FOCUS_GROUP;
                        }
                        if (notepadSnapshotRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                        {//am dat click pe notepad
                            gameState = NOTEPAD;
                        }
                    }
                }
                break;
                case ENDGAME:
                {
                    if (keyCurrent.IsKeyDown(Keys.Escape))
                        this.Exit();
                    if (tvSpeech.update(keyCurrent, keyPrevious) == 1)
                        this.Exit();
                }
                break;
                case SPLASH_SCREEN:
                {
                    day++;
                    if (day == 100)
                    {
                        day = 1;
                        gameState = MAIN_MENU;
                    }
                }
                break;
                case MAIN_MENU:
                {
                    if(mouseStateCurrent.LeftButton == ButtonState.Pressed)
                    {
                        if(newGameRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                            gameState = ROOM;
                        if(quitRect.Contains(mouseStateCurrent.X, mouseStateCurrent.Y))
                            this.Exit();
                    }
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
