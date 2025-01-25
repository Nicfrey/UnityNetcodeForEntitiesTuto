using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class NetcodePlayerInputSystem : SystemBase
{
    private InputSystem_Actions _inputs;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireForUpdate<NetworkStreamInGame>();
        RequireForUpdate<NetcodePlayerInput>();
        
        _inputs = new InputSystem_Actions();
        _inputs.Enable();
    }
    

    protected override void OnUpdate()
    {
        foreach (var netcodePlayerInput in SystemAPI.Query<RefRW<NetcodePlayerInput>>().WithAll<GhostOwnerIsLocal>())
        {
            netcodePlayerInput.ValueRW.inputVector = _inputs.Player.Move.ReadValue<Vector2>();
        }
        
    }
}