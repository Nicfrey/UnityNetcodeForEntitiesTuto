using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct TestNetcodeEntitiesServerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (simpleRpc, receiveRpcCommandRequest, entity)
                 in SystemAPI.Query<RefRO<SimpleRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
        {
            Debug.Log("Received Rpc Command: " + simpleRpc.ValueRO.value + " :: " + receiveRpcCommandRequest.ValueRO.SourceConnection);
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
