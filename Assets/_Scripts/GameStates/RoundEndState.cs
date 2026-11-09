using PurrNet.StateMachine;
using UnityEngine;
using System.Collections;
using PurrNet;
using System.Collections.Generic;
using System.Linq;

public class RoundEndState : StateNode<PlayerID>
{
    [SerializeField] private int amountOfRounds = 3;
    [SerializeField] private StateNode spawningState;

    private int _roundCount = 0;
    private readonly WaitForSeconds _delay = new(3f);
    private readonly Dictionary<PlayerID, int> _roundWins = new();

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;

        EnsureScoreEntries();
        ShowRoundResult("ROUND DRAW", "No winner this round", BuildScoreboard());

        // A round can end without a clear winner; the next round still needs to start.
        Debug.Log("Round has ended with no winner!");
        CheckForGameEnd();
    }

    public override void Enter(PlayerID winner, bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;

        EnsureScoreEntries();

        if (!_roundWins.ContainsKey(winner))
            _roundWins.Add(winner, 0);

        _roundWins[winner]++;
        ShowRoundResult($"{winner} HAS WON THE ROUND!", string.Empty, BuildScoreboard());
        Debug.Log($"{winner} has won the round");
        CheckForGameEnd();
    }

    [ObserversRpc(runLocally: true)]
    private void ShowRoundResult(string headline, string detail, string scoreboard)
    {
        RoundAnnouncementUI.Present(headline, detail, scoreboard, 3f);
    }

    private void EnsureScoreEntries()
    {
        if (networkManager == null)
            return;

        foreach (var player in networkManager.players)
        {
            if (!_roundWins.ContainsKey(player))
                _roundWins.Add(player, 0);
        }
    }

    private string BuildScoreboard()
    {
        return string.Join("\n", _roundWins
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => entry.Key.ToString(), System.StringComparer.Ordinal)
            .Select(entry => $"{entry.Key}    {entry.Value}"));
    }

    private IEnumerator DelayNextState()
    {
        yield return _delay;
        if (spawningState != null)
            machine.SetState(spawningState);
    }

    private void CheckForGameEnd()
    {
        _roundCount++;

        // If the configured number of rounds has been reached, hand the tally to the game end state.
        if (_roundCount >= amountOfRounds)
        {
            machine.Next(_roundWins);
            return;
        }

        StartCoroutine(DelayNextState());
    }
}