using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrickBreaker.GameObjects;
using ConsoleGameFrameWork;

namespace BrickBreaker
{
    /// <summary>
    /// static class to hold information about all active game objects 
    /// </summary>
    public static class GameObjectCollection
    {
        //we don't want to let people update the list from outside this class.
        //todo look into using a "lookup" object instead.
        public static IList<GameObject> Collection => collection.AsReadOnly();
        public static List<string> ActivePowerUps { get; } = new List<string>();
       
        //does this list size make sense? 
        static List<GameObject> collection = new List<GameObject>(200);

        //hold objects in a qeue to add after we have finished updating our list.
        //we don't want to add/remove a list we are iterating through. 
        //Is this better than doing list.ToArray() every loop?
        static Queue<GameObject> gameObjectsQueue = new Queue<GameObject>();    
        public static void Initialize(IEnumerable<GameObject> initialCollection)
        {   
            collection.Clear();
            gameObjectsQueue.Clear();
            ActivePowerUps.Clear();
            collection.AddRange(initialCollection);
             
        }
        /// <summary>
        /// Inject an object into  GameObjectCollection on the next loop.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Inject(GameObject gameObject)
        {
            if (gameObject != null &&!collection.Contains(gameObject))
            {
                gameObjectsQueue.Enqueue(gameObject);
            }
        }
        /// <summary>
        /// Inject an a list of objects into  GameObjectCollection on the next loop.
        /// </summary>
        /// <param name="gameObjects"></param>
        public static void Inject(IEnumerable<GameObject> gameObjects)
        {
            foreach(var gameObject in gameObjects)
            {
                if (!collection.Contains(gameObject))
                {
                    gameObjectsQueue.Enqueue(gameObject);
                }
            }
        }
        /// <summary>
        /// Add all the pending game objects into the collection.
        /// </summary>
        public static void ProcessPending()
        {
            while (gameObjectsQueue.Any())
            {
                var item = gameObjectsQueue.Dequeue();
                if(item!=null)
                {
                    collection.Add(item);
                }
            }
        }
        /// <summary>
        /// Removes all inactive InteractiveGameObects from the collection
        /// </summary>
        public static void DestroyInactive()
        {
            collection.RemoveAll(x => x is InteractiveGameObject && !x.IsActive);
        }
        public static int CountActive<T>()
        {
            return collection.Count(x=>x is T && x.IsActive);
        }
        public static int CountInactive<T>()
        {
            return collection.Count(x => x is T && !x.IsActive);
        }
        public static List<T> GetActive<T>() where T: GameObject
        {
            return collection.OfType<T>().Where(x=>x.IsActive).ToList();
        }
        public static List<T> GetActive<T>(Func<T,bool> condition) where T : GameObject
        {
            return collection.OfType<T>().Where(x => x.IsActive && condition(x) ).ToList();
        }

        /// <summary>
        /// Check all active objects to see if they collide with an other.
        /// </summary>
        /// <param name="gameTime"></param>
        public static void CheckCollisions(GameTime gameTime)
        {
            foreach(var gameObject in collection)
            {
                if(gameObject is InteractiveGameObject interactiveGameObject)
                {
                    //We don't bother checking an object if its not moving.
                    if (interactiveGameObject.IsActive && interactiveGameObject.Speed>0)
                    {
                        interactiveGameObject.CheckForCollisions(gameTime);
                    }
                }
            }
        }
        public static void UpdateCollection(GameTime gameTime)
        {
            foreach(var gameObject in collection)
            {
                if(gameObject.IsActive)
                {
                    gameObject.Update(gameTime);
                }
            }
        }
        public static void DrawCollection(GameTime gameTime)
        {           
            foreach (var gameObject in collection)
            {
                if (gameObject.IsActive)
                {
                    gameObject.Draw(gameTime);
                    
                }
            }      
        }
    }
    
}
