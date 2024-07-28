using UnityEngine;

public class PoolTable : MonoBehaviour {
    public GameObject holder;
    public GameObject whiteBall;
    public AudioClip hitBoard;
    public AudioClip hitBall;
    public AudioClip firstHit;
    public AudioClip poolSink;
    private AudioSource au;
    private int ballcount;
    private string[] ballNames;
    private Vector3[] balls;

    private void Start() {
        au = GetComponent<AudioSource>();
        ballcount = holder.transform.childCount;
        balls = new Vector3[ballcount];
        ballNames = new string[ballcount];
        for (int i = 0; i < holder.transform.childCount; i++) {
            balls[i] = holder.transform.GetChild(i).transform.localPosition;
            ballNames[i] = holder.transform.GetChild(i).name;
        }
    }

    private void OnTriggerEnter(Collider collision) {
        PlaySound(3);
        if (collision.gameObject.name == whiteBall.name) {
            Debug.Log("Pool: White Ball In");
            var t = NameToTransform(whiteBall.name);
            collision.gameObject.transform.localPosition = t;
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        else if (collision.gameObject.name != holder.name) {
            Debug.Log("Pool: Other Ball In");
            ballcount--;
            if (ballcount <= 1) {
                PlayerPrefs.SetInt("TicketCount", PlayerPrefs.GetInt("TicketCount") + 100);
                ballcount = balls.Length;
                for (int i = 0; i < holder.transform.childCount; i++) {
                    holder.transform.GetChild(i).gameObject.SetActive(true);
                    var t = NameToTransform(holder.transform.GetChild(i).name);
                    holder.transform.GetChild(i).transform.localPosition = t;
                    holder.transform.GetChild(i).GetComponent<Rigidbody>().velocity = Vector3.zero;
                    holder.transform.GetChild(i).GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }
            else {
                collision.gameObject.SetActive(false);
            }
        }
    }

    private Vector3 NameToTransform(string name) {
        for (int i = 0; i < ballNames.Length; i++)
            if (name == ballNames[i])
                return balls[i];
        return Vector3.zero;
    }

    public void Hit(int input) {
        PlaySound(0);
        whiteBall.GetComponent<PoolBall>().firstHit = true;
        if (input == 0)
            whiteBall.GetComponent<Rigidbody>().AddForce(Vector3
                .ProjectOnPlane(
                    (whiteBall.transform.position -
                     GameObject.Find("Player").GetComponentInChildren<Camera>().transform.position).normalized,
                    Vector3.up).normalized * 200);
        else
            whiteBall.GetComponent<Rigidbody>().AddForce(Vector3
                .ProjectOnPlane(
                    (whiteBall.transform.position -
                     GameObject.Find("Player 2").GetComponentInChildren<Camera>().transform.position).normalized,
                    Vector3.up).normalized * 200);
    }

    public void PlaySound(int type) {
        switch (type) {
            case 0:
                au.PlayOneShot(hitBoard);
                break;
            case 1:
                au.PlayOneShot(hitBall);
                break;
            case 2:
                au.PlayOneShot(firstHit);
                break;
            case 3:
                au.PlayOneShot(poolSink);
                break;
        }
    }
}