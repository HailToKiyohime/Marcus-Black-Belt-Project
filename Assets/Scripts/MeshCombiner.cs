using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public bool combineOnStart = true;

    void Start()
    {
        if (combineOnStart)
            CombineMeshes();
    }

    [ContextMenu("Combine Meshes")]
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        Material sharedMat = null;

        int i = 0;
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf == this.GetComponent<MeshFilter>())
                continue;

            if (mf.GetComponent<Renderer>() && sharedMat == null)
                sharedMat = mf.GetComponent<Renderer>().sharedMaterial;

            combine[i].mesh = mf.sharedMesh;
            combine[i].transform = mf.transform.localToWorldMatrix;
            i++;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // for large meshes
        combinedMesh.CombineMeshes(combine, true, true);

        MeshFilter mfMain = GetComponent<MeshFilter>();
        mfMain.mesh = combinedMesh;

        MeshRenderer mrMain = GetComponent<MeshRenderer>();
        if (sharedMat != null)
            mrMain.material = sharedMat;

        // Disable children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        Debug.Log("Mesh Combine Complete");
    }
}
