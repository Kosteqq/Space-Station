using UnityEngine;

namespace SpaceStation.Core
{
    public abstract class GameController : MonoBehaviour
    {
        public GameManager GameManager { get; private set; }
        
        public virtual void InitializeGame()
        { }
        
        public virtual void StartGame()
        { }
    }
}