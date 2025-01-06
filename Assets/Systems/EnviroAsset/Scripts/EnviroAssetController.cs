using SpaceStation.Core;
using SpaceStation.PathFinding;
using UnityEngine;

namespace SpaceStation.EnviroAsset
{
    [RequireComponent(typeof(PathFindingTarget))]
    public abstract class EnviroAssetController : SystemController<EnviroAssetsManager>
    {
        public bool InUse { get; internal set; }

        public override void InitializeGame()
        {
            base.InitializeGame();
            SystemManager.AddAsset(this);
        }

        private void OnDestroy()
        {
            SystemManager.RemoveAsset(this);
        }
    }
}
