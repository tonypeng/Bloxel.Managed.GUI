/*
 * Bloxel.Managed.GUI - Control.cs
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
    public abstract class Control
    {
        private Game _gameInstance;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private ShapeRenderer _shapeRenderer;
        private GUIManager _guiManager;
        private GUIContentLibrary _contentLibrary;

        private Control _parentControl;

        private string _name;
        private object _tag;

        private bool _focused;

        public event EventHandler Click;
        public event EventHandler MousePress; // TODO should pass information about which button was pressed

        protected Game Game { get { return _gameInstance; } }
        protected GraphicsDevice GraphicsDevice { get { return _graphicsDevice; } }
        protected SpriteBatch SpriteBatch { get { return _spriteBatch; } }
        protected ShapeRenderer ShapeRenderer { get { return _shapeRenderer; } }
        protected GUIManager Manager { get { return _guiManager; } }
        protected GUIContentLibrary ContentLibrary { get { return _contentLibrary; } }

        protected Control ParentControl { get { return _parentControl; }}

        public bool Focused { get { return _focused; } }

        public abstract Vector2 AbsolutePosition { get; }
        public abstract Vector2 Position { get; set; }
        public abstract Vector2 ClientAreaPosition { get; }
        public Vector2 AbsoluteClientAreaPosition { get { return AbsolutePosition + (ClientAreaPosition - Position); } }

        /// <summary>
        /// Gets or sets the width of the client area of this control.
        /// </summary>
        public abstract int Width { get; set; }
        /// <summary>
        /// Gets or sets the height of the client area of this control.
        /// </summary>
        public abstract int Height { get; set; }

        public string Name { get { return _name; } set { _name = value; } }
        public object Tag { get { return _tag; } set { _tag = value; } }

        /// <summary>
        /// Returns the rectangle covering this control.
        /// <remarks>The position of the rectangle is absolute.  The origin is at the top-left corner of the rendering surface.</remarks>
        /// </summary>
        public abstract Rectangle ControlRectangle { get; }
        /// <summary>
        /// Returns the rectangle covering the client area of this control.
        /// <remarks>The position of the rectangle is absolute.  The origin is at the top-left corner of the rendering surface.</remarks>
        /// </summary>
        public abstract Rectangle ClientAreaRectangle { get; }

        public Control()
        {
            _name = "";

            _focused = false;
        }

        internal void internal_setguimanager(GUIManager guiManager)
        {
            _guiManager = guiManager;
            _contentLibrary = guiManager.ContentLibrary;
            _gameInstance = guiManager.Game;
            _graphicsDevice = _gameInstance.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _shapeRenderer = guiManager.ShapeRenderer;
        }

        internal void internal_setparent(Control parent)
        {
            _parentControl = parent;
        }

        internal void internal_onfocused()
        {
            _focused = true;

            onFocused();
        }

        internal void internal_onunfocused()
        {
            _focused = false;

            onUnfocused();
        }

        internal virtual void internal_onloaded() { }

        protected void event_Click()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        protected void event_MousePress()
        {
            if (MousePress != null)
                MousePress(this, EventArgs.Empty);
        }

        protected abstract void onFocused();
        protected abstract void onUnfocused();

        internal abstract void Update(GameTime gameTime);
        internal abstract void Draw(GameTime gameTime);
    }
}
