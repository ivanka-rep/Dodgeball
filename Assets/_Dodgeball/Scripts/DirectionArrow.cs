using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
   private Vector3 m_offset;

   private void Start()
   {
      m_offset = transform.position;
   }

   private void Update()
   {
      transform.position = new Vector3(transform.position.x, m_offset.y, transform.position.z);
   }
}
