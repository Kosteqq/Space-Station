using System;
using SpaceStation.Core;
using Unity.Behavior;
using Unity.Properties;

namespace SpaceStation.AI
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "CacheTaskController",
        story: "Cache [target] [controller]",
        category: "Action",
        id: "5d480582bb77ead7bcd150cd2ea1fcd0")]
    public class CacheTaskControllerAction : CacheControllerAction<TaskController>
    { }
}
