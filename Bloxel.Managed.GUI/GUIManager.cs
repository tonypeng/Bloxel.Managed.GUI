/*
 * Bloxel.Managed.GUI - GUIManager.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Bloxel.Managed.GUI.Controls;
using Bloxel.Managed.GUI.Input;
using input = Bloxel.Managed.GUI.Input.Input;

namespace Bloxel.Managed.GUI
{
    public class GUIManager
    {
        private Game _game;

        // out of convenience; so "_game." doesn't have to prefix each reference...
        private ContentManager _content;
        private GraphicsDevice _device;

        private SpriteBatch _spriteBatch;

        private bool _loaded;

        private GUIContentLibrary _contentLibrary;

        private ShapeRenderer _shapeRenderer;

        // TODO: add support for multiple root controls
        private RootControl _rootControl;

        public Game Game { get { return _game; } }

        public GUIContentLibrary ContentLibrary { get { return _contentLibrary; } }
        public ShapeRenderer ShapeRenderer { get { return _shapeRenderer; } }

        public GUIManager(Game game)
        {
            _game = game;

            KeyboardBuffer.Singleton = new KeyboardBuffer(game.Window.Handle);

            _loaded = false; // just to be explicit

            input.Create(game);
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            _device = _game.GraphicsDevice;
            _content = _game.Content;

            _spriteBatch = new SpriteBatch(_device);

            _contentLibrary = new GUIContentLibrary(_device);
            _contentLibrary.LoadContent(_content);

            _shapeRenderer = new GUI.ShapeRenderer(_device, _contentLibrary);

            _loaded = true;

            _rootControl = new RootControl();
            _rootControl.internal_setguimanager(this);
            _rootControl.internal_onfocused();
        }

        public void UnloadContent()
        {
            input.Destroy();
        }

        public void AddControl(Control control)
        {
            _rootControl.AddControl(control);
        }

        public void RemoveControl(Control control)
        {
            _rootControl.RemoveControl(control);
        }

        public void RemoveAllControlsWithName(string str)
        {
            _rootControl.RemoveAllControlsWithName(str);
        }

        public void Update(GameTime gameTime)
        {
            input.Get().Update();

            _rootControl.Update(gameTime);   
        }

        public void Draw(GameTime gameTime)
        {
            _rootControl.Draw(gameTime);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_contentLibrary.MouseCursor, new Vector2(input.Get().MouseXCoordinate(), input.Get().MouseYCoordinate()), Color.White);

            _spriteBatch.End();
        }
    }
}
