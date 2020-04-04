using System;
namespace LuxEngine
{
    [Serializable]
    public class Parent : BaseComponent<Parent>
    {
        public Entity ParentEntity { get; set; }

        public Parent(Entity parentEntity)
        {
            ParentEntity = parentEntity;
        }
    }

    public class ParentSystem : BaseSystem<ParentSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Parent>();
        }

        protected override void OnDestroyEntity(Entity destroyedEntity)
        {
            // Foreach entity that has a parent
            foreach (var entity in RegisteredEntities)
            {
                // If the entity's parent is the destroyed entity
                var child = World.Unpack<Parent>(entity);
                if (destroyedEntity == child.ParentEntity)
                {
                    // Remove its child component
                    World.RemoveComponent<Parent>(entity);
                }
            }
        }
    }
}
