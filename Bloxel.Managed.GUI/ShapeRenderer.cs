/*
 * Bloxel.Managed.GUI - ShapeRenderer.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bloxel.Managed.GUI
{
    public class ShapeRenderer
    {
        private SpriteBatch _spriteBatch;

        private GUIContentLibrary _contentLibrary;

        public ShapeRenderer(GraphicsDevice device, GUIContentLibrary contentLibrary)
        {
            _spriteBatch = new SpriteBatch(device);

            _contentLibrary = contentLibrary;
        }

        public void DrawRectangle(Color c, Rectangle r)
        {
            DrawRectangle(_contentLibrary.DummyTexture, r, c);
        }

        public void DrawRectangle(Texture2D texture, Rectangle r)
        {
            DrawRectangle(texture, r, Color.White);
        }

        public void DrawRectangle(Texture2D texture, Rectangle r, Color tint)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(texture, r, tint);

            _spriteBatch.End();
        }

        public void DrawBorder(Color c, Rectangle innerRectangle, int borderWidth)
        {
            _spriteBatch.Begin();
            // top border
            _spriteBatch.Draw(_contentLibrary.DummyTexture, new Rectangle(innerRectangle.X - borderWidth, innerRectangle.Y - borderWidth, innerRectangle.Width + borderWidth * 2, borderWidth), c);
            // bottom border
            _spriteBatch.Draw(_contentLibrary.DummyTexture, new Rectangle(innerRectangle.X - borderWidth, innerRectangle.Y + innerRectangle.Height, innerRectangle.Width + borderWidth * 2, borderWidth), c);
            // left border
            _spriteBatch.Draw(_contentLibrary.DummyTexture, new Rectangle(innerRectangle.X - borderWidth, innerRectangle.Y, borderWidth, innerRectangle.Height), c);
            // right border
            _spriteBatch.Draw(_contentLibrary.DummyTexture, new Rectangle(innerRectangle.X + innerRectangle.Width, innerRectangle.Y, borderWidth, innerRectangle.Height), c);
            _spriteBatch.End();
        }
    }
}
