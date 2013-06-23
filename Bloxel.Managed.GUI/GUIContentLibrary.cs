/*
 * Bloxel.Managed.GUI - GUIContentLibrary.cs
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

namespace Bloxel.Managed.GUI
{
    public class GUIContentLibrary
    {
        private GraphicsDevice _device;

        private ContentManager _content;

        private Dictionary<string, object> _customAssets;

        // Textures
        public Texture2D DummyTexture { get; private set; }
        public Texture2D MouseCursor { get; private set; }

        // Fonts
        public SpriteFont UIFont_Small { get; private set; }

        public ContentManager Content { get { return _content; } }

        public GUIContentLibrary(GraphicsDevice device)
        {
            _device = device;

            _customAssets = new Dictionary<string, object>();
        }

        public void LoadContent(ContentManager content)
        {
            _content = content;

            DummyTexture = new Texture2D(_device, 1, 1);
            DummyTexture.SetData<Color>(new Color[] { Color.White });
            MouseCursor = Content.Load<Texture2D>("GUIContent/Textures/cursor");

            UIFont_Small = Content.Load<SpriteFont>("GUIContent/Fonts/UIFont_Small");
            UIFont_Small.DefaultCharacter = '?';
        }

        public void StoreCustomAsset(string id, object asset)
        {
            _customAssets.Add(id, asset);
        }

        public T GetCustomAsset<T>(string id)
        {
            return (T)_customAssets[id];
        }
    }
}
