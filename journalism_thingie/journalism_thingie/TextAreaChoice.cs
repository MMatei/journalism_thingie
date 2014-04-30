using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace journalism_thingie
{
    class TextAreaChoice
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private List<String> description = new List<String>();
        private List<String>[] choice;
        private int height, width;
        private Vector2 start;
        // considering that text may overflow, we require a second page
        private bool hasSecondPage, onSecondPage;
        private int secondPageChoice = 9999;//the first choice of the second page

        public TextAreaChoice(SpriteBatch sb, SpriteFont font, int startX, int startY, int width, int height)
        {
            spriteBatch = sb;
            this.font = font;
            this.height = height;
            this.width = width;
            start = new Vector2(startX, startY);
        }

        public void setText(String descr, String[] choice)
        {
            int spaceSize = (int)font.MeasureString(" ").X;
            int textHeight = 0;//we measure height to determine if we need a second page
            //trebuie sa impartim descrierea pe linii, pt ca: primim 1 sir si DrawString deseneaza doar pe o linie continua
            String[] word = descr.Split(' ');
            String line = "    ";
            int lineLength = 4*spaceSize;
            foreach (String w in word)
            {
                int wordLength = (int)font.MeasureString(w).X + spaceSize;
                if (lineLength + wordLength < width)
                {
                    lineLength += wordLength;
                    line += w + " ";
                }
                else
                {
                    description.Add(line);
                    textHeight += font.LineSpacing;
                    line = w + " ";
                    lineLength = wordLength;
                }
            }
            textHeight += font.LineSpacing;
            description.Add(line);
            this.choice = new List<String>[choice.Length];
            for (int i = 0; i < choice.Length; i++)
            {
                word = choice[i].Split(' ');
                line = "    ";
                lineLength = 4*spaceSize;
                this.choice[i] = new List<String>();
                foreach (String w in word)
                {
                    int wordLength = (int)font.MeasureString(w).X + spaceSize;
                    if (lineLength + wordLength < width)
                    {
                        lineLength += wordLength;
                        line += w + " ";
                    }
                    else
                    {
                        textHeight += font.LineSpacing;
                        if (textHeight > height * 0.8)
                        {
                            hasSecondPage = true;
                            secondPageChoice = i;
                        }
                        this.choice[i].Add(line);
                        line = w + " ";
                        lineLength = wordLength;
                    }
                }
                textHeight += font.LineSpacing;
                this.choice[i].Add(line);
            }
        }

        public void draw()
        {
            Vector2 deplasament = start;
            if (onSecondPage)
            {
                for (int i = secondPageChoice; i < choice.Length; i++)
                {
                    foreach (String line in choice[i])
                    {
                        spriteBatch.DrawString(font, line, deplasament, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                        deplasament.Y += font.LineSpacing;
                    }
                }
            }
            else
            {
                foreach (String word in description)
                {
                    spriteBatch.DrawString(font, word, deplasament, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    deplasament.Y += font.LineSpacing;
                }
                deplasament.Y += font.LineSpacing;
                int n = choice.Length > secondPageChoice ? secondPageChoice : choice.Length;
                for (int i = 0; i < n; i++ )
                {
                    foreach (String line in choice[i])
                    {
                        spriteBatch.DrawString(font, line, deplasament, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                        deplasament.Y += font.LineSpacing;
                    }
                }
            }
        }

        /// <summary>
        /// If the user clicked on a choice, returns choice nr. Otherwise, returns -1;
        /// </summary>
        public int update(MouseState mouseStateCrrt, KeyboardState keyCurrent)
        {
            if (keyCurrent.IsKeyDown(Keys.Right))
                onSecondPage = true;
            if (keyCurrent.IsKeyDown(Keys.Left))
                onSecondPage = false;
            int mouseY = mouseStateCrrt.Y, mouseX = mouseStateCrrt.X;
            if (mouseStateCrrt.LeftButton == ButtonState.Pressed && Game.mousePrevious.LeftButton == ButtonState.Released
                && mouseX > start.X && mouseX < (start.X + width)
                && mouseY > start.Y )
            {//we check what option is being clicked
                //dupa acea prima verificare, ma aflu _sigur_ in patratelul carnetelului
                if (onSecondPage)
                {
                    int dist = (int)start.Y;
                    for (int i = secondPageChoice; i < choice.Length; i++)
                    {//for each choice
                        dist += choice[i].Count * font.LineSpacing;
                        if (mouseY < dist)
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    if (mouseY <= description.Count * font.LineSpacing + start.Y)
                        return -1;//verificare suplimentara ca nu am dat click pe descriere
                    int dist = (description.Count + 1) * font.LineSpacing + (int)start.Y;
                    int n = choice.Length > secondPageChoice ? secondPageChoice : choice.Length;
                    for (int i = 0; i < n; i++)
                    {//for each choice
                        dist += choice[i].Count * font.LineSpacing;
                        if (mouseY < dist)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
    }
}
