using UnityEngine;

public class MeshOnParent : MonoBehaviour
{
    void Start()
    {
        if (transform.position != transform.parent.position)
        {
            transform.position = transform.parent.position;
        }
    }
}
