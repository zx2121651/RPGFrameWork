using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class multiChoosePanel : keyBoardMenuList
    {
        private Action<int> onChoiceMade;

        // This method will be called by the UIManager to set up the panel
        public void showChoices(List<string> choices, Action<int> callback)
        {
            this.onChoiceMade = callback;

            // Dynamically set the menu size
            optionNum = new Vector2Int(1, choices.Count);

            // Spawn the UI list items
            if (listItems.Count < choices.Count)
            {
                spawanItems();
            }

            // Populate the list items with choice text
            List<ListItemData> datas = new List<ListItemData>();
            List<string> keys = new List<string>();
            for (int i = 0; i < choices.Count; i++)
            {
                datas.Add(new ListItemData(new string[] { choices[i] }));
                keys.Add(i.ToString()); // Use index as key
            }

            // Update the UI
            allPage = (int)Math.Ceiling((float)datas.Count / getMaxNum());
            if (page > allPage)
            {
                page = 0;
                resetCurosr();
            }
            int startIndex = page * getMaxNum();
            clearAll();
            if (datas != null && keys != null)
            {
                for (int i = 0; i < listItems.Count; i++)
                {
                    if (i < datas.Count && i < keys.Count)
                    {
                        setKey(i, keys[i + startIndex]);
                        setData(i, datas[i + startIndex]);
                    }
                    else
                    {
                        // Deactivate unused list items
                        listItems[i].gameObject.SetActive(false);
                    }
                }
            }

            // Set the panel to be active
            gameObject.SetActive(true);
            doFunc(); // A method from the parent to take control
        }

        // Override doOption to handle the choice selection
        public override void doOption()
        {
            base.doOption(); // Plays "yes" sound

            int selectedIndex = getCursorPosIndex();

            if (onChoiceMade != null)
            {
                onChoiceMade(selectedIndex);
            }

            // Destroy the panel after making a choice
            Destroy(gameObject);
        }

        // Override cancel to do nothing, forcing a choice
        public override void cancel()
        {
            // Or, we could implement a default choice/cancellation behavior
            // For now, disable cancelling.
            data.playSE(unable, true);
        }
    }
}
