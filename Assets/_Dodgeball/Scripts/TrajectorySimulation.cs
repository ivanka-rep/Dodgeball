using System.Collections.Generic;
using Items;
using UnityEngine;
 
/// <summary>
/// Controls the Laser Sight for the player's aim
/// </summary>
public class TrajectorySimulation : MonoBehaviour
{
	// Reference to the LineRenderer we will use to display the simulated path
	// public LineRenderer sightLine;
 
	// Reference to a Component that holds information about fire strength, location of cannon, etc.
	//public PlayerFire playerFire;
 
	// Number of segments to calculate - more gives a smoother line
	public int segmentCount = 2;
 
	// Length scale for each segment
	public float segmentScale = 1f;
 
	// gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
	private Collider _hitObject;
	public Collider hitObject => _hitObject;

	private ThrowingController m_throwCtrl;
	// private Color m_ballColor;
	private APRController m_aprCtrl;
	private GameManager m_gameManager;

	private List<GameObject> m_spheres;

	private GameData m_gameData;
	
	void Start()
	{
		m_throwCtrl = GameManager.s_instance.Players[0].GetComponent<ThrowingController>();
		m_gameData = GameData.s_instance;
		m_gameManager = GameManager.s_instance;
		m_spheres = new List<GameObject>();
		// m_ballColor = gameObject.GetComponent<MeshRenderer>().materials[0].color;
		m_aprCtrl = m_throwCtrl.gameObject.GetComponent<APRController>();

		m_spheres = m_gameManager.SpheresPool.PooledObjects;
	}
	
	void FixedUpdate()
	{
		simulatePath();
	}
 
	/// <summary>
	/// Simulate the path of a launched ball.
	/// Slight errors are inherent in the numerical method used.
	/// </summary>
	void simulatePath()
	{
		var ballTransform = m_throwCtrl.ball.transform;
		
		Vector3[] segments = new Vector3[segmentCount];
 
		// The first line point is wherever the player's cannon, etc is
		segments[0] = ballTransform.position;
 
		// The initial velocity
		var dir = m_aprCtrl.Root.transform.forward;
		var throwForce = GameManager.s_instance.DifficultyParams.playerThrowForce;   
		
		ItemBall itemBall = (ItemBall)m_gameData.Items.Find(item => item.ItemId == m_gameData.UserData.CurrentBallId);
		float distance = (itemBall.GetDistance() / 100f) + 1f; // Numbers between 1 and 2; 
		float speed = (itemBall.GetSpeed() / 100f) + 1f; // Numbers between 1 and 2; 
		
		Vector3 segVelocity = (dir * (throwForce * speed * m_throwCtrl.ChangingAdditionalForce) + Vector3.up * distance);
 
		// reset our hit object
		_hitObject = null;
 
		for (int i = 1; i < segmentCount; i++)
		{
			// Time it takes to traverse one segment of length segScale (careful if velocity is zero)
			float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;
 
			// Add velocity from gravity for this segment's timestep
			segVelocity = segVelocity + Physics.gravity * segTime;
 
			// Check to see if we're going to hit a physics object
			if (Physics.Raycast(segments[i - 1], segVelocity, out RaycastHit hit, segmentScale, LayerMask.NameToLayer("LimitWall")))
			{
				// remember who we hit
				_hitObject = hit.collider;
 
				// set next position to the position where we hit the physics object
				segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
				// correct ending velocity, since we didn't actually travel an entire segment
				segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
				// flip the velocity to simulate a bounce
				segVelocity = Vector3.Reflect(segVelocity, hit.normal);
 
				/*
				 * Here you could check if the object hit by the Raycast had some property - was 
				 * sticky, would cause the ball to explode, or was another ball in the air for 
				 * instance. You could then end the simulation by setting all further points to 
				 * this last point and then breaking this for loop.
				 */
			}
			// If our raycast hit no objects, then set the next position to the last one plus v*t
			else
			{
				segments[i] = segments[i - 1] + segVelocity * segTime;
			}
		}
 
		// At the end, apply our simulations to the LineRenderer
 
		// // Set the colour of our path to the colour of the next ball
		// Color startColor = m_ballColor;
		// Color endColor = startColor;
		// startColor.a = 1;
		// endColor.a = 0;
		// sightLine.startColor = startColor;
		// sightLine.endColor = endColor;
 	//
		// sightLine.positionCount = segmentCount;
		// for (int i = 0; i < segmentCount; i++)
		// 	sightLine.SetPosition(i, segments[i]);

		for (int i = 0; i < segmentCount; i++)
		{
			GameObject sphere = m_spheres[i];
			sphere.SetActive(true);
			sphere.transform.position = segments[i];
		}
	}
}