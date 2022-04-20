using UnityEngine;

public class FadeDisappear : MonoBehaviour
{
    public enum ObjType { Mesh, Flat };
    public ObjType objType = ObjType.Mesh;

    public SpriteRenderer sprite;

    public bool delete = false;
    public float rate = 1.0f;
    private bool invisible = false;
    private bool fading = false;
    private Color color;
    private Renderer meshRend;

    private void Start()
    {
        if(objType == ObjType.Mesh)
        {
            meshRend = GetComponent<Renderer>();
            color = meshRend.material.color;
        }
        else
        {
            meshRend = sprite;
            color = meshRend.material.color;
        }
        
    }

    private void Update()
    {
        if(fading)
        {
            color.a -= Time.deltaTime * rate;
            meshRend.material.color = color;
        }
        if(color.a <= 0 && fading)
        {
            if(delete)
            {
                DeleteObj();
            }
            fading = false;
            color.a = 0;
            meshRend.material.color = color;
            invisible = true;
        }
    }

    public void StartFadingObj()
    {
        fading = true;
    }

    public bool IsInvisible()
    {
        return invisible;
    }

    public void SetVisible()
    {
        color.a = 1;
        meshRend.material.color = color;
        invisible = false;
    }

    private void DeleteObj()
    {
        color.a = 1;
        meshRend.material.color = color;
        Destroy(gameObject);
    }
}
