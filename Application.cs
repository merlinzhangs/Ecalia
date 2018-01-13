﻿using Ecalia.Character;
using Ecalia.Game;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecalia
{
    /// <summary>
    /// Main Application class
    /// </summary>
    public class Application : RenderWindow, IDisposable
    {
        private static RenderWindow window;
        private Map map = new Map();
        private InputHandler input;
        private View view = new View(new Vector2f(0, 300), new Vector2f(800, 600));
        private RectangleShape random = new RectangleShape(new Vector2f(100, 100));

        public Application(string title = "window", uint width = 800, uint height = 600)
            : base(new VideoMode(width, height), title)
        {
            window = this;
            input = new InputHandler();
        }

        /// <summary>
        /// Initializes the window
        /// </summary>
        public void Init()
        {
            //InitNetwork(); // TODO: Multi-Thread this
            InitEvents();
            //InitOpenGL();
            OnRender(); // Game Loop
        }

        /// <summary>
        /// Game loop
        /// </summary>
        private void OnRender()
        {
            while (IsOpen)
            {
                WaitAndDispatchEvents();
                Clear(Color.Cyan);
                Draw();
                Display();
                Update();
            }
        }

        /// <summary>
        /// Initializes the network
        /// </summary>
        private void InitNetwork()
        {

        }

        private void InitEvents()
        {
            Closed += Window_Closed;
            KeyPressed += Application_KeyPressed;
            KeyReleased += Application_KeyReleased;
        }

        private void Application_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void Application_KeyPressed(object sender, KeyEventArgs e)
        {
            input.HandleInput(e, view);
        }

        private void InitOpenGL()
        {
            SetActive(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Close();
        }

        protected virtual void Draw()
        {
            map.OnLoad();
        }

        protected virtual void Update()
        {
            //view.Center = random.Position;
            SetView(view);
            //Draw(random);
        }

        private void OnWindowClosed()
        {
            Close();
        }

        public static RenderWindow Window
        {
            get { return window; }
        }
    }
}
