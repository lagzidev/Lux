using System;
namespace LuxEngine
{
    [Serializable]
    public class Animation : BaseComponent<Animation>
    {
        public string Name;

    }

    public class AnimationSystem : BaseSystem<AnimationSystem>
    {
        public AnimationSystem() : base()
        {
        }
    }
}
