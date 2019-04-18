using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	public float maxHealth = 100f;
	public float currentHealth { get; private set; }

	public Stat damage;
	public Stat armor;

	private void Awake()
	{
		currentHealth = maxHealth;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			TakeDamage( 10 );
		}
	}

	public void TakeDamage(float damage)
	{
		damage -= armor.GetValue();
		damage = Mathf.Clamp( damage, 0, float.MaxValue );

		currentHealth -= damage;
		Debug.Log(transform.name + " takes " + damage + " damage");

		if(currentHealth <= 0)
		{
			Die();
		}
	}

	public virtual void Die()
	{
		// This method is ment to be overwirtten
		Debug.Log( transform.name + " died." );
	}
}
