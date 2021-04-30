using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    public class DynamicInfoMessage : GameObject
    {
        public override bool IsActive { get; set; }
        private Func<string> obtainMessageVia;
        private int x;
        private int y;
        /// <summary>
        /// Creates an info message that will update with the loop
        /// </summary>
        /// <param name="obtainMessageVia"> the function passed that will create the message</param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public DynamicInfoMessage(Func<string> obtainMessageVia,int startX,int startY)
        {
            this.obtainMessageVia = obtainMessageVia;
            this.x = startX;
            this.y = startY;
        }
        //todo add a way to make the message different colors
        public override void Update(GameTime gameTime)
        {
            var message = obtainMessageVia();
           this.Sprite.DrawMap = Helpers.ToText(message, x, y, ConsoleColor.Black);
        
        }
        public override void Draw(GameTime gameTime)
        {
            DrawEngine.QueueSpriteForDraw(this.Sprite);
        }
    }
}
