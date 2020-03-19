using System;
namespace LuxEngine
{
    public class RelationshipComponent : BaseComponent<RelationshipComponent>
    {
        public EntityHandle ParentEntity { get; set; }

        public RelationshipComponent(EntityHandle parentEntity)
        {
            ParentEntity = parentEntity;
        }
    }
}
