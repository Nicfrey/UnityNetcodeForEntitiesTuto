using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct NetcodePlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (netcodePlayerInput, localTransform) in SystemAPI
                     .Query<RefRO<NetcodePlayerInput>, RefRW<LocalTransform>>().WithAll<Simulate>()) // important to use Simulate
        {
            float3 moveVector = new float3(netcodePlayerInput.ValueRO.inputVector.x,0,netcodePlayerInput.ValueRO.inputVector.y);
            localTransform.ValueRW.Position += moveVector * SystemAPI.Time.DeltaTime * netcodePlayerInput.ValueRO.moveSpeed;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
