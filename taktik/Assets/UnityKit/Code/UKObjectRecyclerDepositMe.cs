using UnityEngine;
using System.Collections;

public class UKObjectRecyclerDepositMe : MonoBehaviour {
	public UKObjectRecycler recycler;
	public string recyclerGroup;

	public void Deposit()
	{
		recycler.DepositObject(recyclerGroup, gameObject);
	}

	public void Deposit(float timeout)
	{
		Invoke("Deposit", timeout);
	}
}
