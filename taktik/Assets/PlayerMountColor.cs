using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
public class PlayerMountColor : MonoBehaviour {
    public Material Player0Material;
    public Material Player1Material;

    public Renderer Mount;

    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    void Start()
    {
        UpdatePlayerColor();
    }

	// Update is called once per frame
	void UpdatePlayerColor () 
    {
        Mount.material = unit.PlayerId == 0 ? Player0Material : Player1Material;
    }
}
