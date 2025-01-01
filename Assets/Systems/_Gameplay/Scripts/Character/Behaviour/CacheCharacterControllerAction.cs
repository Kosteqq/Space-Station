using System;
using SpaceStation.Core;
using Unity.Behavior;
using Unity.Properties;

namespace SpaceStation.Gameplay.Character
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Cache Character Controller",
        story: "Cache [target] [controller]",
        category: "Action",
        id: "9884dd2fbd5f23971f6b37d42f3a0f04")]
    public sealed partial class CacheCharacterControllerAction : CacheControllerAction<CharacterController>
    {
    }
}
