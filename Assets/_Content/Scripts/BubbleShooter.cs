using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleShooter : MonoBehaviour
{
	[SerializeField] private Camera mainCamera;
	[SerializeField] private Transform myBubble;
	[SerializeField] private LayerMask raycastLayers;
	public LayerMask BubbleMask;

	void Update()
	{
		GetDestination(GetMousePosition(myBubble.position));
	}
	
	//We use the path for everything but line must end where last hit location happend.
	//We can out the last hit point in the method but it would mean there will be a lot of unnecasy out vars.
	//So we extract it to this variable;
	Vector2 lastHitPoint;
	
	//This is hard coded becuase getting the information from the bubble would mean unnessary calculations at runtime.
	//Although calculations are not heavy and wouldn't make a difference there is no need to depend on a lower level object in the hierarcy.
	public float BubbleWidht = 0.5f;



	private Vector2 GetMousePosition(Vector2 casterPosition)
	{
		var mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
		var delta = mousePos - casterPosition;
		return delta;
	}

	private List<Vector3> GetDestination(Vector2 inputPosition)
	{
		Vector2 snapPoint = Vector2.zero;
		var ray = new Ray2D(myBubble.position, inputPosition);

		List<Vector3> path = new List<Vector3>();


		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, raycastLayers);


		if (hit)
		{
			Debug.DrawLine(myBubble.position, hit.centroid, Color.red);
			//I usually don't use tags too often but in this case it's faster than cheking by the component
			//and here we don't need to get the component.

			//Shot is directly to bubbles
			if (hit.collider.tag == "Bubble")
			{
				path.Add(ray.origin);
				snapPoint = GetSnapPoint(hit.normal, hit.collider.transform.position);
				path.Add(snapPoint);
				lastHitPoint = hit.point;
			}
			//Shot Bounces from wall
			else if (hit.collider.tag == "Wall")
			{
				// Get a rotation to go from our ray direction (negative, so coming from the wall),
				// to the normal of whatever surface we hit.
				var deflectRotation = Quaternion.FromToRotation(-ray.direction, hit.normal);

				// We then take that rotation and apply it to the same normal vector to basically
				// mirror that angle difference.
				var deflectDirection = deflectRotation * hit.normal;

				Ray deflectRay = new Ray(hit.centroid, deflectDirection);

				//When using a single cast hit retuns the original rays hit(Wall) we will just do a for loop in hits an see if we hit a bubble.
				RaycastHit2D[] bounceHit =
					Physics2D.RaycastAll(deflectRay.origin, deflectRay.direction, Mathf.Infinity, raycastLayers);


				//I would impliment a bounce method to support multible wall hits
				//Kind of zigzag movement but in example game 2 wall bounce used as cancellesion method
				//this is why I'm doing the same
				RaycastHit2D bubbleHit = new RaycastHit2D();
				for (int i = 0; i < bounceHit.Length; i++)
				{
					if (bounceHit[i].collider.tag == "Bubble")
					{
						Debug.DrawLine(deflectRay.origin, bounceHit[i].point, Color.blue);
						path.Add(ray.origin);
						path.Add(deflectRay.origin);
						bubbleHit = bounceHit[i];
						lastHitPoint = bubbleHit.point;
						break;
					}
				}

				if (bubbleHit)
				{
					snapPoint = GetSnapPoint(bubbleHit.normal, bubbleHit.transform.position);
					path.Add(snapPoint);
				}
			}
		}

		return path;
	}
	
	 Vector3 GetSnapPoint(Vector2 normal, Vector2 hitObjectPos)
    {
        Vector2 snapPoint = hitObjectPos;
        float bubbleWidht = BubbleWidht;

        List<Vector2> possiblePoints = new List<Vector2>();

        // Left and Right
        Vector2 right = hitObjectPos + Vector2.right * bubbleWidht;
        Vector2 left = hitObjectPos + Vector2.left * bubbleWidht;

        //Down Left and Right
        Vector2 lowerRight = new Vector2(hitObjectPos.x + bubbleWidht / 2, hitObjectPos.y - bubbleWidht / 1.1f);
        Vector2 lowerLeft = new Vector2(hitObjectPos.x - bubbleWidht / 2, hitObjectPos.y - bubbleWidht / 1.1f);

        //Upper Left and Right
        Vector2 upperRight = new Vector2(hitObjectPos.x + bubbleWidht / 2, hitObjectPos.y + bubbleWidht / 1.1f);
        Vector2 upperLeft = new Vector2(hitObjectPos.x - bubbleWidht / 2, hitObjectPos.y + bubbleWidht / 1.1f);

        //This is a really long if statement I usually avoid this kind of coding but in the case I had no choice and time is sort
        //I'm not great at math so I created a working logic. I tried better solutions but this on is working the bets for UX.
        //LowerHit
        if(normal.y < 0)
        {
            //Right
            if (normal.x > 0)
            {
                if (!IsOccupied(lowerRight))
                    snapPoint = lowerRight;
                else
                {
                    if (!IsOccupied(right))
                        snapPoint = right;
                    else
                    {
                        if (!IsOccupied(lowerLeft))
                            snapPoint = lowerLeft;
                        else
                        {
                            if (!IsOccupied(left))
                                snapPoint = left;  
                        }
                    }
                }
            }
            //Left
            else
            {
                if (!IsOccupied(lowerLeft))
                    snapPoint = lowerLeft;
                else
                {
                    if (!IsOccupied(left))
                        snapPoint = left;
                    else
                    {
                        if (!IsOccupied(lowerRight))
                            snapPoint = lowerRight;
                        else
                        {
                            if (!IsOccupied(right))
                                snapPoint = right;
                        }
                    }
                }
            }
        }
        //UpperHit
        else
        {
            //Right
            if (normal.x > 0)
            {
                if (!IsOccupied(upperRight))
                    snapPoint = upperRight;
                else
                {
                    if (!IsOccupied(right))
                        snapPoint = right;
                    else
                    {
                        if (!IsOccupied(upperLeft))
                            snapPoint = upperLeft;
                        else
                        {
                            if (!IsOccupied(left))
                                snapPoint = left;
                        }
                    }
                }
            }
            else
            {
                if (!IsOccupied(upperLeft))
                    snapPoint = upperLeft;
                else
                {
                    if (!IsOccupied(left))
                        snapPoint = left;
                    else
                    {
                        if (!IsOccupied(upperRight))
                            snapPoint = upperRight;
                        else
                        {
                            if (!IsOccupied(right))
                                snapPoint = right;
                        }
                    }
                }
            }
        }

        if(normal.y == 0)
        {
            if (normal.x > 0)
                snapPoint = right;
            else snapPoint = left;
        }

        return snapPoint;
    }
	
	private bool IsOccupied(Vector2 pos)
	{
		return Physics2D.OverlapCircle(pos, BubbleWidht / 2.2f, BubbleMask);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		// Gizmos.DrawWireSphere(BubbleIndicator.transform.position, BubbleWidht / 2.2f);
	}
}