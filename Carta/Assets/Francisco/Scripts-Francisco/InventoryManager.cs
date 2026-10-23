
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemStack
    {
        public ItemData item;
        public int quantidade;

        public ItemStack(ItemData item, int quantidade)
        {
            this.item = item;
            this.quantidade = quantidade;
        }
    }

    [Header("Interface")]
    [SerializeField] private TMP_Text inventarioTexto;
    [SerializeField] private TMP_Text dinheiroTexto;

    [Header("Dinheiro inicial")]
    [SerializeField, Min(0)] private int dinheiroInicial = 0;

    private readonly List<ItemStack> itens = new List<ItemStack>();

    public int Dinheiro { get; private set; }

    private void Awake()
    {
        Dinheiro = dinheiroInicial;
        AtualizarInterface();
    }

    public void AdicionarItem(ItemData item)
    {
        if (item == null)
            return;

        ItemStack stack = itens.Find(x => x.item == item);

        if (stack != null)
        {
            stack.quantidade++;
        }
        else
        {
            itens.Add(new ItemStack(item, 1));
        }

        AtualizarInterface();
    }

    public int CalcularValorTotal()
    {
        int total = 0;

        foreach (ItemStack stack in itens)
        {
            if (stack.item != null)
                total += stack.item.preçoVenda * stack.quantidade;
        }

        return total;
    }

    public void VenderTudo()
    {
        int total = CalcularValorTotal();

        if (total <= 0)
            return;

        Dinheiro += total;
        itens.Clear();

        AtualizarInterface();

        Debug.Log("Itens vendidos! Você recebeu $" + total);
    }

    public void AtualizarInterface()
    {
        if (inventarioTexto != null)
        {
            StringBuilder texto = new StringBuilder();

            texto.AppendLine("INVENTÁRIO");

            if (itens.Count == 0)
            {
                texto.AppendLine("Vazio");
            }
            else
            {
                foreach (ItemStack stack in itens)
                {
                    if (stack.item != null)
                    {
                        texto.AppendLine(
                            stack.item.nomeItem + " x" + stack.quantidade
                        );
                    }
                }
            }

            inventarioTexto.text = texto.ToString();
        }

        if (dinheiroTexto != null)
        {
            dinheiroTexto.text = "$ " + Dinheiro;
        }
    }
}
