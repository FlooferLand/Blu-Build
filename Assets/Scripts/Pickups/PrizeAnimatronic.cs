using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PrizeAnimatronic : MonoBehaviour {
    public enum PrizeType {
        basic,
        animatronic,
        buildingblock
    }

    public float mass = 3.0f;
    public PrizeType prizeType;

    public void AttStartup() {
        switch (prizeType) {
            case PrizeType.basic:
                GetComponent<Rigidbody>().mass = mass;
                gameObject.layer = 9;
                break;
            case PrizeType.animatronic:
                gameObject.AddComponent<Rigidbody>();
                GetComponent<Rigidbody>().mass = mass;
                gameObject.layer = 9;
                GetComponent<BoxCollider>().enabled = true;
                GetComponentInChildren<Character_Valves>().StartUp();
                break;
            case PrizeType.buildingblock:
                GetComponent<Rigidbody>().mass = mass;
                gameObject.layer = 17;
                break;
        }
    }

    public void ATTSkin(string mesh) {
        switch (prizeType) {
            case PrizeType.basic:
                //Deletes all meshes under the first gameobject that aren't the string name or "Armature"
                for (int i = 0; i < transform.childCount; i++)
                    if (transform.GetChild(i).gameObject.name != mesh &&
                        transform.GetChild(i).gameObject.name != "Armature")
                        Destroy(transform.GetChild(i).gameObject);
                break;
            case PrizeType.animatronic:
                //Deletes all meshes under the first gameobject that aren't the string name or "Armature"
                for (int i = 0; i < transform.GetChild(0).childCount; i++)
                    if (transform.GetChild(0).GetChild(i).gameObject.name != mesh &&
                        transform.GetChild(0).GetChild(i).gameObject.name != "Armature")
                        Destroy(transform.GetChild(0).GetChild(i).gameObject);
                break;
            case PrizeType.buildingblock:
                var gg = GameObject.Find("Global Controller").GetComponent<GlobalController>();
                for (int i = 0; i < gg.buildMaterials.Length; i++)
                    if (gg.buildMaterials[i].name == mesh) {
                        var mr = GetComponentsInChildren<MeshRenderer>();
                        Material[] mat;
                        for (int e = 0; e < mr.Length; e++) {
                            mat = mr[e].materials;
                            for (int f = 0; f < mat.Length; f++) {
                                var col = mat[f].GetColor("_BaseColor");
                                mat[f] = Instantiate(gg.buildMaterials[i].mat);
                                mat[f].SetColor("_BaseColor", col);
                            }

                            mr[e].materials = mat;
                        }
                    }

                break;
        }
    }

    public void ATTScale(float scale) {
        transform.localScale = new Vector3(transform.localScale.x * scale, transform.localScale.y * scale,
            transform.localScale.z * scale);
        GetComponent<Rigidbody>().mass *= scale;
    }

    public void ATTComplexScale(Vector3 scale) {
        transform.localScale = new Vector3(transform.localScale.x * scale.x, transform.localScale.y * scale.y,
            transform.localScale.z * scale.z);
    }

    public void ATTLightTemperature(int temp) {
        var light = GetComponentsInChildren<Light>();
        for (int i = 0; i < light.Length; i++) light[i].colorTemperature = temp;
    }

    public void ATTLightIntensity(float intensity) {
        var light = GetComponentsInChildren<HDAdditionalLightData>();
        for (int i = 0; i < light.Length; i++) light[i].SetIntensity(intensity);
    }

    public void ATTLightColor(Color color) {
        var light = GetComponentsInChildren<HDAdditionalLightData>();
        for (int i = 0; i < light.Length; i++) light[i].SetColor(color);
    }

    public void ATTLightSpotSize(Vector2 both) {
        var light = GetComponentsInChildren<HDAdditionalLightData>();
        for (int i = 0; i < light.Length; i++)
            if (light[i].gameObject.name != "IGNORE")
                light[i].SetSpotAngle(both.x, both.y);
    }

    public void ATTMaterialColor(Color color) {
        Debug.Log(name + " " + color);
        var mr = GetComponentsInChildren<MeshRenderer>();
        Material[] mat;
        if (prizeType == PrizeType.buildingblock)
            for (int i = 0; i < mr.Length; i++) {
                mat = mr[i].materials;
                for (int e = 0; e < mat.Length; e++) mat[e].SetColor("_BaseColor", color);
                mr[i].materials = mat;
            }
        else
            for (int i = 0; i < mr.Length; i++) {
                mat = mr[i].materials;
                mat[0].SetColor("_BaseColor", color);
                mr[i].materials = mat;
            }
    }
}