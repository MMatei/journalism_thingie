using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace journalism_thingie
{
    class FocusGroupView
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Texture2D person, tooltipBackground;
        private Rectangle personRect, tooltipRect;
        private int width, height;
        private Citizen[] population;
        private bool alreadyCounting = false, displayingTooltip = false;
        private int counter;
        private String tooltip;
        private Vector2 tooltipStart = new Vector2();
        private int personsPerRow;

        public FocusGroupView(SpriteBatch sb, SpriteFont font, int screenW, int screenH, Texture2D personTexture,
            Texture2D tooltipTexture, Citizen[] pop)
        {
            spriteBatch = sb;
            this.font = font;
            width = screenW;
            height = screenH;
            this.person = personTexture;
            this.tooltipBackground = tooltipTexture;
            personRect = new Rectangle(0, 0, personTexture.Bounds.Width, personTexture.Bounds.Height);
            population = pop;
            personsPerRow = width / personRect.Width;
        }

        public void draw()
        {
            personRect.X = -personRect.Width;
            personRect.Y = 0;
            for (int i = 0; i < population.Length; i++)
            {
                personRect.X += personRect.Width;
                if (personRect.X + personRect.Width > width)
                {
                    personRect.Y += personRect.Height;
                    personRect.X = 0;
                }
                //color will depend on ideology: extreme right is black, center is blue and extreme left is red
                float ideology = (float)population[i].ideology;
                Color color;
                if (ideology <= 0.5f)
                {
                    color = new Color(0.0f, 0.0f, ideology * 2);
                }
                else
                    color = new Color((ideology - 0.5f) * 2, 0.0f, (1.0f - ideology) * 2);
                spriteBatch.Draw(person, personRect, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
            }
            if (displayingTooltip)
            {
                spriteBatch.Draw(tooltipBackground, tooltipRect, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.95f);
                spriteBatch.DrawString(font, tooltip, tooltipStart, Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        /// <summary>
        /// Check if mouse hovers over person more than 2 seconds => pop-up displaying relevant info.
        /// Returns 1 if user wants to return to notepad
        /// </summary>
        public int update(MouseState mouseStateCrrt, MouseState mouseStatePrev, KeyboardState keyCurrent, GameTime gameTime)
        {
            if (keyCurrent.IsKeyDown(Keys.Enter))
                return 1;
            bool mouseNotMoved = mouseStateCrrt.X == mouseStatePrev.X && mouseStateCrrt.Y == mouseStatePrev.Y;
            if(displayingTooltip)
            {
                if (mouseNotMoved)
                    return 0;
                else
                {
                    displayingTooltip = false;
                    return 0;
                }
            }
            if (mouseNotMoved)
            {
                if (alreadyCounting)
                {
                    counter += gameTime.ElapsedGameTime.Milliseconds;
                    Console.WriteLine(counter);
                    if (counter >= 2000)
                    {
                        displayingTooltip = true;
                        alreadyCounting = false;
                        int x = mouseStateCrrt.X / personRect.Width;
                        int y = mouseStateCrrt.Y / personRect.Height;
                        tooltip = "Ideology: " + population[y * personsPerRow + x].ideology.ToString("0.00");
                        tooltipStart.X = mouseStateCrrt.X + 20;
                        tooltipStart.Y = mouseStateCrrt.Y;
                        Vector2 stringSize = font.MeasureString(tooltip);
                        tooltipRect = new Rectangle((int)tooltipStart.X - 2, (int)tooltipStart.Y - 2, (int)stringSize.X + 4, (int)stringSize.Y + 4);
                    }
                }
                else
                {
                    alreadyCounting = true;
                    counter = 0;
                }
            }
            return 0;
        }
    }
}
