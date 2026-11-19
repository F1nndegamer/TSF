using UnityEngine;

public enum Function
{
    Hoe,
    Can,
    Seeds,
    Sythe,
    Untill,
    Sell,
    Buy,
    Plant,
    Nothing,
}
public class HotBarSlot : MonoBehaviour
{
    public Function function;
    public PlantData data;
    public Sprite sprite;
    public int index;
    void Start()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == this.transform)
            {
                index = i;
            }
        }
    }
}
