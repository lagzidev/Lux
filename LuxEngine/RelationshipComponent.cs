using System;
namespace LuxEngine
{
    public class Relationship  : BaseComponent<Relationship>
    {
        public EntityHandle ParentEntity { get; set; }

        public Relationship(EntityHandle parentEntity)
        {
            ParentEntity = parentEntity;
        }
    }
}
