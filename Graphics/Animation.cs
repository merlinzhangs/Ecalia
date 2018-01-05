using CocosSharp;
using Ecalia.Tools;
using reWZ.WZProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecalia.Graphics
{
    /// <summary>
    /// Main animation class (prototype)
    /// </summary>
    public class Animation : CCAnimation
    {
        public float Delay { get; set; }
        public CCAnimate animate;

        /// <summary>
        /// Handles animation
        /// </summary>
        public Animation()
            : base ()
        {
            animate = new CCAnimate(this);
        }

        /// <summary>
        /// Adds a sprite to a list of frame used for animation
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="delay"></param>
        public void AddFrame(CCSprite sprite, float delay)
        {
            AddSpriteFrame(sprite);
            //DelayPerUnit = delay;
        }

        public float GetDelay(WZObject wz)
        {
            if (wz.HasChild("delay"))
                return Delay = DataTool.GetInt(wz["delay"]);
            else
                return 0;
        }
    }
}
