using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
	public Text moneyText;
	public int money=100;
    // Start is called before the first frame update
    void Start()
    {
        		moneyText.text="$"+money;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void addMoney(int amount){
		money+=amount;
		if (moneyText){
			moneyText.text="$"+money;
		}
	}

}
