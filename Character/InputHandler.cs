using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

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
                case Keyboard.Key.Left:
                    x -= 100;
                    player.Move(new SFML.System.Vector2f(x, y));
                    break;
                case Keyboard.Key.Right:
                    x += 100;
                    player.Move(new SFML.System.Vector2f(x, y));

                    break;
                case Keyboard.Key.Up:
                    break;
                case Keyboard.Key.Down:
                    break;
            }
        }
    }
}
