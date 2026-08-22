using PurrNet.StateMachine;
using UnityEngine;
using System.Collections;
using PurrNet;
using System.Collections.Generic;
using NUnit.Framework;

public class RoundEndState : StateNode<PlayerID>
{
    [SerializeField] private int amountOfRounds = 3;
    [SerializeField] private StateNode spawningState;

    private int _roundCount = 0;
    private WaitForSeconds _delay = new(3f);

    private Dictionary<PlayerID, int> _roundWins = new();

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;
        
        Debug.Log("Round has ended with no winner!");

        CheckForGameEnd();
    }

    public override void Enter(PlayerID winner, bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;

        if(!_roundWins.ContainsKey(winner))
            _roundWins.Add(winner, 0);
        _roundWins[winner]++;
        Debug.Log($"{winner} has won the round");

        CheckForGameEnd();
    }


    private IEnumerator DelayNextState()
    {
        yield return _delay;
        machine.SetState(spawningState);
    }

    private void CheckForGameEnd()
    {
        _roundCount++;
        if (_roundCount >= amountOfRounds)
        {
            machine.Next(_roundWins);
            return;
        }
        StartCoroutine(DelayNextState());
    }
}