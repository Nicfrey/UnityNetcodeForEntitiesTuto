# Netcode for entities

This project is a tutorial project on how to make a netcode for entities unity project. It is made using Unity 6.

## Packages

To start a simple project, we need to install the package `Netcode for entities`, `Entities for graphics` and `Multiplayer Play Mode`.

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

This is only working in the editor. Not actually in game.

## How to iterate

The simple way to iterate your feature while making a multiplayer game is normally to create a build every single time. The build you make will only be set up in `Client` mode. this can be changed like this:

`Project Settings` -> `Multiplayer/Build` -> `Client` -> `NetCode Client Target` 

But we can use something else as well. We can use the package `Multiplayer Play Mode`. 

Create a new window called `Multiplayer Play Mode` and activate one player and it will have a new window appearing that looks like the editor. Then you can select the layout `PlayMode tools` to select if this window is a client, client and server or just the server.

## Synchronise Data

