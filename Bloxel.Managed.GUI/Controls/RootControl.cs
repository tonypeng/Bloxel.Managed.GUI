/*
 * Bloxel.Managed.GUI - RootControl.cs
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
    public class RootControl : ContainerControl
    {
        private Vector2 _absolutePosition;

        private Color _backgroundUnfocused;
        private Color _backgroundFocused;

        /// <summary>
        /// Returns the absolute position, from the top left corner of the rendering surface, of this control. 
        /// <remarks>Because RootControl must be the highest control possible, its absolute position does not depend on another control.</remarks>
        /// </summary>
        public override Vector2 AbsolutePosition { get { return _absolutePosition; } }
        public override Vector2 Position { get { return AbsolutePosition; } set { throw new NotSupportedException("Modifying the position of a RootControl is currently not supported."); } }
        public override Vector2 ClientAreaPosition { get { return Position; } }

        public override int Width { get { return GraphicsDevice.Viewport.Width; } set { throw new NotSupportedException("Modifying the width of a RootControl is currently not supported."); } }
        public override int Height { get { return GraphicsDevice.Viewport.Height; } set { throw new NotSupportedException("Modifying the height of a RootControl is currently not supported."); } }

        public override Rectangle ControlRectangle { get { return new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, Width, Height); } }
        public override Rectangle ClientAreaRectangle { get { return ControlRectangle; } }

        public Color BackgroundUnfocusedColor { get { return _backgroundUnfocused; } set { _backgroundUnfocused = value; } }
        public Color BackgroundFocusedColor { get { return _backgroundFocused; } set { _backgroundFocused = value; } }

        public RootControl ()
            : base()
        {
            _backgroundUnfocused = Color.Transparent;
            _backgroundFocused = new Color(0, 0, 0, 0.5f);
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
        }

        internal override void Draw(GameTime gameTime)
        {
            ShapeRenderer.DrawRectangle(Focused ? _backgroundFocused : _backgroundUnfocused, ControlRectangle);

            for (int i = 0; i < ChildControls.Count; i++)
                ChildControls[i].Draw(gameTime);
        }
    }
}
