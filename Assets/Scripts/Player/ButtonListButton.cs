using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour {
    [SerializeField] private ButtonListControl buttonControl;

    public void StartAwake() {
        StartCoroutine(CollectIcon());
    }

    public void Click() {
        buttonControl.CreateAttributePage(name);
    }

    private IEnumerator CollectIcon() {
        Debug.Log(name + "Icon Loading");
        var rr = Resources.LoadAsync<Sprite>("Merch/Icons/" + name);
        while (!rr.isDone) yield return null;
        Debug.Log(name + "Icon Loaded");
        transform.GetChild(0).GetComponent<Image>().sprite = rr.asset as Sprite;
    }
}