/*
 * Bloxel.Managed.GUI - ContainerControl.cs
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

using Bloxel.Managed.GUI.Input;
using input = Bloxel.Managed.GUI.Input.Input;

namespace Bloxel.Managed.GUI.Controls
{
    public abstract class ContainerControl : Control
    {
        private List<Control> _childControls;
        private Control _currentFocusedControl;

        public List<Control> ChildControls { get { return _childControls; } }
        protected Control CurrentFocusedControl { get { return _currentFocusedControl; } }

        public ContainerControl()
            : base()
        {
            _childControls = new List<Control>();
        }

        internal virtual void UpdateChildControls(GameTime gameTime)
        {
            bool focusedAlreadySet = false;
            bool mousePressed = input.Get().IsLeftMouseButtonDown(true) || input.Get().IsRightMouseButtonDown(true);

            for (int i = 0; i < _childControls.Count; i++)
            {
                if (!focusedAlreadySet)
                {
                    if (mousePressed)
                    {
                        if (_childControls[i].ControlRectangle.Contains(input.Get().MouseXCoordinate(), input.Get().MouseYCoordinate()))
                        {
                            if (_currentFocusedControl != null && _currentFocusedControl != _childControls[i])
                                _currentFocusedControl.internal_onunfocused();

                            if (_currentFocusedControl != _childControls[i])
                            {
                                _currentFocusedControl = _childControls[i];
                                _currentFocusedControl.internal_onfocused();
                            }

                            focusedAlreadySet = true;
                        }
                        else
                        {
                            if (_currentFocusedControl == _childControls[i])
                            {
                                _currentFocusedControl.internal_onunfocused();
                                _currentFocusedControl = null;
                            }
                        }
                    }
                }

                _childControls[i].Update(gameTime);
            }
        }

        public virtual void AddControl(Control control)
        {
            control.internal_setguimanager(Manager);
            control.internal_setparent(this);

            control.internal_onloaded();

            if (!_childControls.Contains(control))
                _childControls.Add(control);
        }

        public virtual void RemoveControl(Control control)
        {
            _childControls.Remove(control);
        }

        public virtual void RemoveAllControlsWithName(string str)
        {
            for (int i = 0; i < _childControls.Count; i++)
            {
                if (_childControls[i].Name == str)
                    _childControls.RemoveAt(i);
            }
        }
    }
}
