using UnityEngine;
using PurrNet;
using PurrNet.StateMachine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSpawningState : StateNode
{
    [SerializeField] private PlayerHealth playerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();
    private int roundNumber;

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
        RoundObjectManager.ResetRegisteredObjects(asServer);

        if (!asServer)
            return;

        roundNumber++;
        ShowRoundStart(roundNumber, roundNumber == 1 ? BuildInitialScoreboard() : null);

        // Clear previous round players before building the next set.
        DespawnPlayers();
        var spawnedPlayers = SpawnPlayers();
        machine.Next(spawnedPlayers);
    }

    [ObserversRpc(runLocally: true)]
    private void ShowRoundStart(int number, string scoreboard)
    {
        RoundAnnouncementUI.Present($"ROUND {number}", "FIGHT!", scoreboard, 2.5f);
    }

    private string BuildInitialScoreboard()
    {
        return string.Join("\n", networkManager.players
            .OrderBy(player => player.ToString(), System.StringComparer.Ordinal)
            .Select(player => $"{player}    0"));
    }

    private List<PlayerHealth> SpawnPlayers()
    {
        var spawnedPlayers = new List<PlayerHealth>();

        if (playerPrefab == null || spawnPoints.Count == 0)
            return spawnedPlayers;

        int currentSpawnIndex = 0;
        foreach (var player in networkManager.players)
        {
            var spawnPoint = spawnPoints[currentSpawnIndex];
            if (spawnPoint == null)
                continue;

            var newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            newPlayer.GiveOwnership(player);
            spawnedPlayers.Add(newPlayer);
            currentSpawnIndex++;

            if (currentSpawnIndex >= spawnPoints.Count)
                currentSpawnIndex = 0;
        }

        return spawnedPlayers;
    }

    private void DespawnPlayers()
    {
        var allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsInactive.Exclude);
        foreach (var player in allPlayers)
        {
            if (player != null)
                Destroy(player.gameObject);
        }
    }
}
