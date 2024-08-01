using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Curtain_Valves : MonoBehaviour {
    public GameObject mackValves;

    [Range(0.0001f, .01f)] public float[] flowControlOut;
    [Range(0.0001f, .01f)] public float[] flowControlIn;

    public Animator[] curtainAnimators;
    public bool[] curtainBools;
    public bool curtainOverride = false;
    
    private readonly List<int> cylindersBottom = new();
    private readonly List<int> cylindersTop = new();
    private Mack_Valves bitChart;

    private void Start() {
        bitChart = mackValves.GetComponent<Mack_Valves>();
        foreach (var animator in curtainAnimators) {
            animator.Play("Closed");
            for (int e = 0; e < animator.layerCount; e++) {
                string temp = animator.GetLayerName(e);
                if (temp[^1] == 'T')
                    cylindersTop.Add(int.Parse(temp.Substring(0, temp.Length - 1)));
                else
                    cylindersBottom.Add(int.Parse(temp.Substring(0, temp.Length - 1)));
            }
        }
        curtainBools = new bool[curtainAnimators.Length];
    }

    public void CreateMovements(float num3) {
        // Loop through cylinders to update
        for (int i = 0; i < cylindersTop.Count; i++)
            // Get current animation value
            SetTime("T", cylindersTop[i], bitChart.topDrawer, ref i, num3);
        for (int i = 0; i < cylindersBottom.Count; i++)
            // Get current animation value
            SetTime("B", cylindersBottom[i], bitChart.bottomDrawer, ref i, num3);
    }

    private void SetTime(string drawername, int currentAnim, bool[] drawer, ref int lasti, float num3) {
        // Cycle through parameters to find matching code
        foreach (var curtain in curtainAnimators) {
            for (int e = 0; e < curtain.parameters.Length; e++) {
                if (curtain.parameters[e].name.Substring(0, curtain.parameters[e].name.Length - 1) == currentAnim.ToString()) {
                    //Calculate next value of the parameter
                    float nextTime = curtain.GetFloat(currentAnim + drawername);

                    //Check if animation is already done
                    if (!curtainOverride) {
                        if (Mathf.Approximately(nextTime, 0) && !drawer[currentAnim - 1])
                            break;
                        if (Mathf.Approximately(nextTime, 1) && drawer[currentAnim - 1])
                            break;
                    }

                    // Set bool
                    if (drawer[currentAnim - 1])
                        curtainBools[lasti] = true;
                    else if (drawer[currentAnim + 1 - 1]) curtainBools[lasti] = false;
                    // Set Curtain
                    if (!curtainOverride) {
                        if (curtainBools[lasti])
                            nextTime += flowControlOut[e] * 1.25f * num3;
                        else
                            nextTime -= flowControlIn[e] * 1.25f * num3;
                    }
                    else {
                        nextTime += flowControlOut[e] * 1.25f * num3;
                    }

                    nextTime = Mathf.Min(Mathf.Max(nextTime, 0), 1);

                    // Apply parameter
                    curtain.SetFloat(currentAnim + drawername, nextTime);
                    break;
                }
            }
        }
    }
}