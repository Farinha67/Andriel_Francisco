
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemData dados;

    public ItemData Dados => dados;

    private void OnValidate()
    {
        if (dados == null)
            return;

        gameObject.tag = "Item";
    }
}