using UnityEngine;

[System.Serializable]
public struct CardSpawnData
{
    public Vector3 position;
    public int layer;

    public CardSpawnData(Vector3 pos, int layer)
    {
        this.position = pos;
        this.layer = layer;
    }

    public void ChangePosition(Vector3 pos)
    {
        this.position = pos;
    }
    
    public void ChangeLayer(int layer)
    {
        this.layer = layer;
    }
}