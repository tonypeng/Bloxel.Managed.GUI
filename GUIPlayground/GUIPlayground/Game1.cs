using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Bloxel.Managed.GUI;
using Bloxel.Managed.GUI.Controls;

namespace GUIPlayground
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GUIManager _guiManager;

        FPSCounter fpsCounter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _guiManager = new GUIManager(this);
        }

        protected override void Initialize()
        {
            _guiManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _guiManager.LoadContent();
            Textbox textbox = new Textbox();
            textbox.Name = "Test Textbox";
            textbox.Position = new Vector2(10, 10);
            textbox.Width = 200;
            textbox.Height = 25;
            textbox.Text = "Hello, world!";
            _guiManager.AddControl(textbox);
            Button button = new Button();
            button.Text = "btn!";
            button.Position = new Vector2(220, 10);
            button.Width = 100;
            button.Height = 100;
            button.Click += new EventHandler(button_Click);
            button.MousePress += new EventHandler(button_MousePress);
            _guiManager.AddControl(button);
            Panel panel = new Panel();
            panel.Position = new Vector2(330, 10);
            panel.Width = 200;
            panel.Height = 200;
            _guiManager.AddControl(panel);
            Button panel_Button = new Button();
            panel_Button.Text = "child btn!";
            panel_Button.Position = Vector2.Zero;
            panel_Button.Width = 100;
            panel_Button.Height = 100;
            panel_Button.MousePress += new EventHandler(button_MousePress);
            panel_Button.Click += new EventHandler(button_Click);
            panel.AddControl(panel_Button);
            TabControl tabControl = new TabControl();
            tabControl.Position = new Vector2(10, 120);
            tabControl.Width = 300;
            tabControl.Height = 300;
            _guiManager.AddControl(tabControl);
            Panel tabPage1 = new Panel();
            Button tabPage1_Button = new Button();
            tabPage1_Button.Text = "child btn! (1)";
            tabPage1_Button.Position = Vector2.Zero;
            tabPage1_Button.Width = 100;
            tabPage1_Button.Height = 100;
            tabPage1_Button.MousePress += new EventHandler(button_MousePress);
            tabPage1_Button.Click += new EventHandler(button_Click);
            Panel tabPage2 = new Panel();
            Button tabPage2_Button = new Button();
            tabPage2_Button.Text = "child btn! (2)";
            tabPage2_Button.Position = Vector2.Zero;
            tabPage2_Button.Width = 100;
            tabPage2_Button.Height = 100;
            tabPage2_Button.MousePress += new EventHandler(button_MousePress);
            tabPage2_Button.Click += new EventHandler(button_Click);
            tabControl.AddPage("test", tabPage1);
            tabControl.AddPage("test2", tabPage2);
            tabPage1.AddControl(tabPage1_Button);
            tabPage2.AddControl(tabPage2_Button);

            fpsCounter = new FPSCounter(this, _guiManager.ContentLibrary.UIFont_Small);
            fpsCounter.LoadContent();
        }

        void button_MousePress(object sender, EventArgs e)
        {
            Console.WriteLine("button press from {0}!", ((Button)sender).Text);
        }

        void button_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button click from {0}!", ((Button)sender).Text);
        }

        protected override void UnloadContent()
        {
            _guiManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _guiManager.Update(gameTime);

            fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _guiManager.Draw(gameTime);

            fpsCounter.Draw();

            base.Draw(gameTime);
        }
    }
}
