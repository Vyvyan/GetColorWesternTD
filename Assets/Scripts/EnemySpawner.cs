using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public string wave1, wave2, wave3, wave4, wave5, wave6, wave7, wave8, wave9, wave10, wave11, wave12, wave13, wave14, wave15, wave16, wave17, wave18, wave19, wave20, wave21, wave22, wave23, wave24, wave25, wave26, wave27, wave28, wave29, wave30;
    public string[] WaveStrings;

	// Use this for initialization
	void Start ()
    {
        WaveStrings = new string[] { wave1, wave2, wave3, wave4, wave5, wave6, wave7, wave8, wave9, wave10, wave11, wave12, wave13, wave14, wave15, wave16, wave17, wave18, wave19, wave20, wave21, wave22, wave23, wave24, wave25, wave26, wave27, wave28, wave29, wave30 };
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}
}
