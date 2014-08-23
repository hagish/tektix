using UnityEngine;
using System.Collections;

public class UKDebugMesh : MonoBehaviour {
    public bool ShowVertex;
    public bool ShowNormals;
    public bool ShowBones;
    public bool ShowVertexColors;

    public bool DoNotTransform;

	void OnDrawGizmos () {
        Transform t = transform;
        Mesh m = null;

        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();

        if (smr != null)
        {
            m = smr.sharedMesh;

            if (ShowBones)
            {
                foreach (var bone in smr.bones)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(bone.transform.position, 0.025f);
                    Gizmos.DrawLine(bone.transform.position, bone.transform.parent.position);
                }
            }
        }

        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf != null)
        {
            m = smr.sharedMesh;
        }

        if (m != null)
        {
            if (ShowVertexColors)
            {
                for (int i = 0; i < m.vertexCount && i < m.colors.Length; ++i)
                {
                    Gizmos.color = m.colors[i];
                    var p = DoNotTransform ? m.vertices[i] : t.TransformPoint(m.vertices[i]);
                    Gizmos.DrawCube(p, Vector3.one * 0.025f);
                }
            }

            if (ShowVertex)
            {
                for (int i = 0; i < m.vertexCount; ++i)
                {
                    Gizmos.color = Color.magenta;
                    var p = DoNotTransform ? m.vertices[i] : t.TransformPoint(m.vertices[i]);
                    Gizmos.DrawSphere(p, 0.01f);
                }
            }

            if (ShowNormals)
            {
                for (int i = 0; i < m.vertexCount; ++i)
                {
                    var n = m.normals[i] * 0.02f;
                    Gizmos.color = Color.magenta;
                    var p = DoNotTransform ? m.vertices[i] : t.TransformPoint(m.vertices[i]);
                    Gizmos.DrawRay(p, n);
                }
            }
        }
	}
}
