using System;
using System.Collections.Generic;
using UnityEngine;

public class Curtain_Valves : MonoBehaviour
{

    public GameObject mackValves;
    [Range(0.0001f, .01f)]
    public float[] flowControlOut;
    [Range(0.0001f, .01f)]
    public float[] flowControlIn;

    Animator curtainAnimator;
    List<int> cylindersTop = new();
    List<int> cylindersBottom = new();
    Mack_Valves bitChart;
    public bool[] curtainbools;
    public bool curtainOverride;

    private void Start()
    {
        curtainAnimator = GetComponent<Animator>();
        bitChart = mackValves.GetComponent<Mack_Valves>();
        curtainAnimator.Play("Closed");
        for (int e = 0; e < curtainAnimator.layerCount; e++)
        {
            string temp = curtainAnimator.GetLayerName(e);
            if (temp[^1] == 'T')
            {
                cylindersTop.Add(Int32.Parse(temp.Substring(0, temp.Length - 1)));
            }
            else
            {

                cylindersBottom.Add(Int32.Parse(temp.Substring(0, temp.Length - 1)));
            }
        }
        curtainbools = new bool[curtainAnimator.layerCount];
    }

    public void CreateMovements(float num3)
    {
        //Loop through cylinders to update
        for (int i = 0; i < cylindersTop.Count; i++)
        {
            //Get current animation value
            SetTime("T", cylindersTop[i], bitChart.topDrawer, i, num3);
        }
        for (int i = 0; i < cylindersBottom.Count; i++)
        {
            //Get current animation value
            SetTime("B", cylindersBottom[i], bitChart.bottomDrawer, i, num3);
        }
    }

    void SetTime(string drawername, int currentAnim, bool[] drawer, int lasti, float num3)
    {
        //Cycle through parameters to find matching code
        for (int e = 0; e < curtainAnimator.parameters.Length; e++) {
            if (curtainAnimator.parameters[e].name.Substring(0, curtainAnimator.parameters[e].name.Length - 1) == currentAnim.ToString()) {
                //Calculate next value of the parameter
                float nextTime = curtainAnimator.GetFloat(currentAnim + drawername);

                //Check if animation is already done
                if (!curtainOverride)
                {
                    if (nextTime == 0 && !drawer[currentAnim - 1])
                        break;
                    if (nextTime == 1 && drawer[currentAnim - 1])
                        break;
                }

                //Set bool
                if (drawer[currentAnim - 1])
                {
                    curtainbools[lasti] = true;
                }
                else if (drawer[currentAnim + 1 - 1])
                {
                    curtainbools[lasti] = false;
                }
                //Set Curtain
                if (!curtainOverride)
                {
                    if (curtainbools[lasti])
                    {
                        nextTime += (flowControlOut[e] * 1.25f * num3);
                    }
                    else
                    {
                        nextTime -= (flowControlIn[e] * 1.25f * num3);
                    }
                }
                else
                {
                    nextTime += (flowControlOut[e] * 1.25f * num3);
                }
                nextTime = Mathf.Min(Mathf.Max(nextTime, 0), 1);

                //Apply parameter
                curtainAnimator.SetFloat(currentAnim + drawername, nextTime);
                break;
            }
        }
    }
}
