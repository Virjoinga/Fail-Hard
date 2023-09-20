using UnityEngine;

[RequireComponent(typeof(Spline))]
public class SplineAutoAlign : MonoBehaviour
{
	public LayerMask raycastLayers = -1;

	public float offset = 0.1f;

	public string[] ignoreTags;

	public Vector3 raycastDirection = Vector3.down;

	public void AutoAlign()
	{
		if (raycastDirection.x == 0f && raycastDirection.y == 0f && raycastDirection.z == 0f)
		{
			Debug.LogWarning(base.gameObject.name + ": The raycast direction is zero!", base.gameObject);
			return;
		}
		Spline component = GetComponent<Spline>();
		SplineNode[] splineNodes = component.SplineNodes;
		foreach (SplineNode splineNode in splineNodes)
		{
			RaycastHit[] array = Physics.RaycastAll(splineNode.Position, raycastDirection, float.PositiveInfinity, raycastLayers);
			RaycastHit raycastHit = default(RaycastHit);
			raycastHit.distance = float.PositiveInfinity;
			RaycastHit[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				RaycastHit raycastHit2 = array2[j];
				bool flag = false;
				string[] array3 = ignoreTags;
				foreach (string text in array3)
				{
					if (raycastHit2.transform.tag == text)
					{
						flag = true;
					}
				}
				if (!flag && raycastHit.distance > raycastHit2.distance)
				{
					raycastHit = raycastHit2;
				}
			}
			if (raycastHit.distance != float.PositiveInfinity)
			{
				splineNode.Transform.position = raycastHit.point - raycastDirection * offset;
			}
		}
	}
}
