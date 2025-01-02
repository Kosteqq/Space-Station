using UnityEngine;

namespace SpaceStation.AI
{
    public interface ITaskDispatcher
    {
        Task GetTask();
    }
}