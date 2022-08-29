using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceLoader : MonoBehaviour
{
   private void Awake(){
      Object[] objects = Resources.LoadAll("Assets/Prefabs/Networked");
      foreach (Object o in objects){
         NetworkClient.RegisterPrefab((GameObject)o);
      }
   }
}
