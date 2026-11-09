using System.Collections;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

public class WaitForPlayersState : StateNode
{
    [SerializeField] private int minPlayers = 2;

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;

        ShowWaitingMessage();

        // Keep waiting until enough players have connected to start the round.
        StartCoroutine(nameof(WaitForPlayers));
    }

    [ObserversRpc(runLocally: true)]
    private void ShowWaitingMessage()
    {
        RoundAnnouncementUI.Present("WAITING FOR PLAYERS", "Waiting for more players to join", string.Empty, 0f);
    }
    
    private IEnumerator WaitForPlayers()
    {
        while (networkManager.players.Count < minPlayers)
            yield return null;

        machine.Next();
    }
}
