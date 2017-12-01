using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    #region Attributes

    public static Inventory instance;
    public static int numSlots = 8;

    public struct Item
    {
        public Sprite sprite;
        public string info;
        public string name;
        public int id;

        public Item(PickUpItem pickUpItem)
        {
            sprite = pickUpItem.GetComponent<SpriteRenderer>().sprite;
            info = pickUpItem.info;
            name = pickUpItem.name;
            id = pickUpItem.id;
        }
    }

    private GameObject selectedItemPanel;
    private GameObject selectedItemSlot;
    private Text selectedItemInfo;
    private Item? selectedItem;
    private Item?[] items;

    #endregion

    #region Start

    public void Start()
    {
        instance = this;

        if (items == null)
        {
            items = new Item?[numSlots];
        }
    }

    #endregion

    #region Common

    public void UseItem(PlayerController player)
    {

        if (selectedItem == null)
        {
            return;
        }

        ActivableSystem[] systems = GameObject.FindObjectsOfType<ActivableSystem>();
        ActivableSystem system = null;

        for (int i = 0; i < systems.Length; i++)
        {
            if (!systems[i].activated)
            {
                if (Vector2.Distance(systems[i].transform.position, player.transform.position) <= systems[i].activationDistance)
                {
                    system = systems[i];
                    break;
                }
            }
        }

        if (system)
        {
            if (system.PlaceItem(selectedItem.Value.sprite))
            {
                RemoveItem();
            }
        }

    }

    public void AddItem(PickUpItem item)
    {
        int freeSlot = GetFreeSlot();

        if (freeSlot != -1)
        {
            items[freeSlot] = new Item(item);

            Image slotSprite = GameObject.Find("SlotSprite" + freeSlot).GetComponent<Image>();
            slotSprite.sprite = item.GetComponent<SpriteRenderer>().sprite;

            SendMessageToServer("InventoryUpdate/Add/" + freeSlot + "/" + item.name, true);
        }

    }

    public void RemoveItem()
    {

        bool found = false;
        int index;

        for (index = 0; index < items.Length; index++)
        {
            if (items[index].Equals(selectedItem))
            {
                found = true;
                break;
            }
        }

        if (found)
        {
            items[index] = null;

            Image slotSprite = GameObject.Find("SlotSprite" + index).GetComponent<Image>();
            slotSprite.sprite = null;

            UnselectItem();
            SendMessageToServer("InventoryUpdate/Remove/" + index, true);
        }

    }

    public void SelectItem(Item item)
    {
        selectedItem = item;

        selectedItemInfo.text = "";
        selectedItemInfo.text = "<color=#e67f84ff><b>" + "Usando '" + selectedItem.Value.name + "': </b></color>" + "\r\n";
        selectedItemInfo.text += "<color=#f9ca45ff>" + selectedItem.Value.info + "</color>";

        selectedItemSlot.GetComponent<Image>().sprite = selectedItem.Value.sprite;

        ToogleSelectedItem(true);
    }

    public void UnselectItem()
    {
        if (selectedItem == null)
        {
            Debug.LogError("No item to drop");
            return;
        }

        selectedItem = null;

        selectedItemInfo.text = "";
        selectedItemSlot.GetComponent<Image>().sprite = null;

        ToogleSelectedItem(false);
    }

    public void DropItem()
    {
        if (selectedItem == null)
        {
            Debug.LogError("No item to drop");
            return;
        }


        SendMessageToServer("CreateGameObject/" + selectedItem.Value.name, true);
        RemoveItem();
        ToogleSelectedItem(false);
    }

    #endregion

    #region Utils

    public int GetFreeSlot()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    protected void ToogleSelectedItem(bool active)
    {
        selectedItemSlot.SetActive(active);
        selectedItemPanel.SetActive(active);
    }

    #endregion

    #region Messaging

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}
