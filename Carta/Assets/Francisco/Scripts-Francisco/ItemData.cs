
using UnityEngine;

[CreateAssetMenu(fileName = "Novo Item", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string nomeItem = "Item";
    [Min(0)] public int preçoVenda = 10;
    public Sprite icone;
}
