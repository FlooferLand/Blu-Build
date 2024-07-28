using UnityEngine;

public class TurntableController : MonoBehaviour {
    public enum RotationEnum {
        bigTurntable,
        billyBob,
        fatz,
        mitzi,
        looneyStage,
        dookStage,
        mangleStage,
        mangleCharStage
    }

    public GameObject mackValves;
    public int currentPosition = 0;
    public bool directionTurn;
    public RotationEnum turnTableType;
    public float rotationSpeed;
    public float smoothSpeed = 0.1f;
    private Mack_Valves bitChart;
    private bool forceClose;
    private float multiplier = 0;
    private float nextTime = 0;
    private int oldposition = 0;
    private float smoothvalue = 0;
    private int startPosition = 0;

    private void Awake() {
        if (turnTableType == RotationEnum.bigTurntable) {
            multiplier = 12.9032258065f;
            startPosition = -200;
        }
        else if (turnTableType == RotationEnum.looneyStage || turnTableType == RotationEnum.dookStage) {
            multiplier = 180;
            startPosition = -180;
        }
        else {
            multiplier = 36;
            startPosition = -270;
        }

        bitChart = mackValves.GetComponent<Mack_Valves>();
    }

    public void CreateMovements(float num3) {
        //30fps mode check
        //Check Bits
        int temp = 0;
        int firstbit = 0;
        switch (turnTableType) {
            case RotationEnum.bigTurntable:
                firstbit = 68;
                for (int i = 0; i < 5; i++) temp |= (bitChart.bottomDrawer[firstbit + i] ? 1 : 0) << i;
                break;
            case RotationEnum.billyBob:
                firstbit = 85;
                for (int i = 0; i < 4; i++) temp |= (bitChart.bottomDrawer[firstbit + i] ? 1 : 0) << i;
                break;
            case RotationEnum.fatz:
                firstbit = 93;
                for (int i = 0; i < 4; i++) temp |= (bitChart.bottomDrawer[firstbit + i] ? 1 : 0) << i;
                break;
            case RotationEnum.mitzi:
                firstbit = 77;
                for (int i = 0; i < 4; i++) temp |= (bitChart.bottomDrawer[firstbit + i] ? 1 : 0) << i;
                break;
            case RotationEnum.looneyStage:
                temp = bitChart.bottomDrawer[41] ? -1 : 0;
                break;
            case RotationEnum.dookStage:
                temp = bitChart.bottomDrawer[42] ? -1 : 0;
                break;
        }

        if (turnTableType != RotationEnum.mangleStage && turnTableType != RotationEnum.mangleCharStage) {
            //Check new direction change
            if (nextTime > currentPosition - 0.2f && nextTime < currentPosition + 0.2f)
                directionTurn = false;
            else
                directionTurn = true;
            //Position Apply
            currentPosition = temp;
            if (currentPosition != oldposition) {
                oldposition = currentPosition;
                smoothvalue = 0;
            }

            smoothvalue = Mathf.Min(1, smoothvalue + smoothSpeed);
            //Check position of turntable
            if (turnTableType != RotationEnum.dookStage && turnTableType != RotationEnum.looneyStage) {
                if (nextTime < currentPosition && directionTurn)
                    nextTime += rotationSpeed * num3 * smoothvalue;
                else if (nextTime > currentPosition && directionTurn) nextTime -= rotationSpeed * num3 * smoothvalue;
            }
            else {
                if (currentPosition == 0)
                    nextTime += rotationSpeed * num3 * smoothvalue;
                else
                    nextTime -= rotationSpeed * num3 * smoothvalue;
                nextTime = Mathf.Max(-1, Mathf.Min(0, nextTime));
            }

            //Give new degrees
            if (turnTableType == RotationEnum.bigTurntable)
                transform.rotation = Quaternion.Euler(new Vector3(-90, 0, startPosition + nextTime * multiplier));
            else if (turnTableType == RotationEnum.looneyStage || turnTableType == RotationEnum.dookStage)
                transform.rotation = Quaternion.Euler(new Vector3(-90, 0, startPosition + nextTime * multiplier));
            else
                transform.localRotation = Quaternion.Euler(new Vector3(0, 0, startPosition + nextTime * multiplier));
        }
        else {
            //MANGLE STAGES
            //T 9 10 11 12 13 14

            //Force
            if ((bitChart.topDrawer[12] && turnTableType == RotationEnum.mangleStage) ||
                (bitChart.topDrawer[13] && turnTableType == RotationEnum.mangleCharStage)) forceClose = true;
            if (forceClose) {
                if (turnTableType == RotationEnum.mangleStage) {
                    float pos = transform.localRotation.eulerAngles.y;
                    pos = pos > 180 ? pos - 360 : pos;
                    if (pos > 0)
                        transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.Max(0, pos - 0.5f * num3), 0));
                    else if (pos < 0)
                        transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.Min(0, pos + 0.5f * num3), 0));
                    else
                        forceClose = false;
                }
                else {
                    float pos = transform.localRotation.eulerAngles.y;
                    pos = pos > 180 ? pos - 360 : pos;
                    if (pos > 0)
                        transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.Max(0, pos - 1 * num3), 0));
                    else if (pos < 0)
                        transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.Min(0, pos + 1 * num3), 0));
                    else
                        forceClose = false;
                }
            }
            else {
                if (turnTableType == RotationEnum.mangleStage) {
                    float pos = transform.localRotation.eulerAngles.y;
                    pos = pos > 180 ? pos - 360 : pos;
                    if (bitChart.topDrawer[8])
                        transform.localRotation =
                            Quaternion.Euler(new Vector3(0, Mathf.Max(-179.9f, pos - 0.5f * num3), 0));
                    else if (bitChart.topDrawer[9])
                        transform.localRotation =
                            Quaternion.Euler(new Vector3(0, Mathf.Min(179.9f, pos + 0.5f * num3), 0));
                }
                else {
                    float pos = transform.localRotation.eulerAngles.y;
                    pos = pos > 180 ? pos - 360 : pos;
                    if (bitChart.topDrawer[10])
                        transform.localRotation =
                            Quaternion.Euler(new Vector3(0, Mathf.Max(-179.9f, pos - 1 * num3), 0));
                    else if (bitChart.topDrawer[11])
                        transform.localRotation =
                            Quaternion.Euler(new Vector3(0, Mathf.Min(179.9f, pos + 1 * num3), 0));
                }
            }
        }
    }
}