using System.Collections.Generic;
using UnityEngine;

public class RandomShapeGenerator : Singleton<RandomShapeGenerator>
{
    /// <summary>
    /// 生成多层随机矩阵
    /// </summary>
    public List<bool[,]> GenerateLayerMatrices(
        int layerCount,
        int minRows,
        int maxRows,
        int minCols,
        int maxCols,
        int minTotalFilledCount,
        bool allowMirror)
    {
        while (true)
        {
            List<bool[,]> layers = new();
            int totalFilled = 0;

            for (int i = 0; i < layerCount; i++)
            {
                int rows = Random.Range(minRows, maxRows + 1);
                int cols = Random.Range(minCols, maxCols + 1);

                bool[,] matrix = new bool[rows, cols];

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        bool value = Random.value > 0.3f; // 70% 填充率
                        matrix[r, c] = value;

                        if (value)
                            totalFilled++;
                    }
                }

                if (allowMirror)
                {
                    MirrorHorizontal(matrix);
                    MirrorVertical(matrix);
                }

                layers.Add(matrix);
            }

            if (totalFilled >= minTotalFilledCount)
                return layers;
            // 否则重新生成整套矩阵
        }
    }

    private void MirrorHorizontal(bool[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols / 2; c++)
            {
                matrix[r, cols - 1 - c] = matrix[r, c];
            }
        }
    }

    private void MirrorVertical(bool[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int r = 0; r < rows / 2; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                matrix[rows - 1 - r, c] = matrix[r, c];
            }
        }
    }
}
