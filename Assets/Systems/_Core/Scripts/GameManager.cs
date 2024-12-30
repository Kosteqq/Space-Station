using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SpaceStation.Core
{
    public class GameManager : MonoBehaviour
    {
        private GameSystemManager[] _systemManagers;
        private bool _initializedSystems;
        private bool _gameStart;
        
        private void Start()
        {
            InitializeSystems();
            
            var gameplayControllers = FindObjectsByType<GameController>(FindObjectsSortMode.None);
            
            InitializeControllers(gameplayControllers);
            StartControllers(gameplayControllers);

            _gameStart = true;
        }

        public TSystem GetSystem<TSystem>()
            where TSystem : GameSystemManager
        {
            return _systemManagers.First(system => system is TSystem) as TSystem;
        }

        public GameObject InstantiatePrefab(GameObject p_prefab)
        {
            var instance = Instantiate(p_prefab);

            var controllers = instance.GetComponentsInChildren<GameController>();

            if (_initializedSystems)
            {
                InitializeControllers(controllers);
            }

            if (_gameStart)
            {
                StartControllers(controllers);
            }
            
            return instance;
        }

        private void InitializeSystems()
        {
            _systemManagers = FindObjectsByType<GameSystemManager>(FindObjectsSortMode.None);

            foreach (var manager in _systemManagers)
            {
                manager.Initialize();
            }

            _initializedSystems = true;
        }

        private void InitializeControllers(GameController[] p_controllers)
        {
            foreach (var controller in p_controllers)
            {
                SetupControllerManager(controller);
                controller.InitializeGame();
            }
        }

        private void StartControllers(GameController[] p_controllers)
        {
            foreach (var controller in p_controllers)
            {
                controller.StartGame();
            }
        }

        private void SetupControllerManager(GameController p_controllers)
        {
            var type = typeof(GameController);
            var property = type.GetProperty(nameof(GameController.GameManager), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            property.SetValue(p_controllers, this);
        }
    }
}