using System.Collections.Generic;
using System.Linq;
using SpaceStation.Core;
using SpaceStation.Utils;
using UnityEngine;

namespace SpaceStation.EnviroAsset
{
    public class EnviroAssetUserController : SystemController<EnviroAssetsManager>
    {
        public List<EnviroAssetController> _owningAssets = new(16);
        
        public GameController UserController { get; set; }

        private void OnDestroy()
        {
            foreach (var asset in _owningAssets)
            {
                asset.InUse = false;
            }
        }

        public bool IsOwningAsset<TAsset>()
            where TAsset : EnviroAssetController
        {
            return _owningAssets.Any(asset => asset is TAsset);
        }

        public bool TryOwnAsset<TAsset>()
            where TAsset : EnviroAssetController
        {
            var assets = SystemManager.GetUnusedAssets<TAsset>();

            if (assets.IsEmpty())
                return false;

            var asset = assets.OrderBy(asset => Vector3.Distance(asset.transform.position, transform.position)).First();
            asset.InUse = true;
            _owningAssets.Add(asset);
            return true;
        }

        public void AddOwning(EnviroAssetController p_asset)
        {
            p_asset.InUse = true;
            _owningAssets.Add(p_asset);
        }

        public void RemoveAllOwnings()
        {
            for (var index = _owningAssets.Count - 1; index >= 0; index--)
                RemoveOwn(_owningAssets[index]);
        }

        public void RemoveOwn(EnviroAssetController p_asset)
        {
            if (_owningAssets.Remove(p_asset))
            {
                p_asset.InUse = false;
            }
        }

        public void RemoveOwn<TAsset>()
            where TAsset : EnviroAssetController
        {
            var asset = _owningAssets.FirstOrDefault(asset => asset is TAsset);
            
            if (asset == null)
                Debug.LogError($"User doesnt own \"{typeof(TAsset).Name}\" asset", this);

            asset.InUse = false;
            _owningAssets.Remove(asset);
        }

        public TAsset GetOwninggAsset<TAsset>()
            where TAsset : EnviroAssetController
        {
            foreach (var asset in _owningAssets)
            {
                if (asset is TAsset targetAsset)
                {
                    return targetAsset;
                }
            }
            
            Debug.LogError($"There is no owning asset of type {typeof(TAsset).Name} of total count {_owningAssets.Count}", this);
            return null;
        }
    }
}