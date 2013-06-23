/*
 * Bloxel.Managed.GUI - Button.cs
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

using input = Bloxel.Managed.GUI.Input.Input;

namespace Bloxel.Managed.GUI.Controls
{
    public class Button : Control
    {
        private Vector2 _position;
        private int _width, _height;

        private FontSize _fontSize;
        private string _text;

        private readonly int BORDER_WIDTH = 1;

        private Texture2D _backgroundImage;
        private Texture2D _backgroundImageClicked;
        private Texture2D _backgroundImageMouseover;
        private Color _backgroundColor;
        private Color _backgroundColorClicked;
        private Color _backgroundColorMouseover;
        private Color _borderColorFocused;
        private Color _textColor;
        private Color _textColorMouseover;
        private Color _textColorClick;

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

        public FontSize FontSize { get { return _fontSize; } set { _fontSize = value; } }
        public string Text { get { return _text; } set { _text = value; } }

        public Texture2D BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; }
        }

        public Texture2D BackgroundImageClicked
        {
            get { return _backgroundImageClicked; }
            set { _backgroundImageClicked = value; }
        }

        public Texture2D BackgroundImageMouseover
        {
            get { return _backgroundImageMouseover; }
            set { _backgroundImageMouseover = value; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        public Color BackgroundColorClicked
        {
            get { return _backgroundColorClicked; }
            set { _backgroundColorClicked = value; }
        }

        public Color BackgroundColorMouseover
        {
            get { return _backgroundColorMouseover; }
            set { _backgroundColorMouseover = value; }
        }

        public Color BorderColorFocused
        {
            get { return _borderColorFocused; }
            set { _borderColorFocused = value; }
        }

        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        public Color TextColorMouseover
        {
            get { return _textColorMouseover; }
            set { _textColorMouseover = value; }
        }

        public Color TextColorClick
        {
            get { return _textColorClick; }
            set { _textColorClick = value; }
        }

        public Button()
            : base()
        {
            _text = "";
            _width = 2;
            _height = 2;

            _backgroundColor = new Color(0.18f, 0.45f, 1.0f);
            _backgroundColorClicked = new Color(15, 99, 193);
            _backgroundColorMouseover = _backgroundColor;
            _borderColorFocused = Color.Black;
            _textColor = Color.White;
            _textColorMouseover = Color.Black;
            _textColorClick = Color.Black;

            _fontSize = FontSize.Small;
        }

        internal override void internal_onloaded()
        {
            _backgroundImage = ContentLibrary.DummyTexture;
            _backgroundImageClicked = ContentLibrary.DummyTexture;
            _backgroundImageMouseover = ContentLibrary.DummyTexture;
        }

        protected override void onFocused()
        {
        }

        protected override void onUnfocused()
        {
        }

        internal override void Update(GameTime gameTime)
        {
            _renderBackgroundColor = _backgroundColor;
            _renderBackgroundTexture = _backgroundImage;
            _renderTextColor = _textColor;

            if(ControlRectangle.Contains(input.Get().MouseXCoordinate(), input.Get().MouseYCoordinate()))
            {
                // mouse over
                _renderBackgroundTexture = _backgroundImageMouseover;
                _renderBackgroundColor = _backgroundColorMouseover;
                _renderTextColor = _textColorMouseover;
            }

            if(Focused)
            {
                if(input.Get().IsLeftMouseButtonDown())
                {
                    _renderBackgroundColor = _backgroundColorClicked;
                    _renderBackgroundTexture = _backgroundImageClicked;
                    _renderTextColor = _textColorClick;

                    event_MousePress();

                    if (input.Get().IsLeftMouseButtonDown(true))
                    {
                        event_Click();
                    }
                }
            }

            switch (_fontSize)
            {
                case Controls.FontSize.Small:
                    _renderSpriteFont = ContentLibrary.UIFont_Small;
                    break;
                case Controls.FontSize.Smaller:
                    _renderSpriteFont = ContentLibrary.UIFont_Smaller;
                    break;
            }
        }

        private Texture2D _renderBackgroundTexture;
        private Color _renderBackgroundColor;
        private Color _renderTextColor;
        private SpriteFont _renderSpriteFont;

        internal override void Draw(GameTime gameTime)
        {
            if (_renderBackgroundTexture != null)
            {
                ShapeRenderer.DrawRectangle(_renderBackgroundTexture, ControlRectangle, _renderBackgroundColor);
            }

            if (Focused)
                ShapeRenderer.DrawBorder(_borderColorFocused, ClientAreaRectangle, BORDER_WIDTH);

            Vector2 dim = _renderSpriteFont.MeasureString(_text);

            SpriteBatch.Begin();

            Point center = ClientAreaRectangle.Center;

            SpriteBatch.DrawString(_renderSpriteFont, _text, new Vector2(center.X - dim.X / 2f, center.Y - dim.Y / 2f), _renderTextColor);

            SpriteBatch.End();
        }
    }
}
