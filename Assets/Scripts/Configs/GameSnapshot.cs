using System.Collections.Generic;

public class GameSnapshot
{
    public List<int> remainingCardIds = new();
    public List<int> slotCardTypeIds = new();

    public int stepsUsed;
}