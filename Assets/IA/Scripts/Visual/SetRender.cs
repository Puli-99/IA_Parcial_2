using UnityEngine;

public class SetRender : MonoBehaviour
{
    [SerializeField] bool renderersEnabled;

    void Update()
    {
        Change();
    }
    void Change()
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.enabled = renderersEnabled;
        }
    }
}
