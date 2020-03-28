using System;
namespace LuxEngine
{
    public class Relationship : BaseComponent<Relationship>
    {
        public EntityHandle ParentEntity { get; set; }

        public Relationship(EntityHandle parentEntity)
        {
            ParentEntity = parentEntity;
        }
    }

    public class RelationshipSystem : BaseSystem<RelationshipSystem>
    {
        public RelationshipSystem() : base(Relationship.ComponentType)
        {
        }

        public override void PreDestroyEntity(Entity entity)
        {
            //var relationship = World.Unpack<Relationship>(entity);
            //relationship.ParentEntity
        }
    }
}
