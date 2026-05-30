using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerShopButton : MonoBehaviour
{
    public TowerData towerData;
    public TMP_Text costLabel;

    void Start()
    {
        // Find the button component and tell it to call SelectThisTower when clicked
        Button myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(SelectThisTower);
        }

        // Set the text on the button
        if (towerData != null)
        {
            if (costLabel != null)
            {
                costLabel.text = towerData.cost.ToString();
            }
        }

        // Listen for gold changes to enable/disable button
        GameManager.OnGoldChanged += UpdateAffordability;
        
        // Check once at start
        UpdateAffordability(GameManager.Instance.gold);
    }

    void OnDestroy()
    {
        // Stop listening if the button is destroyed
        GameManager.OnGoldChanged -= UpdateAffordability;
    }

    // This method is called by the Button's OnClick event in the Inspector
    public void SelectThisTower()
    {
        Debug.Log("[Shop] Clicked: " + towerData.towerName);
        
        // Tell the Placer to use this tower
        TowerPlacer.Instance.SelectTower(towerData);
        
        // Update the visual text at the top
        GameObject debugObj = GameObject.Find("DebugClickText");
        if (debugObj != null)
        {
            TMP_Text textComp = debugObj.GetComponent<TMP_Text>();
            textComp.text = "LAST CLICKED: " + towerData.towerName;
        }
    }

    void UpdateAffordability(int currentGold)
    {
        Button myButton = GetComponent<Button>();
        if (towerData != null && myButton != null)
        {
            if (currentGold >= towerData.cost)
            {
                myButton.interactable = true;
            }
            else
            {
                myButton.interactable = false;
            }
        }
    }
}
