﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
	public static EquipmentManager instance;

	private void Awake()
	{
		instance = this;
	}

	public delegate void OnEquipmentChanged( Equipment newItem, Equipment oldItem );
	public OnEquipmentChanged onEquipmentChanged;

	public Equipment [] defaultItems;
	public SkinnedMeshRenderer targetMesh;

	Equipment [] currentEquipment;
	SkinnedMeshRenderer [] currentMeshes;
	Inventory inventory;

	private void Start()
	{
		inventory = Inventory.instance;

		int numberOfSlots = System.Enum.GetNames( typeof( EquipmentSlot ) ).Length;
		currentEquipment = new Equipment [numberOfSlots];
		currentMeshes = new SkinnedMeshRenderer [numberOfSlots];

		EquipDefaultItems();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.U))
		{
			UnEquipAll();
		}
	}

	public void Equip(Equipment newItem)
	{
		int slotIndex = (int)newItem.equipSlot;
		Equipment oldItem = Unequip( slotIndex );

		if (currentEquipment[slotIndex] != null)
		{
			oldItem = currentEquipment [slotIndex];
			inventory.Add( oldItem );
		}

		if(onEquipmentChanged != null)
		{
			onEquipmentChanged.Invoke( newItem, oldItem );
		}

		SetEquipmentBlendShapes( newItem, 100 );

		currentEquipment [slotIndex] = newItem;
		SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>( newItem.mesh );
		newMesh.transform.parent = targetMesh.transform;

		newMesh.bones = targetMesh.bones;
		newMesh.rootBone = targetMesh.rootBone;
		currentMeshes [slotIndex] = newMesh;
	}

	public Equipment Unequip(int slotIndex)
	{
		if(currentEquipment[slotIndex] != null)
		{
			if(currentMeshes[slotIndex] != null)
			{
				Destroy( currentMeshes [slotIndex].gameObject );
			}

			Equipment oldItem = currentEquipment [slotIndex];
			SetEquipmentBlendShapes( oldItem, 0 );

			if(inventory.Add( oldItem ))
			{
				currentEquipment [slotIndex] = null;
			}

			if ( onEquipmentChanged != null )
			{
				onEquipmentChanged.Invoke( null, oldItem );
			}

			return oldItem;
		}

		return null;
	}

	public void UnEquipAll()
	{
		for (int i = 0; i < currentEquipment.Length; i++ )
		{
			Unequip( i );
		}
		EquipDefaultItems();
	}

	private void SetEquipmentBlendShapes(Equipment item, int weight)
	{
		foreach(EquipmentMeshRegion blendShape in item.coverMeshRegions)
		{
			targetMesh.SetBlendShapeWeight( (int)blendShape, weight );
		}
	}

	private void EquipDefaultItems()
	{
		foreach(Equipment item in defaultItems)
		{
			Equip( item );
		}
	}
}
