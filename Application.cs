using Ecalia.Game;
using SFML.Graphics;
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

        public Application(string title = "window", uint width = 800, uint height = 600)
            : base(new VideoMode(width, height), title)
        {
            window = this;
        }

        /// <summary>
        /// Initializes the window
        /// </summary>
        public void Init()
        {
            
            //SetActive(false);
            //Thread render = new Thread(new ThreadStart(OnRender));
            //render.Start();
            //render.Join();

            InitNetwork(); // TODO: Multi-Thread this
            InitEvents();
            InitOpenGL();

            while (IsOpen)
            {
                WaitAndDispatchEvents();
                if (PollEvent(out var @event))
                {
                    switch(@event.Type)
                    {
                        case EventType.Closed:
                            OnWindowClosed();
                            break;
                    }
                    Clear(Color.Cyan);
                    Draw();
                    Display();
                    Update();
                }
            }
        }

        /// <summary>
        /// Rendering on a different thread
        /// </summary>
        private void OnRender()
        {
            while (IsOpen)
            {
                Clear(Color.Cyan);
                Draw();
                Display();
                Update();
            }
        }

        private void InitNetwork()
        {
        }

        private void InitEvents()
        {
            Closed += Window_Closed;
        }

        private void InitOpenGL()
        {
            SetActive();
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
