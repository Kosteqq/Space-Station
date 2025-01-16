using System.Collections.Generic;
using System.Linq;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.EnviroAsset
{
    public class EnviroAssetUserController : SystemController<EnviroAssetsManager>
    {
        public List<EnviroAssetController> _usingAssets = new(16);
        
        public EnviroAssetController Asset { get; set; }
        public GameController UserController { get; set; }

        private void OnDestroy()
        {
            Asset.InUse = false;
        }

        public bool TryUseAsset<TAsset>()
            where TAsset : EnviroAssetController
        {
            var assets = SystemManager.GetUnusedAssets<TAsset>();

            if (assets.IsEmpty())
                return false;

            var asset = assets.OrderBy(asset => Vector3.Distance(asset.transform.position, transform.position)).First();
            asset.InUse = true;
            _usingAssets.Add(asset);
            return true;
        }

        public void AddUsage(EnviroAssetController p_asset)
        {
            p_asset.InUse = true;
            _usingAssets.Add(p_asset);
        }

        public void RemoveAllUsage()
        {
            for (var index = _usingAssets.Count - 1; index >= 0; index--)
                RemoveUsage(_usingAssets[index]);
        }

        public void RemoveUsage(EnviroAssetController p_asset)
        {
            if (_usingAssets.Remove(p_asset))
            {
                p_asset.InUse = false;
            }
        }

        public void RemoveUsage<TAsset>()
            where TAsset : EnviroAssetController
        {
            var asset = _usingAssets.FirstOrDefault(asset => asset is TAsset);
            
            if (asset == null)
                Debug.LogError($"User doesnt use \"{typeof(TAsset).Name}\" asset", this);

            asset.InUse = false;
            _usingAssets.Remove(asset);
        }

        public TAsset GetUsingAsset<TAsset>()
            where TAsset : EnviroAssetController
        {
            foreach (var asset in _usingAssets)
            {
                if (asset is TAsset targetAsset)
                {
                    return targetAsset;
                }
            }
            
            Debug.LogError($"There is no asset in using of type {typeof(TAsset).Name} of total count {_usingAssets.Count}", this);
            return null;
        }
    }
}