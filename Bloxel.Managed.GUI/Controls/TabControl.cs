/*
 * Bloxel.Managed.GUI - TabControl.cs
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
    public class TabControl : Control
    {
        private Vector2 _position;
        private int _width, _height;

        private Color _backgroundUnfocused;
        private Color _backgroundFocused;
        private Color _borderColorUnfocused;
        private Color _borderColorFocused;

        private Texture2D _backgroundTextureUnfocused;
        private Texture2D _backgroundTextureFocused;

        private bool _shouldUpdateUnfocusedPages;

        private List<TabPage> _tabPages;
        private int _currentIndex;

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

        public bool ShouldUpdateUnfocusedPages { get { return _shouldUpdateUnfocusedPages; } set { _shouldUpdateUnfocusedPages = value; } }

        public List<TabPage> TabPages { get { return _tabPages; } }
        public int CurrentIndex { get { return _currentIndex; } set { _currentIndex = Math.Min(_tabPages.Count - 1, value); } }

        private Panel CurrentPanel { get { if (_currentIndex >= _tabPages.Count || _currentIndex < 0) return null; return _tabPages[_currentIndex].Panel; } }

        public TabControl()
            : base()
        {
            _backgroundUnfocused = Color.LightGray;
            _backgroundFocused = Color.White;
            _borderColorFocused = Color.Black;
            _borderColorUnfocused = _backgroundUnfocused;

            _tabPages = new List<TabPage>();
            _currentIndex = 0;
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

        public void AddPage(string name, Panel p)
        {
            Button b = new Button();
            b.Text = name;
            b.Tag = _tabPages.Count;
            b.Width = 50;
            b.Height = 25;
            b.Position = new Vector2((ClientAreaPosition - Position).X + _tabPages.Count * 52, (ClientAreaPosition - Position).Y);
            b.Click += new EventHandler(button_Click);
            b.internal_setguimanager(Manager);
            b.internal_setparent(this);
            b.internal_onloaded();
            b.FontSize = FontSize.Smaller;

            p.Position = new Vector2((ClientAreaPosition - Position).X, (ClientAreaPosition - Position).Y + 25);
            p.Width = ClientAreaRectangle.Width - 1; // 1 because of the border padding TODO make this less annoying
            p.Height = ClientAreaRectangle.Height - 25;
            p.internal_setguimanager(Manager);
            p.internal_setparent(this);
            p.internal_onloaded();

            _tabPages.Add(new TabPage(b, p));
        }

        void button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            _currentIndex = (int)btn.Tag;

            Console.WriteLine("Current tab set to {0}", _currentIndex);
        }

        internal override void Update(GameTime gameTime)
        {
            bool mouseDown = input.Get().IsLeftMouseButtonDown(true) || input.Get().IsRightMouseButtonDown(true);

            if (Focused)
            {
                if (mouseDown)
                {
                    if (CurrentPanel != null)
                    {
                        if (CurrentPanel.ClientAreaRectangle.Contains(input.Get().MouseXCoordinate(), input.Get().MouseYCoordinate()))
                        {
                            if (!CurrentPanel.Focused)
                                CurrentPanel.internal_onfocused();
                        }
                        else
                        {
                            if (CurrentPanel.Focused)
                                CurrentPanel.internal_onunfocused();
                        }
                    }
                }
            }
            else
            {
                if (CurrentPanel != null && CurrentPanel.Focused) CurrentPanel.internal_onunfocused();
                if (_tabPages.Count > 0 && _tabPages[CurrentIndex].Button.Focused) _tabPages[CurrentIndex].Button.internal_onunfocused();
            }

            for (int i = 0; i < _tabPages.Count; i++)
            {
                if (Focused)
                {
                    if (mouseDown)
                    {
                        if (_tabPages[i].Button.ClientAreaRectangle.Contains(input.Get().MouseXCoordinate(), input.Get().MouseYCoordinate()))
                        {
                            if (_currentIndex != i)
                            {
                                _tabPages[_currentIndex].Button.internal_onunfocused();
                                _tabPages[i].Button.internal_onfocused();
                            }
                        }
                        else
                        {
                            if (i == _currentIndex)
                                _tabPages[i].Button.internal_onunfocused();
                        }
                    }
                }

                _tabPages[i].Button.Update(gameTime);

                if (ShouldUpdateUnfocusedPages || _currentIndex == i)
                {
                    _tabPages[i].Panel.Update(gameTime);
                }
            }

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

            for (int i = 0; i < _tabPages.Count; i++)
                _tabPages[i].Button.Draw(gameTime);

            if (CurrentPanel != null)
            {
                CurrentPanel.Draw(gameTime);
            }

            ShapeRenderer.DrawBorder(Focused ? _borderColorFocused : _borderColorUnfocused, ClientAreaRectangle, BORDER_WIDTH);
        }
    }
}
