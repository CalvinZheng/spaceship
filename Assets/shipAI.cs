using UnityEngine;
using System.Collections;

public class shipAI : MonoBehaviour {

	public int AIType;
	public Transform endPoint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		GameObject ship = InitGame.spaceship;
		ArrayList meteors = InitGame.visibleMeteors;

		Vector3 force = (endPoint.position - ship.transform.position).normalized;

		if ((endPoint.position - ship.transform.position).magnitude < 50)
		{
			Vector3 direction = force;
			force = Vector3.zero;
			float distance = (endPoint.position - ship.transform.position).magnitude;
			Vector3 shipV = ship.GetComponent<Rigidbody>().velocity;
			float shipVy = Vector3.Dot(shipV, direction);
			float forceY = shipVy * shipVy / 2 / distance;
			force += forceY * direction;
			Vector3 shipVx = shipV - shipVy*direction;
			force -= 2*shipVx.magnitude*forceY/shipVy * shipVx.normalized;
		}

		switch (AIType)
		{
		case 0:
			force = Vector3.zero;
			break;
		case 1:
			for (int i = 0; i < meteors.Count; i++)
			{
				GameObject aMeteor = (GameObject)meteors [i];
				if (aMeteor == null || !aMeteor.GetComponent<Rigidbody>())
					continue;
				float distance = (aMeteor.transform.position - ship.transform.position).magnitude;
				float size = ((MeteorController) aMeteor.GetComponent(typeof(MeteorController))).meteorSize;
				force +=  5 * size / distance / distance * (ship.transform.position - aMeteor.transform.position).normalized;
			}

			force = force.normalized;
			break;
		case 2:
			for (int i = 0; i < meteors.Count; i++)
			{
				GameObject aMeteor = (GameObject)meteors [i];

				if (aMeteor == null || !aMeteor.GetComponent<Rigidbody>())
					continue;

				float distance = (aMeteor.transform.position - ship.transform.position).magnitude;
				Vector3 direction = (aMeteor.transform.position - ship.transform.position).normalized;
				Vector3 shipV = ship.GetComponent<Rigidbody>().velocity;
				Vector3 meteorV = aMeteor.GetComponent<Rigidbody>().velocity;
				float relativeV = Vector3.Dot(shipV, direction) + Vector3.Dot(meteorV, -direction);
				float size = ((MeteorController) aMeteor.GetComponent(typeof(MeteorController))).meteorSize;

				if (distance - size - 1 < 0.5*relativeV*relativeV)
				{
					aMeteor.GetComponent<Renderer>().material.color = Color.yellow;
					force += 0.8f * relativeV * relativeV / 2 / (distance - size - 1) * -direction;
				}
				else
				{
					aMeteor.GetComponent<Renderer>().material.color = Color.white;
				}
			}
			
			force = force.normalized;
			break;
		case 3:

			if (ship.GetComponent<Rigidbody>().velocity.magnitude > 10 && Vector3.Dot(ship.GetComponent<Rigidbody>().velocity, force) > 0)
			{
				force *= 0.1f;
			}

			for (int i = 0; i < meteors.Count; i++)
			{
				GameObject aMeteor = (GameObject)meteors [i];
				
				if (aMeteor == null || !aMeteor.GetComponent<Rigidbody>())
					continue;
				
				float distance = (aMeteor.transform.position - ship.transform.position).magnitude;
				Vector3 direction = (aMeteor.transform.position - ship.transform.position).normalized;
				Vector3 shipV = ship.GetComponent<Rigidbody>().velocity;
				Vector3 meteorV = aMeteor.GetComponent<Rigidbody>().velocity;
				float relativeV = Vector3.Dot(shipV, direction) + Vector3.Dot(meteorV, -direction);
				float size = ((MeteorController) aMeteor.GetComponent(typeof(MeteorController))).meteorSize;

				float impactTime = (distance - size - 1) / relativeV;
				Vector3 meteorMove = (meteorV - Vector3.Dot(meteorV, -direction)*-direction) * impactTime;
				Vector3 shipMove = (shipV - Vector3.Dot(shipV, direction)*direction) * impactTime;

				if ((meteorMove - shipMove).magnitude < size + 2)
				{
					aMeteor.GetComponent<Renderer>().material.color = Color.yellow;
					float dodge = (meteorMove - shipMove).magnitude - size - 2;
					force += (meteorMove - shipMove).normalized * 2 * 2 * dodge / impactTime / impactTime;
				}
				else
				{
					aMeteor.GetComponent<Renderer>().material.color = Color.white;
				}
			}
			
			force = force.normalized;

			break;
		}

		Debug.DrawLine (ship.transform.position, ship.transform.position + force * 5);

		ship.GetComponent<Rigidbody> ().AddForce (force);
	}
}
