using System.Collections.Generic;
using PurrNet.StateMachine;
using UnityEngine;
using PurrNet;

public class RoundRunningState : StateNode<List<PlayerHealth>> 
{
    private List<PlayerID> _players = new();

    public override void Enter(List<PlayerHealth> data, bool asServer) //var type var name
    {
        base.Enter(data, asServer);

        if (!asServer)
            return;
        _players.Clear();
        
        foreach (var player in data)
        {
            if(player.owner.HasValue)
                _players.Add(player.owner.Value);
            player.onDeath_Server += OnPlayerDeath;
        }
    }

    private void OnPlayerDeath(PlayerID deadPlayer)
    {
        _players.Remove(deadPlayer);


        if (_players.Count <= 1)
        {
            if(_players.Count == 1)
                machine.Next(_players[0]);
            else
                machine.Next();
        }
    }
}
