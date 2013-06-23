/*
 * Bloxel.Managed.GUI - Panel.cs
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

namespace Bloxel.Managed.GUI.Controls
{
    public class Panel : ContainerControl
    {
        private Vector2 _position;
        private int _width, _height;

        private Color _backgroundUnfocused;
        private Color _backgroundFocused;
        private Color _borderColorUnfocused;
        private Color _borderColorFocused;

        private Texture2D _backgroundTextureUnfocused;
        private Texture2D _backgroundTextureFocused;

        private readonly int BORDER_WIDTH = 1;

        /// <summary>
        /// Returns the absolute position, from the top left corner of the rendering surface, of this control. 
        /// </summary>
        public override Vector2 AbsolutePosition { get { return ParentControl.AbsolutePosition + Position; } }
        public override Vector2 Position { get { return _position; } set { _position = value; } }
        public override Vector2 ClientAreaPosition { get { return Position + new Vector2(BORDER_WIDTH, BORDER_WIDTH); } }

        public override int Width { get { return _width; } set { _width = value; } }
        public override int Height { get { return _height; } set { _height = value; } }

        public override Rectangle ControlRectangle { get { return new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, Width + 2 * BORDER_WIDTH, Height + 2 * BORDER_WIDTH); } }
        public override Rectangle ClientAreaRectangle { get { return new Rectangle((int)AbsoluteClientAreaPosition.X, (int)AbsoluteClientAreaPosition.Y, Width, Height); } }

        public Color BackgroundUnfocusedColor { get { return _backgroundUnfocused; } set { _backgroundUnfocused = value; } }
        public Color BackgroundFocusedColor { get { return _backgroundFocused; } set { _backgroundFocused = value; } }

        public Color BorderUnfocusedColor { get { return _borderColorUnfocused; } set { _borderColorUnfocused = value; } }
        public Color BorderFocusedColor { get { return _borderColorFocused; } set { _borderColorFocused = value; } }

        public Texture2D BackgroundUnfocusedTexture { get { return _backgroundTextureUnfocused; } set { _backgroundTextureUnfocused = value; } }
        public Texture2D BackgroundFocusedTexture { get { return _backgroundTextureFocused; } set { _backgroundTextureFocused = value; } }

        public Panel()
            : base()
        {
            _backgroundUnfocused = Color.LightGray;
            _backgroundFocused = Color.White;
            _borderColorFocused = Color.Black;
            _borderColorUnfocused = _backgroundUnfocused;
        }

        internal override void internal_onloaded()
        {
            _backgroundTextureFocused = ContentLibrary.DummyTexture;
            _backgroundTextureUnfocused = ContentLibrary.DummyTexture;
        }

        protected override void onFocused()
        {
        }

        protected override void onUnfocused()
        {
        }

        internal override void Update(GameTime gameTime)
        {
            UpdateChildControls(gameTime);

            _renderBackgroundColor = _backgroundUnfocused;
            _renderBackgroundTexture = _backgroundTextureUnfocused;

            if (Focused)
            {
                _renderBackgroundColor = _backgroundFocused;
                _renderBackgroundTexture = _backgroundTextureFocused;
            }
        }

        private Color _renderBackgroundColor;
        private Texture2D _renderBackgroundTexture;

        internal override void Draw(GameTime gameTime)
        {
            if (_renderBackgroundTexture != null)
            {
                ShapeRenderer.DrawRectangle(_renderBackgroundTexture, ClientAreaRectangle, _renderBackgroundColor);
            }

            for (int i = 0; i < ChildControls.Count; i++)
                ChildControls[i].Draw(gameTime);

            ShapeRenderer.DrawBorder(Focused ? _borderColorFocused : _borderColorUnfocused, ClientAreaRectangle, BORDER_WIDTH);
        }
    }
}
