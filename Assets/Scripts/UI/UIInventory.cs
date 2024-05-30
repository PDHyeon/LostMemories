using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public Transform slotPanel;

    private PlayerController controller;
    private PlayerCondition condition;

    public ItemData selectedItem;
    int selectedItemIndex = 0;

    int preSelectedItemIndex;
    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;

        CharacterManager.Instance.Player.addItem += AddItem;

        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
        }
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        //�������� �ߺ� ��������? canStack
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        // ����ִ� ���� �����´�
        ItemSlot emptySlot = GetEmptySlot();

        // �ִٸ�
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }
    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;
    }

    public void OnUseItem()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.Stamina:
                        condition.AddStamina(selectedItem.consumables[i].value);
                        break;
                    case ConsumableType.SpeedUp:
                        StartCoroutine(controller.SpeedUp(selectedItem.consumables[i].value));
                        break;
                }
            }
            RemoveSelectedItem();
            slots[preSelectedItemIndex].HighLightSlot(false);
        }
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].item = null;
            selectedItemIndex = -1;
        }

        UpdateUI();
    }

    public void OnQuickSlotNumberInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            int index = int.Parse(context.control.name) - 1;
            // ������ �����ߴ� �������� ���� �� ���̶���Ʈ�� ���ְ� ������ �������� ���̶���Ʈ�� ���ֱ�.
            // �ƹ� �����۵� ���� ���� ���� ���̶���Ʈ ���� �ʱ�.

            //���Կ� �������� ���� ����
            if (slots[index].item != null)
            {
                // ó�� ������ ���
                if (selectedItem == null)
                {
                    preSelectedItemIndex = index;
                    slots[index].HighLightSlot(true);
                }
                // ���õ� �������� �� �� �� ������ ���
                else if(preSelectedItemIndex == index)
                {
                    OnUseItem();
                }
                // ó�� ������ �ƴ� ���
                else
                {
                    slots[preSelectedItemIndex].HighLightSlot(false);
                    slots[index].HighLightSlot(true);
                    preSelectedItemIndex = index;
                }
                SelectItem(index);
            }
        }
    }
}
