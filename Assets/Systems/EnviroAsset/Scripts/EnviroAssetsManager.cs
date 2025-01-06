using System.Collections.Generic;
using SpaceStation.Core;
using UnityEngine;

namespace SpaceStation.EnviroAsset
{
    public class EnviroAssetsManager : GameSystemManager
    {
        private readonly List<EnviroAssetController> _assets = new (1024);
        
        public override void Initialize()
        {
            
        }

        internal void AddAsset(EnviroAssetController p_asset)
        {
            _assets.Add(p_asset);
        }

        internal void RemoveAsset(EnviroAssetController p_asset)
        {
            _assets.Remove(p_asset);
        }

        public TAsset GetUnusedAssts<TAsset>()
            where TAsset : EnviroAssetController
        {
            foreach (var asset in _assets)
            {
                if (!asset.InUse && asset is TAsset targetAsset)
                {
                    return targetAsset;
                }
            }
            
            Debug.Log($"Failed to get any unused asset of type {typeof(TAsset).Name}");
            return null;
        }
    }
}