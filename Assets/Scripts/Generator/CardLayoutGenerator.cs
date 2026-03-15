using System.Collections.Generic;
using UnityEngine;

public class CardLayoutGenerator : Singleton<CardLayoutGenerator>
{
    public List<CardSpawnData> Generate(LevelConfig config)
    {
        List<CardSpawnData> result = new();

        if (config == null)
            return result;

        int denseCardCount = CalculateDenseCardCount(config);

        // 1️⃣ 先生成多层堆叠网格区
        Bounds gridBounds = GenerateGridArea(config, result, denseCardCount);

        // 2️⃣ 再生成密集堆叠区（自动避开网格）
        if (config.denseArea != null && config.denseArea.enable)
        {
            GenerateDenseArea(config, result, gridBounds);
        }

        return result;
    }

    // ==========================================
    // 计算密集区总牌数
    // ==========================================
    private int CalculateDenseCardCount(LevelConfig config)
    {
        int count = 0;

        if (config.denseArea == null || !config.denseArea.enable)
            return 0;

        foreach (var stack in config.denseArea.stacks)
        {
            count += stack.stackCount;
        }

        return count;
    }

    // ==========================================
    // 1️⃣ 网格区域生成 + 返回边界
    // ==========================================
    private Bounds GenerateGridArea(
        LevelConfig config,
        List<CardSpawnData> result,
        int denseCardCount)
    {
        var grid = config.gridArea;

        int requiredCardCount = config.minCardCount-denseCardCount;

        List<bool[,]> layerMatrices =
            RandomShapeGenerator.Instance.GenerateLayerMatrices(
                grid.layerCount,
                grid.minRows,
                grid.maxRows,
                grid.minCols,
                grid.maxCols,
                requiredCardCount,
                true
            );

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int layer = 0; layer < layerMatrices.Count; layer++)
        {
            bool[,] matrix = layerMatrices[layer];

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            Vector2 centerOffset = new Vector2(
                (cols - 1) * config.cardWidth * 0.5f,
                (rows - 1) * config.cardHeight * 0.5f
            );

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!matrix[r, c])
                        continue;

                    float x = c * config.cardWidth - centerOffset.x;
                    float y = -r * config.cardHeight + centerOffset.y;
                    float z = 0;

                    Vector3 pos = new Vector3(x, y, z);

                    result.Add(new CardSpawnData(pos, layer));

                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                    minY = Mathf.Min(minY, y);
                    maxY = Mathf.Max(maxY, y);
                }
            }
        }

        Vector3 center = new Vector3(
            (minX + maxX) * 0.5f,
            (minY + maxY) * 0.5f,
            0);

        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            0);

        return new Bounds(center, size);
    }

    // ==========================================
    // 2️⃣ 密集区自动避开生成
    // ==========================================
    private void GenerateDenseArea(
        LevelConfig config,
        List<CardSpawnData> result,
        Bounds gridBounds)
    {
        foreach (var stack in config.denseArea.stacks)
        {
            Vector3 startPos = Vector3.zero;
            Vector3 offsetPerCard = Vector3.zero;
            switch (stack.spawnSide)
            {
                case DenseStackLayoutConfig.SpawnSide.Right:
                    offsetPerCard=new Vector3(0f,-0.15f,0f);
                    startPos = new Vector3(
                        gridBounds.max.x + stack.margin,
                        gridBounds.center.y,
                        0);
                    break;

                case DenseStackLayoutConfig.SpawnSide.Left:
                    offsetPerCard=new Vector3(0f,-0.15f,0f);
                    startPos = new Vector3(
                        gridBounds.min.x - stack.margin,
                        gridBounds.center.y,
                        0);
                    break;

                case DenseStackLayoutConfig.SpawnSide.Top:
                    offsetPerCard=new Vector3(0.15f,0f,0f);
                    startPos = new Vector3(
                        gridBounds.center.x,
                        gridBounds.max.y + stack.margin,
                        0);
                    break;

                case DenseStackLayoutConfig.SpawnSide.Bottom:
                    offsetPerCard=new Vector3(0.15f,0f,0f);
                    startPos = new Vector3(
                        gridBounds.center.x,
                        gridBounds.min.y - stack.margin,
                        0);
                    break;
            }

            for (int i = 0; i < stack.stackCount; i++)
            {
                Vector3 pos =
                    startPos +
                    offsetPerCard * i;

                int layer = stack.startLayerIndex + i;

                result.Add(new CardSpawnData(
                    new Vector3(pos.x, pos.y, 0),
                    layer
                ));
            }
        }
    }
}
