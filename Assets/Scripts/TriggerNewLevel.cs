using CommandPattern;
using System.Collections;
using UnityEngine;

public class TriggerNewLevel : MonoBehaviour {

    public GameObject NewLevel;
    public GameObject OldLevel;
    public GameObject character;
    public Vector3 position;
    
    private bool triggered = false;
	
    void Start()
    {
        if(character==null)
        {
            character = Utils.FindGameObjectsWithLayer(LayerMask.NameToLayer("OrigGO"));
            if (character == null)
                Debug.Log("NULL CHARACTER!");
        }
    }

	// Update is called once per frame
	void Update () {
        if (triggered) return;
        RaycastHit[] hits = Physics.RaycastAll(gameObject.transform.position, Vector3.up, 1, LayerMask.GetMask("OrigGO"));
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject == character)
                {
                    CreateNewLevel();
                    triggered = true;
                    break;
                }
            }
        }
    }

    void CreateNewLevel()
    {
        GameObject newLevel = Instantiate(NewLevel);
        newLevel.transform.position = position;
        new Task(RaiseLevel(newLevel, position, new Vector3(position.x, 0, position.z)));
    }

    public IEnumerator RaiseLevel(GameObject level, Vector3 begPosition, Vector3 endPosition)
    {
        character.GetComponent<InputHandler>().enabled = false;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            level.transform.position = Vector3.Lerp(begPosition, endPosition, t);
            yield return null;
        }
        new Task(Walk(character.transform.position, character.transform.position + new Vector3(0, 0, 2.5f)));
        yield return 0;
    }

    public IEnumerator Walk(Vector3 begPosition, Vector3 endPosition)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            character.transform.position = Vector3.Lerp(begPosition, endPosition, t);
            
            yield return null;
        }
        new Task(LowerLevel(OldLevel, OldLevel.transform.position));
        yield return 0;
    }

    public IEnumerator LowerLevel(GameObject level, Vector3 begPosition)
    {
        //Debug.Log("2: " + begPosition.y + " " + endPosition.y);
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            level.transform.position = Vector3.Lerp(begPosition, begPosition + new Vector3(0, position.y, 0), t);
            yield return null;
        }

        Destroy(OldLevel);
        character.GetComponent<InputHandler>().enabled = true;
        yield return 0;
    }

}
