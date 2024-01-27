using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public Text moneyText;

    public int money = 100;

    // Start is called before the first frame update
    private void Start()
    {
        if (moneyText != null)
            moneyText.text = "$" + money;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void addMoney(int amount)
    {
        money += amount;
        if (moneyText) moneyText.text = "$" + money;
    }
}