using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    private TMP_Text textComp;

    public delegate void ClickOnLinkEvent(string keyword);
    public static event ClickOnLinkEvent OnClickedOnLinkEvent = null;
    
    private void Awake() {
        textComp = GetComponent<TMP_Text>();
    }

    private bool findLink(PointerEventData eventData, out string link, out string text) {
        if (!findLink(eventData, out TMP_LinkInfo info)) {
            link = text = "";
            return false;
        }
        
        link = info.GetLinkID();
        text = info.GetLinkText();
        return true;
    }
    private bool findLink(PointerEventData eventData, out TMP_LinkInfo info) {
        var mousePos = new Vector3(eventData.position.x, eventData.position.y, 0f);
        var taggedLink = TMP_TextUtilities.FindIntersectingLink(textComp, mousePos, Camera.main);
        if (taggedLink == -1) {
            info = new TMP_LinkInfo();
            return false;
        }

        info = textComp.textInfo.linkInfo[taggedLink];
        return true;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!findLink(eventData, out string link, out string text)) return;

        if (link.StartsWith("www.") || link.StartsWith("http")) {
            Application.OpenURL(link);
            return;
        }
        
        OnClickedOnLinkEvent?.Invoke(text);
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        if (!findLink(eventData, out var info)) return;
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!findLink(eventData, out var info)) return;
    }
}
