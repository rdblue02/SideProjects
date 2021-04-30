using BrickBreaker.Events;
using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    public abstract class GameObject
    {
        public Guid Id { get; } 
        public Sprite Sprite { get; set; }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        public abstract bool IsActive { get; set; }
 
       
        public GameObject()
        {
            Id = Guid.NewGuid();
            Sprite = new Sprite();
            Sprite.DrawMap = new List<Pixel>();
            this.Sprite.DrawMap = new List<Pixel>();
        }
              
        public override bool Equals(object obj)
        {
            if(obj is GameObject otherObj)
            {
              return otherObj.Id == this.Id;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
