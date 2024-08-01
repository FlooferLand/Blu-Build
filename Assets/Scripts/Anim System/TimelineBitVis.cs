using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TimelineBitVis : MonoBehaviour {
    public UI_ShowtapeManager uiShowtapeManager;
    public TimelineEditor timelineEditor;
    public GameObject bitPrefab;
    public GameObject holderTemplate;

    [FormerlySerializedAs("holders")] [FormerlySerializedAs("holder")]
    public GameObject[] bitBoxes;

    private readonly int maximumHolders = 15;

    private void Awake() {
        holderTemplate.SetActive(false);
    }

    /**
     * Deletes previous tracks and then creates the tracks for the timeline
     */
    public void RepaintBitGroups() {
        if (uiShowtapeManager?.inputHandler?.editorKeys != null)
            uiShowtapeManager.inputHandler.editorKeys = new UI_WindowMaker.MovementRecordings();
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).gameObject.activeSelf)
                Destroy(transform.GetChild(i).gameObject);

        bitBoxes = new GameObject[Mathf.Min(timelineEditor.tlRecordGroup.Length, maximumHolders)];
        var temp = new List<UI_WindowMaker.inputNames>();
        for (int i = 0; i < Mathf.Min(timelineEditor.tlRecordGroup.Length, maximumHolders); i++)
            if (timelineEditor.holderOffset + i < timelineEditor.tlRecordGroup.Length) {
                temp.Add(new UI_WindowMaker.inputNames());
                temp[temp.Count - 1].index = new int[1];
                if (timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].bit > 150) {
                    temp[temp.Count - 1].drawer = true;
                    temp[temp.Count - 1].index[0] = i + timelineEditor.holderOffset - 149;
                }
                else {
                    temp[temp.Count - 1].index[0] = i + timelineEditor.holderOffset + 1;
                }

                var bitBox = Instantiate(holderTemplate, transform);
                var bitBoxDimensions = holderTemplate.GetComponent<RectTransform>().sizeDelta;
                bitBox.SetActive(true);
                if (bitBox.transform is RectTransform rect)
                    rect.anchoredPosition = new Vector3(rect.anchoredPosition.x,
                        rect.anchoredPosition.y - i * (bitBoxDimensions.y + 2));
                int temper = i + 1;
                if (temper == 10) temper = 0;

                var screen = bitBox.GetComponent<TimelineScreen>();
                if (screen) {
                    screen.bitHeaderName.text = "(" + temper + ") " +
                                                timelineEditor.windowMaker.SearchBitChartName(timelineEditor
                                                    .tlRecordGroup[i + timelineEditor.holderOffset].bit);
                    screen.rCBitBarNumber = i;
                    bitBox = screen.holder ?? bitBox;
                }

                bitBoxes[i] = bitBox;
            }

        uiShowtapeManager.inputHandler.editorKeys.inputNames = temp.ToArray();
    }

    public void RepaintTimeline(float viewzoomMin, float viewZoomMax, float audioLengthMax) {
        for (int i = 0; i < timelineEditor.tlRecordGroup.Length; i++) {
            timelineEditor.tlRecordGroup[i].checkedObject = false;
            timelineEditor.tlRecordGroup[i].currentBit = null;
        }

        for (int i = 0; i < bitBoxes.Length; i++)
            if (bitBoxes[i] != null)
                foreach (Transform child in bitBoxes[i].transform)
                    Destroy(child.gameObject);
        int bitStart = Mathf.RoundToInt(viewzoomMin * 60.0f);
        int bitsRepresented = Mathf.RoundToInt((viewZoomMax - viewzoomMin) * 60.0f);

        int secondIndex = 0;
        //Loop all frames in view
        while (true) {
            //Function finished
            if (secondIndex >= bitsRepresented) break;

            var data = uiShowtapeManager.RshwData ?? new BitArray[80];
            if (bitStart + secondIndex < data.Length) {
                for (int i = 0; i < bitBoxes.Length; i++)
                    if (bitBoxes[i] && i + timelineEditor.holderOffset < timelineEditor.tlRecordGroup.Length) {
                        //If bit is true on the current frame
                        if (timelineEditor.tlRecordGroup == null || data.Length < bitStart + secondIndex) continue;
                        if (data[bitStart + secondIndex]
                            .Get(timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].bit - 1)) {
                            if (timelineEditor.tlRecordGroup[i + timelineEditor.holderOffset].currentBit == null)
                                CreateButton(viewzoomMin, viewZoomMax, audioLengthMax, secondIndex, i);
                            else
                                UpdateButton(false, viewzoomMin, viewZoomMax, i);

                            // Making the playback marker light up when it hits a note or a note is held
                            // TODO: FIXME: Very unorganized. Should be moved to TimelineEditor.Update under the playbackMarker set position section
                            timelineEditor.playbackMarker.triggerBitHit();
                        }
                        else {
                            timelineEditor.playbackMarker.triggerBitReset();
                        }
                    }

                for (int i = 0; i < bitBoxes.Length; i++) UpdateButton(true, viewzoomMin, viewZoomMax, i);
            }

            secondIndex++;
        }
    }

    private void CreateButton(float viewzoomMin, float viewZoomMax, float audioLengthMax, int secondIndex,
        int recGroupIndex) {
        if (bitBoxes[recGroupIndex] != null) {
            timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit =
                Instantiate(bitPrefab, bitBoxes[recGroupIndex].transform);
            timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit.SetActive(true);
            var rect =
                timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit
                    .transform as RectTransform;
            rect.position =
                new Vector3(remap(viewzoomMin + secondIndex / 60.0f, viewzoomMin, viewZoomMax, 0, 1) * Screen.width,
                    rect.position.y, 0);
            rect.sizeDelta =
                new Vector2(
                    rect.sizeDelta.x + Screen.width * (1080.0f / Screen.height) /
                    ((viewZoomMax - viewzoomMin) * uiShowtapeManager.dataStreamedFPS), rect.sizeDelta.y);
            timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject = true;
        }
    }

    public float remap(float val, float in1, float in2, float out1, float out2) {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    private void UpdateButton(bool checkOnly, float viewzoomMin, float viewZoomMax, int recGroupIndex) {
        if (bitBoxes[recGroupIndex] != null) {
            if (checkOnly) {
                if (!timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject)
                    timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit = null;
                timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject = false;
            }
            else {
                var rect = timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].currentBit
                    .transform as RectTransform;
                rect.sizeDelta =
                    new Vector2(
                        rect.sizeDelta.x + Screen.width * (1080.0f / Screen.height) /
                        ((viewZoomMax - viewzoomMin) * uiShowtapeManager.dataStreamedFPS), rect.sizeDelta.y);
                timelineEditor.tlRecordGroup[recGroupIndex + timelineEditor.holderOffset].checkedObject = true;
            }
        }
    }
}