using System;
using UnityEngine;

public class BirthdayCheck : MonoBehaviour {
    private void Awake() {
        if (DateTime.Now.DayOfYear != 40) Destroy(gameObject);
    }
}