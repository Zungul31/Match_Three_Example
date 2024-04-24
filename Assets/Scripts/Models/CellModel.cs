using System;


[Serializable]
public class CellModel
{
    public int x;
    public int y;
    public EItemType type;

    public CellModel(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public CellModel(EItemType type, int x, int y)
    {
        this.type = type;
        this.x = x;
        this.y = y;
    }
}
