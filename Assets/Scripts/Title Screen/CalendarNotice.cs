using System;
using UnityEngine;
using UnityEngine.UI;

public class CalendarNotice : MonoBehaviour {
    private void Awake() {
        var textComp = GetComponent<Text>();
        
        switch (DateTime.Now.Month) {
            case 8 when DateTime.Now.Day == 8:
                textComp.text = "Happy Birthday FNaF!";
                break;
            case 11 when DateTime.Now.Day == 11:
                textComp.text = "Happy Birthday FNaF 2!";
                break;
            case 3 when DateTime.Now.Day == 2:
                textComp.text = "Happy Birthday FNaF 3!";
                break;
            case 7 when DateTime.Now.Day == 23:
                textComp.text = "Happy Birthday FNaF 4!";
                break;
            case 10 when DateTime.Now.Day == 7:
                textComp.text = "Happy Birthday Sister Location!";
                break;
            case 1 when DateTime.Now.Day == 21:
                textComp.text = "Happy Birthday FNaF World!";
                break;
            case 10 when DateTime.Now.Day == 30:
                textComp.text = "Happy Birthday FNaF 4 Halloween Edition!";
                break;
            case 12 when DateTime.Now.Day == 4:
                textComp.text = "Happy Birthday FFPS!";
                break;
            case 5 when DateTime.Now.Day == 28:
                textComp.text = "Happy Birthday FNaF VR: Help Wanted!";
                break;
            case 8 when DateTime.Now.Day == 24:
                textComp.text = "Happy Birthday RR!";
                break;
            case 1 when DateTime.Now.Day == 22:
                textComp.text = "Happy Birthday Faz-Anim!";
                break;
            default:
                Destroy(gameObject.transform.parent.gameObject);
                break;
        }
    }
}
