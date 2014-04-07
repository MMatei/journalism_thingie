using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace journalism_thingie
{
    class SubtitleArea
    {
        private GraphicsDevice gdi;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private List<String> text = new List<String>();
        //vrem sa tinem subs centrate pe ecran => tinem o lista cu deplasamente pe x to ensure just that
        private List<int> lineDeplas = new List<int>();
        private int crrtLine;
        private int height, width;
        private Vector2 start;

        public SubtitleArea(GraphicsDevice gdi, SpriteBatch sb, SpriteFont font, int startX, int startY, int width, int height)
        {
            this.gdi = gdi;
            spriteBatch = sb;
            this.font = font;
            this.height = height;
            this.width = width;
            start = new Vector2(startX, startY);
        }

        public void setText(String txt)
        {
            int spaceSize = (int)font.MeasureString(" ").X;
            //trebuie sa impartim descrierea pe linii, pt ca: primim 1 sir si DrawString deseneaza doar pe o linie continua
            text.Clear();
            lineDeplas.Clear();
            String[] word = txt.Split(' ');
            String line = "";
            int lineLength = 0;
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
                    text.Add(line);
                    lineDeplas.Add((width - lineLength)/2);
                    line = w + " ";
                    lineLength = wordLength;
                }
            }
            text.Add(line);
            crrtLine = 0;
            lineDeplas.Add((width - lineLength) / 2);
        }

        public void draw()
        {
            if (crrtLine == text.Count - 1)
            {
                float x = start.X + lineDeplas[crrtLine];
                float y = start.Y;
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x - 1, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x + 1, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x, y + 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x, y - 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x, y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                float x = start.X + lineDeplas[crrtLine];
                float y = start.Y;
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x - 1, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x + 1, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x, y + 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x, y - 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(x, y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

                x = start.X + lineDeplas[crrtLine + 1];
                y = start.Y + font.LineSpacing;
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(x - 1, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(x + 1, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(x, y - 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(x, y + 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(x, y), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        /// <summary>
        /// Returns 1 if we're out of subtitles to show.
        /// </summary>
        public int update(KeyboardState keyCurrent, KeyboardState keyPrevious)
        {
            // space sau enter trec la subtitrarea urmatoare
            if ((keyCurrent.IsKeyDown(Keys.Space) && keyPrevious.IsKeyUp(Keys.Space)) ||
                (keyCurrent.IsKeyDown(Keys.Enter) && keyPrevious.IsKeyUp(Keys.Enter)))
            {
                crrtLine += 2;
                if (crrtLine >= text.Count)
                    return 1;//end of the line
            }
            return 0;
        }
    }
}
