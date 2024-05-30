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

        //아이템이 중복 가능한지? canStack
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

        // 비어있는 슬롯 가져온다
        ItemSlot emptySlot = GetEmptySlot();

        // 있다면
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
            // 기존에 선택했던 아이템이 있을 때 하이라이트를 꺼주고 장착할 아이템의 하이라이트를 켜주기.
            // 아무 아이템도 없을 때는 슬롯 하이라이트 하지 않기.

            //슬롯에 아이템이 있을 때만
            if (slots[index].item != null)
            {
                // 처음 선택일 경우
                if (selectedItem == null)
                {
                    preSelectedItemIndex = index;
                    slots[index].HighLightSlot(true);
                }
                // 선택된 아이템을 한 번 더 눌렀을 경우
                else if(preSelectedItemIndex == index)
                {
                    OnUseItem();
                }
                // 처음 선택이 아닐 경우
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
