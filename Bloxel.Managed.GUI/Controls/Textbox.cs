/*
 * Bloxel.Managed.GUI - Textbox.cs
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
using Microsoft.Xna.Framework.Input;

using Bloxel.Managed.GUI.Input;
using input = Bloxel.Managed.GUI.Input.Input;

namespace Bloxel.Managed.GUI.Controls
{
    public class Textbox : Control
    {
        private Vector2 _position;
        private int _width, _height;

        private Color _backgroundColor;
        private Color _borderColorUnfocused;
        private Color _borderColorFocused;
        private Color _textColor;

        private Rectangle _scissorRectangle;
        private RasterizerState _scissorState;

        private string _text;

        private int _caretPos;
        private float _currentOffset;

        public readonly int BORDER_WIDTH = 2;

        /// <summary>
        /// Returns the absolute position, from the top left corner of the rendering surface, of this control. 
        /// </summary>
        public override Vector2 AbsolutePosition { get { return ParentControl.AbsolutePosition + Position; } }
        public override Vector2 Position { get { return _position;} set { _position=value; } }
        public override Vector2 ClientAreaPosition { get { return Position + new Vector2(BORDER_WIDTH, BORDER_WIDTH); } }

        public override int Width { get { return _width; } set { _width = value; } }
        public override int Height { get { return _height; } set { _height = value; } }

        public override Rectangle ControlRectangle { get { return new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, Width + 2 * BORDER_WIDTH, Height + 2 * BORDER_WIDTH); } }
        public override Rectangle ClientAreaRectangle { get { return new Rectangle((int)AbsolutePosition.X + BORDER_WIDTH, (int)AbsolutePosition.Y + BORDER_WIDTH, Width, Height); } }

        public Color BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public Color BorderColorUnfocused { get { return _borderColorUnfocused; } set { _borderColorUnfocused = value; } }
        public Color BorderColorFocused { get { return _borderColorFocused; } set { _borderColorFocused = value; } }
        public Color TextColor { get { return _textColor; } set { _textColor = value; } }

        public string Text { get { return _text; } set { _text = value; } }
        public int CaretPosition { get { return _caretPos; } set { _caretPos = Math.Min(Text.Length, value); } }

        public Textbox()
            : base()
        {
            _position = Vector2.Zero;
            _width = 2;
            _height = 2;

            _text = "";

            _backgroundColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
            _borderColorUnfocused = new Color(0.2f, 0.2f, 0.2f);
            _borderColorFocused = Color.Black;
            _textColor = Color.Black;

            _caretPos = 0;
        }

        internal override void internal_onloaded()
        {
            _scissorRectangle = new Rectangle((int)AbsoluteClientAreaPosition.X, (int)AbsoluteClientAreaPosition.Y, Width, Height);
            _scissorState = new RasterizerState { ScissorTestEnable = true };
        }

        protected override void onFocused()
        {
            KeyboardBuffer.Singleton.Enable();
        }

        protected override void onUnfocused()
        {
            KeyboardBuffer.Singleton.Disable();

            _lastSwitchCaretVisibility = 0;
            _showCaret = false;
        }

        internal override void Update(GameTime gameTime)
        {
            _forceCaretVisible = false;

            if (Focused)
            {
                if (input.Get().IsLeftMouseButtonDown(true))
                {
                    float relativeMouseX = input.Get().MouseXCoordinate() - (AbsoluteClientAreaPosition.X + 2);
                    float wholeStringX = ContentLibrary.UIFont_Small.MeasureString(_text).X - _currentOffset + 2;

                    if (wholeStringX <= relativeMouseX)
                        _caretPos = _text.Length;
                    else
                    {
                        for (int i = _text.Length - 1; i >= 0; --i)
                        {
                            wholeStringX -= ContentLibrary.UIFont_Small.MeasureString(_text[i].ToString()).X;

                            if (wholeStringX <= relativeMouseX)
                            {
                                _caretPos = i;

                                break;
                            }
                        }
                    }
                }

                string buff = KeyboardBuffer.Singleton.GetText();

                if (buff.Length > 0)
                {
                    _text = _text.Insert(_caretPos, buff);
                    _caretPos += buff.Length;

                    _forceCaretVisible = true;
                }

                if (KeyboardBuffer.Singleton.BackSpaceKey)
                {
                    string s1 = _text.Substring(0, _caretPos);

                    if (s1.Length > 0)
                    {
                        s1 = _text.Substring(0, s1.Length - 1);
                        string s2 = _text.Substring(_caretPos, _text.Length - _caretPos);

                        _text = s1 + s2;

                        --_caretPos;
                    }

                    _forceCaretVisible = true;
                }

                if (KeyboardBuffer.Singleton.LeftKey)
                {
                    if (--_caretPos < 0)
                        _caretPos = 0;
                    else
                        _forceCaretVisible = true;
                }

                if (KeyboardBuffer.Singleton.RightKey)
                {
                    if (++_caretPos > _text.Length)
                        _caretPos = _text.Length;
                    else
                        _forceCaretVisible = true;
                }

                Vector2 dim = ContentLibrary.UIFont_Small.MeasureString(_text.Substring(0, _caretPos));
                float caretPosX = 2;
                if (_text.Length < _caretPos + 1)
                    caretPosX += 2f + dim.X;
                else
                {
                    Vector2 dim2 = ContentLibrary.UIFont_Small.MeasureString(_text.Substring(0, _caretPos + 1)) - ContentLibrary.UIFont_Small.MeasureString(_text.Substring(_caretPos, 1));
                    caretPosX += (dim2.X + dim.X) / 2f;
                }

                if (caretPosX - _currentOffset > _width - 3)
                {
                    _currentOffset += caretPosX - _currentOffset - (_width - 3);
                }

                if (caretPosX - _currentOffset < 2)
                {
                    _currentOffset -= 2 - caretPosX + _currentOffset;
                }

                _caretPosition = new Vector2(caretPosX - _currentOffset, (_height - ContentLibrary.UIFont_Small.LineSpacing) / 2f) + AbsoluteClientAreaPosition;
            }

            _scissorRectangle = new Rectangle((int)AbsoluteClientAreaPosition.X, (int)AbsoluteClientAreaPosition.Y, Width, Height);

            if (_scissorRectangle.X < 0)
                _scissorRectangle.X = 0;

            if (_scissorRectangle.Y < 0)
                _scissorRectangle.Y = 0;

            if (_scissorRectangle.Right > GraphicsDevice.Viewport.Width)
                _scissorRectangle.Width = GraphicsDevice.Viewport.Width - _scissorRectangle.X;

            if (_scissorRectangle.Bottom > GraphicsDevice.Viewport.Height)
                _scissorRectangle.Height = GraphicsDevice.Viewport.Height - _scissorRectangle.Y;
        }

        private bool _showCaret = false;
        private int _lastSwitchCaretVisibility = 0;
        private bool _forceCaretVisible = true;
        private Vector2 _caretPosition;

        internal override void Draw(GameTime gameTime)
        {
            ShapeRenderer.DrawRectangle(_backgroundColor, ClientAreaRectangle);
            ShapeRenderer.DrawBorder(Focused ? _borderColorFocused : _borderColorUnfocused, ClientAreaRectangle, BORDER_WIDTH);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _scissorState);

            Rectangle preScissor = GraphicsDevice.ScissorRectangle;

            GraphicsDevice.ScissorRectangle = _scissorRectangle;

            SpriteBatch.DrawString(ContentLibrary.UIFont_Small, _text, AbsoluteClientAreaPosition + new Vector2(2 - _currentOffset, (Height - ContentLibrary.UIFont_Small.LineSpacing) / 2f), TextColor);

            if (Focused)
            {
                if (Environment.TickCount - _lastSwitchCaretVisibility >= 500 || _forceCaretVisible)
                {
                    _lastSwitchCaretVisibility = Environment.TickCount;
                    _showCaret = !_showCaret || _forceCaretVisible;
                }

                if (_showCaret)
                    SpriteBatch.Draw(ContentLibrary.DummyTexture, new Rectangle((int)_caretPosition.X, (int)_caretPosition.Y, 1, ContentLibrary.UIFont_Small.LineSpacing), _textColor);
            }

            GraphicsDevice.ScissorRectangle = preScissor;

            SpriteBatch.End();
        }
    }
}
