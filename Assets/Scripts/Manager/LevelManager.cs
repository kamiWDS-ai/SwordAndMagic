using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public void Generate(LevelConfig levelConfig)
    {
        List<CardSpawnData> spawnPoints =
            CardLayoutGenerator.Instance.Generate(levelConfig);
        
        int LevelCardCount = spawnPoints.Count-spawnPoints.Count%3;

        List<CardData> cardPool =
            CardPoolGenerator.Instance.Generate(levelConfig,LevelCardCount);

        for (int i = 0; i < LevelCardCount; i++)
        {
            CardFactory.Instance.Create(
                cardPool[i],
                spawnPoints[i]
            );
        }

        CardStackChecker.Instance.RefreshAllCardStates();
    }
}