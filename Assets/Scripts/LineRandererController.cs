using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRandererController : MonoBehaviour
{
    [SerializeField] List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public void SetPosition(Transform starPos, Transform endPos)
    {
        if (lineRenderers.Count > 0)
        {
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                if (lineRenderers[i].positionCount >= 2)
                {
                    lineRenderers[i].SetPosition(0, starPos.position);
                    lineRenderers[i].SetPosition(1, endPos.position);
                }
                else
                {
                    Debug.Log("해당 랜더러는 2개의 점이 있어야 합니다.");
                }
            }

        }
    }
}
