using System;
namespace LuxEngine
{
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
