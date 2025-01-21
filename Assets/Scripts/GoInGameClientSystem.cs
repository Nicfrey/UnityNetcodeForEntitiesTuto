using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct GoInGameClientSystem : ISystem
{
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get the entity that wants to connect to the ingame server
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (networkId, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(entity);
            Debug.Log("Setting Client as InGame");
            Entity rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, new GoInGameRequestRpc());
            ecb.AddComponent(rpcEntity, new SendRpcCommandRequest());
        }
        ecb.Playback(state.EntityManager);
    }
}

public struct GoInGameRequestRpc : IRpcCommand
{
    
}
