using System;
namespace LuxEngine
{
    public class Animation : BaseComponent<Animation>
    {
        public string CurrentAnimation;

        public Animation(string currentAnimation)
        {
            CurrentAnimation = currentAnimation;
        }
    }

    public class AnimationSystem : BaseSystem<AnimationSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Animation>();
        }
    }
}
