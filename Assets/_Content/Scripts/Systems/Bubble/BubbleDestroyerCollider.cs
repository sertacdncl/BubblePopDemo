using BubbleSystem;
using UnityEngine;

public class BubbleDestroyerCollider : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Bubble"))
		{
			var bubble = col.GetComponent<BubbleController>();
			BubbleManager.Instance.DestroyBubbleWithParticle(bubble);
			AudioManager.Instance.PlaySoundOnce("BubbleMerge",true);
		}
	}
}
