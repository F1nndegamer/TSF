public enum GroundType
{
    Grass,
    Tilled,
    Watered
}

[System.Serializable]
public class TileData
{
    public GroundType groundType;
    public bool hasCrop;
    public bool hasFertiliser;

    public TileData(GroundType type = GroundType.Grass)
    {
        groundType = type;
        hasCrop = false;
        hasFertiliser = false;
    }
}