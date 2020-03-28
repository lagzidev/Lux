using System;
namespace LuxEngine
{
    public class Parent : BaseComponent<Parent>
    {
        public EntityHandle ParentEntity { get; set; }

        public Parent(EntityHandle parentEntity)
        {
            ParentEntity = parentEntity;
        }
    }

    public class ParentSystem : BaseSystem<ParentSystem>
    {
        public ParentSystem() : base(Parent.ComponentType)
        {
        }

        public override void PreDestroyEntity(Entity destroyedEntity)
        {
            // Foreach entity that has a parent
            foreach (var entity in RegisteredEntities)
            {
                // If the entity's parent is the destroyed entity
                var child = World.Unpack<Parent>(entity);
                if (destroyedEntity == child.ParentEntity.Entity)
                {
                    // Remove its child component
                    World.RemoveComponent<Parent>(entity);
                }
            }
        }
    }
}
