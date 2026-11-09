using PurrNet.StateMachine;
using UnityEngine;
using System.Collections.Generic;
using PurrNet;
using System.Linq;

public class GameEndState : StateNode<Dictionary<PlayerID, int>>
{
    public override void Enter(Dictionary<PlayerID, int> roundWins, bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;

        // A match should always have at least one winner, but this guard prevents crashes for empty dictionaries.
        if (roundWins == null || roundWins.Count == 0)
        {
            Debug.Log("No winners were recorded for this game.");
            ShowMatchResult("MATCH OVER", "No round wins were recorded", string.Empty);
            return;
        }

        var highestScore = roundWins.Values.Max();
        var winners = roundWins
            .Where(player => player.Value == highestScore)
            .Select(player => player.Key.ToString())
            .OrderBy(name => name, System.StringComparer.Ordinal)
            .ToArray();

        var scoreboard = string.Join("\n", roundWins
            .OrderByDescending(player => player.Value)
            .ThenBy(player => player.Key.ToString(), System.StringComparer.Ordinal)
            .Select(player => $"{player.Key}    {player.Value}"));
        var headline = winners.Length == 1 ? $"{winners[0]} WINS THE MATCH!" : "MATCH TIED";
        var detail = winners.Length == 1 ? "Final scores" : $"Tied between {string.Join(", ", winners)}";

        ShowMatchResult(headline, detail, scoreboard);
        Debug.Log($"{headline} {detail}");
        roundWins.Clear();
    }

    [ObserversRpc(runLocally: true)]
    private void ShowMatchResult(string headline, string detail, string scoreboard)
    {
        RoundAnnouncementUI.Present(headline, detail, scoreboard, 0f);
    }
}
