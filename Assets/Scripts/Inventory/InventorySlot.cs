using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public int number = 0;
    void Start()
    {
        Transform parent = transform.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == this.transform)
            {
                number = i;
            }
        }
    }
    void Update()
    {

    }
}
