public enum GroundType
{
    Grass,
    Tilled,
    Watered
}
[System.Serializable]
public class TileData
{
    public GroundType groundType = GroundType.Grass;
    public bool hasCrop;
    public TileData(GroundType type)
    {
        groundType = type;
        hasCrop = false;
    }
}