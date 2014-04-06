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
        private int crrtLine, lastLineDeplas;
        internal RenderTarget2D textArea;
        internal Rectangle textAreaRect;

        public SubtitleArea(GraphicsDevice gdi, SpriteBatch sb, SpriteFont font, int startX, int startY, int width, int height)
        {
            this.gdi = gdi;
            spriteBatch = sb;
            this.font = font;
            textArea = new RenderTarget2D(gdi, width, height, false, gdi.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            textAreaRect = new Rectangle(startX, startY, width, height);
        }

        public void setText(String txt)
        {
            //trebuie sa impartim descrierea pe linii, pt ca: primim 1 sir si DrawString deseneaza doar pe o linie continua
            text.Clear();
            String[] word = txt.Split(' ');
            String line = "";
            int lineLength = 0;
            foreach (String w in word)
            {
                int wordLength = (int)font.MeasureString(w).X + 5;
                if (lineLength + wordLength < textAreaRect.Width)
                {
                    lineLength += wordLength;
                    line += w + " ";
                }
                else
                {
                    text.Add(line);
                    line = w + " ";
                    lineLength = wordLength;
                }
            }
            text.Add(line);
            crrtLine = 0;
            lastLineDeplas = (textAreaRect.Width - lineLength) / 2;
        }

        public void draw()
        {
            gdi.SetRenderTarget(textArea);
            gdi.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, null, null, null);

            if (crrtLine == text.Count - 1)
            {
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(lastLineDeplas-1, 0), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(lastLineDeplas+1, 0), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(lastLineDeplas, 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(lastLineDeplas, -1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(lastLineDeplas, 0), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (crrtLine == text.Count - 2)
            {
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(-1, 0), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(1, 0), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(0, 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(0, -1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], Vector2.Zero, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(lastLineDeplas - 1, font.LineSpacing), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(lastLineDeplas + 1, font.LineSpacing), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(lastLineDeplas, font.LineSpacing - 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(lastLineDeplas, font.LineSpacing + 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(lastLineDeplas, font.LineSpacing), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(-1, 0), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(1, 0), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(0, 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], new Vector2(0, -1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine], Vector2.Zero, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);

                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(-1, font.LineSpacing), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(1, font.LineSpacing), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(0, font.LineSpacing - 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(0, font.LineSpacing + 1), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, text[crrtLine + 1], new Vector2(0, font.LineSpacing), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            spriteBatch.End();
            gdi.SetRenderTarget(null);
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
