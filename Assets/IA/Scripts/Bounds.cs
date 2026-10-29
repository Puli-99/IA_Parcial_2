using UnityEngine;

public class Bounds : MonoBehaviour
{
    public static Bounds Instance { get; private set; }

    [SerializeField] float height;
    [SerializeField] float width;
    [SerializeField] bool drawGizmos;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3 OutOfBounds(Vector3 position) // esta toma version toma en cuenta la posicion del objeto
    {
        Vector3 center = transform.position;
        Vector3 offset = position - center;

        float halfW = width / 2f;
        float halfH = height / 2f;

        if (offset.x > halfW) offset.x = -halfW;
        else if (offset.x < -halfW) offset.x = halfW;

        if (offset.z > halfH) offset.z = -halfH;
        else if (offset.z < -halfH) offset.z = halfH;

        return center + offset;
    }

    /*public Vector3 OutOfBounds(Vector3 position)
    {
        Vector3 newPos = position;

        if (position.x > width / 2) newPos.x = -width / 2;
        if (position.x < -width / 2) newPos.x = width / 2;

        if (position.z > height / 2) newPos.z = -height / 2;
        if (position.z < -height / 2) newPos.z = height / 2;

        return newPos;

    }*/

    void OnDrawGizmos()
    {
        if(!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(width,0 ,height));
    }
}
