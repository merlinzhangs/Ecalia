using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace Ecalia.Character
{
    public class InputHandler
    {
        internal void HandleInput(KeyEventArgs e, View player)
        {
            var x = 0;
            var y = 0;
            switch (e.Code)
            {
                case Key.Left:
                    x -= 100;
                    player.Move(new SFML.System.Vector2f(x, y));
                    break;
                case Key.Right:
                    x += 100;
                    player.Move(new SFML.System.Vector2f(x, y));

                    break;
                case Key.Up:
                    break;
                case Key.Down:
                    break;
            }
        }
    }
}
