# Netcode for entities

This project is a tutorial project on how to make a netcode for entities unity project. It is made using Unity 6.

## Packages

To start a simple project, we need to install the package `Netcode for entities` and `Entities for graphics`.

Next, we need to change inside of the project settings:
- on the `Editor` section, find `Enter Play Mode Settings` and set `When entering Play mode` to `Do not reload Domain or Scene`.
- on the `Player` section, inside of `Resolution and Presentation`, check the box `Run In Background`.

## RPCs

RPC means 'Remote Procedure Call'. It is used to communicate between processes on different workstations.

A simple RPC in Unity can be written by creating a new `struct` that herits from `IRpcCommand`:

```C#
public struct SimpleRpc : IRpcCommand
{
   public int value;
}
```

### Communication between Client and server 

Communicate between the client and the server is easy. We just need to create two different entities system. One for the **client** and one for the **server**.

In this example, when the player press a button, we send the value 56 from the struct we created earlier and we want to know which entities sent that message.

The client will look like this:

```C#
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct TestNetcodeEntitiesClientSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Send Rpc
            Entity rpcEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(rpcEntity, new SimpleRpc
            {
                value = 56
            });
            state.EntityManager.AddComponentData(rpcEntity, new SendRpcCommandRequest());
            Debug.Log("Sending RPC..."); 
        }
    }
}
```

and the server will look like this:

```C#
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct TestNetcodeEntitiesServerSystem : ISystem
{
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

```