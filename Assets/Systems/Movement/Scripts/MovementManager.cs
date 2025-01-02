using SpaceStation.Core;
using UnityEngine;

namespace SpaceStation.Movement
{
    public class MovementManager : GameSystemManager
    {
        [SerializeField]
        private float _defaultSpeed;
        
        [SerializeField]
        private float _defaultStopDistance;
        
        public float DefaultSpeed => _defaultSpeed;
        public float DefaultStopDistance => _defaultStopDistance;
        
        public override void Initialize()
        {
            
        }
    }
}