using System;
namespace LuxEngine
{
    [Serializable]
    public class Parent : AComponent<Parent>
    {
        public Entity ParentEntity { get; set; }

        public Parent(Entity parentEntity)
        {
            ParentEntity = parentEntity;
        }
    }

    public class ParentSystem : ASystem<ParentSystem>
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
                Unpack(entity, out Parent parent);
                if (destroyedEntity == parent.ParentEntity)
                {
                    // Remove its child component
                    _world.RemoveComponent<Parent>(entity);
                }
            }
        }
    }
}
