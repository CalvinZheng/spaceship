using UnityEngine;
using System.Collections;

//float size = (MeteorController) meteor.GetComponent(typeof(MeteorController)).meteorSize;

public class InitGame : MonoBehaviour {
	public GameObject theSpaceship;
	public GameObject background;
	public GameObject meteor;
	public GameObject endPoint;
	public int meteorAmount;

	public static ArrayList allMeteors;
	public static ArrayList visibleMeteors;
	public static GameObject spaceship;
	public static GameObject theBackgound;

	float xyForce;
	float zForce;
	int visibleDistance;
	int endPosition;

	public static bool gameFinished;
	public static bool gameWon;
	
	// Use this for initialization
	void Start () {
		gameFinished = false;
		gameWon = false;

		allMeteors = new ArrayList ();
		visibleMeteors = new ArrayList ();

		xyForce = 2f;
		zForce = 2f;
		visibleDistance = 50;
		endPosition = 200;

		spaceship = (GameObject)Instantiate (theSpaceship, new Vector3 (0, -1, 0), Quaternion.identity);

		while (allMeteors.Count < meteorAmount) {
			this.CreateNewMeteor(true);
		}

		//theBackgound = (GameObject)Instantiate (background, new Vector3 (0, 0, visibleDistance * 1.5f), Quaternion.identity);

	}
	
	// Update is called once per frame
	void Update () {
		if (gameFinished) {
			spaceship.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
			if (gameWon) {
				GameObject.Find("Text").GetComponent<GUIText>().text = "YOU WON!";
			} else {
				GameObject.Find("Text").GetComponent<GUIText>().text = "YOU LOSE!";
			}
		} else {
			// remove destroyed meteors
			this.cleanDestroyedMeteor ();

			// add meteors
			while (allMeteors.Count < meteorAmount) {
				this.CreateNewMeteor (false);
			}

			// only show meteors in visible distance
			foreach (GameObject meteor in allMeteors) {
				if (((GameObject)meteor).GetComponent<Renderer>().bounds.Intersects(spaceship.gameObject.GetComponent<Renderer>().bounds)){
					gameFinished = true;
					gameWon = false;
				}
				if ((((GameObject)meteor).transform.position.z - spaceship.transform.position.z) > visibleDistance) {
					((GameObject)meteor).GetComponent<MeshRenderer> ().enabled = false;
				} else {
					((GameObject)meteor).GetComponent<MeshRenderer> ().enabled = true;
				}
			}

//			if (spaceship.transform.position.z > 300){
//				gameFinished = true;
//				gameWon = false;
//			}

			if (Input.GetKey (KeyCode.UpArrow)) {
				spaceship.GetComponent<Rigidbody>().AddForce (new Vector3 (0, xyForce, 0));
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				spaceship.GetComponent<Rigidbody>().AddForce (new Vector3 (0, -1 * xyForce, 0));
			}
			if (Input.GetKey (KeyCode.LeftArrow)) {
				spaceship.GetComponent<Rigidbody>().AddForce (new Vector3 (-1 * xyForce, 0, 0));
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				spaceship.GetComponent<Rigidbody>().AddForce (new Vector3 (xyForce, 0, 0));
			}
			if (Input.GetKey (KeyCode.W)) {
				spaceship.GetComponent<Rigidbody>().AddForce (new Vector3 (0, 0, zForce));
			}
			if (Input.GetKey (KeyCode.S)) {
				if (spaceship.GetComponent<Rigidbody>().velocity.z < 0) {
					spaceship.GetComponent<Rigidbody>().velocity = new Vector3 (spaceship.GetComponent<Rigidbody>().velocity.x, spaceship.GetComponent<Rigidbody>().velocity.y, 0);
				} else {
					spaceship.GetComponent<Rigidbody>().AddForce (new Vector3 (0, 0, -1 * zForce));
				}
			}

			this.gameObject.transform.position = new Vector3 (spaceship.transform.position.x, spaceship.transform.position.y + 3, spaceship.transform.position.z - 5);
			// theBackgound.transform.position = new Vector3 (0, 0, spaceship.transform.position.z + visibleDistance * 1.5f);

			GameObject.Find("InfoText").GetComponent<GUIText>().text = "Your Position: (" + spaceship.transform.position.x 
																		+ ", " + spaceship.transform.position.y 
																		+ ", " + spaceship.transform.position.z 
																		+ ")\nGoal Position: (0, 0, " + endPosition +")";

			this.CheckVisibilityOfMeteor();

			// this is for checking visible meteors

//			foreach (GameObject meteor in visibleMeteors) {
//				meteor.renderer.material.color = Color.red;
//			}

		}
	}

	public void CreateNewMeteor(bool isStart){
		GameObject newMeteor;
		float x, y, z;
		if (isStart) {
			x = spaceship.transform.position.x + Random.Range (-50f, 50f);
			y = spaceship.transform.position.y + Random.Range (-35f, 35f);
			z = Random.Range (4f, visibleDistance * 2);
		} else {
			float xDiff, yDiff;
			float[] meteorsXY = this.averageXYOfMeteors();
			xDiff = spaceship.transform.position.x - meteorsXY[0];
			yDiff = spaceship.transform.position.y - meteorsXY[1];

			if ( xDiff > 0 && Mathf.Abs(xDiff) > 3) {
				// spaceship too right
				x = spaceship.transform.position.x + Random.Range (30f, 60f);
				z = Random.Range (spaceship.transform.position.z, spaceship.transform.position.z + visibleDistance);
			} else if ( xDiff < 0 && Mathf.Abs(xDiff) > 3){
				x = spaceship.transform.position.x + Random.Range (-60f, -30f);
				z = Random.Range (spaceship.transform.position.z, spaceship.transform.position.z + visibleDistance);
			} else {
				x = spaceship.transform.position.x + Random.Range (-50f, 50f);
				z = Random.Range (spaceship.transform.position.z + visibleDistance, spaceship.transform.position.z + 2 * visibleDistance);
			}
			if ( yDiff > 0 && Mathf.Abs(yDiff) > 3) {
				// spaceship too high
				y = spaceship.transform.position.y + Random.Range (10f, 40f);
				z = Random.Range (spaceship.transform.position.z, spaceship.transform.position.z + visibleDistance);
			} else if ( yDiff < 0 && Mathf.Abs(yDiff) > 3 ){
				y = spaceship.transform.position.y + Random.Range (-40f, -10f);
				z = Random.Range (spaceship.transform.position.z, spaceship.transform.position.z + visibleDistance);
			} else {
				y = spaceship.transform.position.y + Random.Range (-35f, 35f);
				z = Random.Range (spaceship.transform.position.z + visibleDistance, spaceship.transform.position.z + 2 * visibleDistance);
			}
		}

		float size = Random.Range (0.5f, 3.0f);
		Vector3 position = new Vector3 (x, y, z);
		Vector3 existingMeteorPosition;

		foreach (GameObject meteor in allMeteors) {
			existingMeteorPosition = meteor.transform.position;
			if (Mathf.Abs(Vector3.Distance(existingMeteorPosition, position)) < (((MeteorController)meteor.GetComponent(typeof(MeteorController))).meteorSize + size)){
				this.CreateNewMeteor(isStart);
				return;
			}
		}
		newMeteor = (GameObject)Instantiate (this.meteor, position, Quaternion.identity);

		MeteorController meteorScript = (MeteorController) newMeteor.GetComponent(typeof(MeteorController));
		meteorScript.meteorSize = size;

		this.CreateMeteorMesh(newMeteor, size);
		newMeteor.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f)));
		allMeteors.Add (newMeteor);
	}

	public float[] averageXYOfMeteors(){
		float[] xy = new float[2];
		float x = 0, y = 0;
		foreach (GameObject meteor in allMeteors) {
			x += meteor.transform.position.x;
			y += meteor.transform.position.y;
		}
		x = x / allMeteors.Count;
		y = y / allMeteors.Count;
		xy [0] = x;
		xy [1] = y;
		return xy;
	}

	public void CreateMeteorMesh(GameObject newMeteor, float size){
		//Reference URL: http://wiki.unity3d.com/index.php/ProceduralPrimitives
		Mesh mesh  = newMeteor.GetComponent<MeshFilter>().mesh;

		float radius = size;
		// Longitude |||
		int nbLong = Mathf.Max (10, (int)Mathf.Ceil (3 * radius));
		// Latitude ---
		int nbLat = Mathf.Max (10, (int)Mathf.Ceil (3 * radius));
		
		#region Vertices
		Vector3[] vertices = new Vector3[(nbLong+1) * nbLat + 2];
		float pi = Mathf.PI;
		float twoPi = pi * 2f;
		float randomDeformationX, randomDeformationY, randomDeformationZ;
		
		vertices[0] = Vector3.up * radius;
		for( int lat = 0; lat < nbLat; lat++ )
		{
			float temp1 = pi * (float)(lat+1) / (nbLat+1);
			float sin1 = Mathf.Sin(temp1);
			float cos1 = Mathf.Cos(temp1);
			
			for( int lon = 0; lon <= nbLong; lon++ )
			{
				float temp2 = twoPi * (float)(lon == nbLong ? 0 : lon) / nbLong;
				float sin2 = Mathf.Sin(temp2);
				float cos2 = Mathf.Cos(temp2);

				randomDeformationX = Random.Range (-0.1f * size, 0.1f * size);
				randomDeformationY = Random.Range (-0.1f * size, 0.1f * size);
				randomDeformationZ = Random.Range (-0.1f * size, 0.1f * size);
				vertices[ lon + lat * (nbLong + 1) + 1] = new Vector3( sin1 * cos2 + randomDeformationX, cos1 + randomDeformationY, sin1 * sin2 + randomDeformationZ ) * radius;
			}
		}
		vertices[vertices.Length-1] = Vector3.up * -radius;
		#endregion
		
		#region Normales		
		Vector3[] normales = new Vector3[vertices.Length];
		for( int j = 0; j < vertices.Length; j++ )
			normales[j] = vertices[j].normalized;
		#endregion
		
		#region UVs
		Vector2[] uvs = new Vector2[vertices.Length];
		uvs[0] = Vector2.up;
		uvs[uvs.Length-1] = Vector2.zero;
		for( int lat = 0; lat < nbLat; lat++ )
			for( int lon = 0; lon <= nbLong; lon++ )
				uvs[lon + lat * (nbLong + 1) + 1] = new Vector2( (float)lon / nbLong, 1f - (float)(lat+1) / (nbLat+1) );
		#endregion
		
		#region Triangles
		int nbFaces = vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3 + nbLat * 3;
		int[] triangles = new int[ nbIndexes + 6 ];
		
		//Top Cap
		int i = 0;
		for( int lon = 0; lon < nbLong + 1; lon++ )
		{
			triangles[i++] = lon+2;
			triangles[i++] = lon+1;
			triangles[i++] = 0;
		}

		// fill top hole
		triangles[i++] = 1;
		triangles[i++] = nbLong + 1;
		triangles[i++] = 0;

		// fill hole in first latitude
		triangles[i++] = nbLong + 1;
		triangles[i++] = 1;
		triangles[i++] = nbLong + 1 + nbLong;

		triangles[i++] = nbLong + 1;
		triangles[i++] = 1;
		triangles[i++] = nbLong + 2 + nbLong;

		triangles[i++] = nbLong + 1;
		triangles[i++] = nbLong + 2 + nbLong;
		triangles[i++] = nbLong + 1 + nbLong;

		triangles[i++] = 1;
		triangles[i++] = nbLong + 2 + nbLong;
		triangles[i++] = nbLong + 1 + nbLong;


		//Middle
		for( int lat = 0; lat < nbLat - 1; lat++ )
		{
			for( int lon = 0; lon < nbLong + 1; lon++ )
			{
				int current = lon + lat * (nbLong + 1) + 1;
				int next = current + nbLong + 1;
				
				triangles[i++] = current;
				triangles[i++] = current + 1;
				triangles[i++] = next + 1;
				
				triangles[i++] = current;
				triangles[i++] = next + 1;
				triangles[i++] = next;

//				if (lon == nbLong) {	
//					current = lat * (nbLong + 1) + 1 + nbLong;
//					next = current + nbLong + 1;
//					triangles[i++] = current;
//					triangles[i++] = current - nbLong;
//					triangles[i++] = next + 1 - nbLong;
//				}
			}
		}
		
		//Bottom Cap
		for( int lon = 0; lon < nbLong; lon++ )
		{
			triangles[i++] = vertices.Length - 1;
			triangles[i++] = vertices.Length - (lon+2) - 1;
			triangles[i++] = vertices.Length - (lon+1) - 1;
		}
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		//mesh.Optimize();

		newMeteor.GetComponent<MeshFilter> ().mesh = mesh;
		newMeteor.GetComponent<MeshCollider> ().sharedMesh = mesh;
		newMeteor.GetComponent<Rigidbody> ().mass = size;

		copyMaterial (newMeteor);
	}

	public void cleanDestroyedMeteor () {
		for (int i = 0; i < allMeteors.Count; i++) {
			if ((GameObject)allMeteors [i] == null) {
				allMeteors.RemoveAt (i);
				i--;
			} 
		}
	}

	public void CheckVisibilityOfMeteor(){
		visibleMeteors.Clear ();
		RaycastHit hitMeteor = new RaycastHit ();
		for (float i = -50; i < 50; i++) {
			for (float j = -50; j < 50; j++) {
				if (Physics.Raycast (new Vector3 (spaceship.transform.position.x, spaceship.transform.position.y, spaceship.transform.position.z + 1),
				                     new Vector3 (i, j, 20), out hitMeteor)) {
					if (!visibleMeteors.Contains (hitMeteor.transform.gameObject) 
					    && hitMeteor.transform.gameObject.GetComponent<MeshRenderer> ().enabled == true
					    && hitMeteor.transform.gameObject != endPoint) {
						visibleMeteors.Add (hitMeteor.transform.gameObject);
					}
				}
			}
		}
	}

	public void CheckVisibilityOfMeteor2(){
		visibleMeteors.Clear ();

		float x, y, z, radius;
		Vector3 eyePosition;
		RaycastHit hitMeteor = new RaycastHit ();

		foreach (GameObject meteor in allMeteors) {
			x = meteor.transform.position.x;
			y = meteor.transform.position.y;
			z = meteor.transform.position.z;
			radius = ((MeteorController)meteor.GetComponent(typeof(MeteorController))).meteorSize - 0.1f;
			eyePosition = new Vector3 (spaceship.transform.position.x, spaceship.transform.position.y, spaceship.transform.position.z + 1);

			if (Physics.Raycast (eyePosition, new Vector3 (x + radius, y, z), out hitMeteor)
			    || Physics.Raycast (eyePosition, new Vector3 (x - radius, y, z), out hitMeteor)
			    || Physics.Raycast (eyePosition, new Vector3 (x, y + radius, z), out hitMeteor)
			    || Physics.Raycast (eyePosition, new Vector3 (x, y - radius, z), out hitMeteor)
			    || Physics.Raycast (eyePosition, new Vector3 (x, y, z - radius), out hitMeteor)) {
				if (!visibleMeteors.Contains (hitMeteor.transform.gameObject) 
				    && hitMeteor.transform.gameObject.GetComponent<MeshRenderer> ().enabled == true
				    && hitMeteor.transform.gameObject != endPoint) {
					visibleMeteors.Add (hitMeteor.transform.gameObject);
				}
			}
		}

	}

	void copyMaterial(GameObject anObject)
	{
		anObject.GetComponent<Renderer>().material = new Material (anObject.GetComponent<Renderer>().material);
	}
}
