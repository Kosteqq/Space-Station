using System.Collections.Generic;
using SpaceStation.Core;
using UnityEngine;

namespace SpaceStation.EnviroAsset
{
    public class EnviroAssetUserController : SystemController<EnviroAssetsManager>
    {
        public  List<EnviroAssetController> _usingAssets = new(16);
        
        public EnviroAssetController Asset { get; set; }
        public GameController UserController { get; set; }

        private void OnDestroy()
        {
            Asset.InUse = false;
        }

        public void AddUsage(EnviroAssetController p_asset)
        {
            p_asset.InUse = true;
            _usingAssets.Add(p_asset);
        }

        public void RemoveUsage(EnviroAssetController p_asset)
        {
            if (_usingAssets.Remove(p_asset))
            {
                p_asset.InUse = false;
            }
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
            
            Debug.LogError($"There is no asset in using of type {typeof(TAsset).Name} of total count {_usingAssets.Count}");
            return null;
        }
    }
}