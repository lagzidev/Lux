namespace LuxEngine.ECS
{
    // Features
    public interface IFeature
    {
    }

    public interface IInitFeature
    {
        void Init(Systems systems);
    }

    public interface IUpdateFeature
    {
        void Update(Systems systems);
    }

    public interface IUpdateFixedFeature
    {
        void UpdateFixed(Systems systems);
    }

    public interface IDrawFeature
    {
        void Draw(Systems systems);
    }

    public interface IOnDestroyEntityFeature
    {
        void OnDestroyEntity(Systems systems);
    }

    public interface IOnAddComponentFeature
    {
        void OnAddComponent(Systems systems);
    }

    public interface IOnRemoveComponentFeature
    {
        void OnRemoveComponent(Systems systems);
    }
}
