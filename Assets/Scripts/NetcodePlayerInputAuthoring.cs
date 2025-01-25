using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetcodePlayerInputAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    private class Baker : Baker<NetcodePlayerInputAuthoring>
    {
        public override void Bake(NetcodePlayerInputAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new NetcodePlayerInput
            {
                moveSpeed = authoring.moveSpeed,
            });
        }
    }
}

public struct NetcodePlayerInput : IInputComponentData
{
    public float2 inputVector;
    public float moveSpeed;
}
