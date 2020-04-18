using LuxEngine.ECS;
using LuxEngine.LuxSystems;

namespace LuxEngine.Features
{
    public class RenderFeature : IFeature, IUpdateFeature
    {
        public void Update(Systems systems)
        {
            systems
                .Add<Transform>(WindSystemEx.Whoosh);
        }
    }
}
