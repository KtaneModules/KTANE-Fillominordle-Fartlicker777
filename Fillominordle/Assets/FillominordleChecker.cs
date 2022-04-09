using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class FillominordleChecker : MonoBehaviour {

   public static bool CheckIfGroupsAreCorrectSizes (int Index, int[] Grid) {

      List<int> Group = new List<int> { Index };
      List<int> Visited = new List<int> { Index };

      //Debug.Log("BFS");

      while (Visited.Count() != 0) {
         for (int i = 0; i < Visited.Count(); i++) {
            //Debug.Log(Visited[i] + " ");
         }
         List<int> AddToVisit = new List<int> { };
         if (Left(Visited.Last(), Grid[Index], Grid) && !Group.Contains(Visited.Last() - 1)) {
            AddToVisit.Add(Visited.Last() - 1);
         }
         if (Right(Visited.Last(), Grid[Index], Grid) && !Group.Contains(Visited.Last() + 1)) {
            AddToVisit.Add(Visited.Last() + 1);
         }
         if (Up(Visited.Last(), Grid[Index], Grid) && !Group.Contains(Visited.Last() - 5)) {
            AddToVisit.Add(Visited.Last() - 5);
         }
         if (Down(Visited.Last(), Grid[Index], Grid) && !Group.Contains(Visited.Last() + 5)) {
            AddToVisit.Add(Visited.Last() + 5);
         }
         //Debug.Log("Removing " + Visited.Last());
         Visited.RemoveAt(Visited.Count() - 1);
         foreach (int item in AddToVisit) {
            Visited.Add(item);
            //Debug.Log("Adding " + Visited.Last());
            Group.Add(item);
         }
         AddToVisit.Clear();
      }

      return Grid[Group[0]] == Group.Count();
   }

   #region Duplicate Checking

   static bool Left (int Index, int Check, int[] Grid) {
      if (Index % 5 != 0 && Grid[Index - 1] == Check) {
         return true;
      }
      return false;
   }

   static bool Right (int Index, int Check, int[] Grid) {
      if (Index % 5 != 4 && Grid[Index + 1] == Check) {
         return true;
      }
      return false;
   }

   static bool Up (int Index, int Check, int[] Grid) {
      if (Index / 5 != 0 && Grid[Index - 5] == Check) {
         return true;
      }
      return false;
   }

   static bool Down (int Index, int Check, int[] Grid) {
      if (Index / 5 != 4 && Grid[Index + 5] == Check) {
         return true;
      }
      return false;
   }

   #endregion
}
