using UnityEngine;

namespace SpaceStation.Core
{
    public abstract class GameplayController : MonoBehaviour
    {
        public GameplayManager GameplayManager { get; private set; }
        
        public virtual void InitializeGame()
        { }
        
        public virtual void StartGame()
        { }
    }
}