using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    class InfoMessage : GameObject
    {
        public override bool IsActive { get; set; }
        public string MessageId {get;} //this seems terrible. Look into using the Id of the sprite instead.
        public TimeSpan? DisplayDuration { get; set; }
        public TimeSpan DisplayTime { get; set; }
       
        public InfoMessage(string message)
        {
            this.MessageId = message;   
        }
         
        public override void Update(GameTime gameTime)
        {
            if(DisplayDuration != null)
            {
                if (gameTime.TotalElapsedTime - DisplayTime > DisplayDuration)
                {
                    this.IsActive = false;
                }
            }       
        }
        public override void Draw(GameTime gameTime)
        {
            DrawEngine.QueueSpriteForDraw(this.Sprite);
        }
        public override bool Equals(object obj)
        {
            return obj !=null && obj is InfoMessage otherMessage && otherMessage.Id == this.Id && otherMessage.MessageId == this.MessageId;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id, this.MessageId);
        }
    }
}
