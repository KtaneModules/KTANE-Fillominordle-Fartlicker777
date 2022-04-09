using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class FillominoGenerator {

   int[] Grid = new int[25];

   public void GenerateFillimino () {
      MegaFuckUp:
      /*   
      Debug.Log(Grid[0].ToString() + " " + Grid[1].ToString() + " " + Grid[2].ToString() + " " + Grid[3].ToString() + " " + Grid[4].ToString());
      Debug.Log(Grid[5].ToString() + " " + Grid[6].ToString() + " " + Grid[7].ToString() + " " + Grid[8].ToString() + " " + Grid[9].ToString());
      Debug.Log(Grid[10].ToString() + " " + Grid[11].ToString() + " " + Grid[12].ToString() + " " + Grid[13].ToString() + " " + Grid[14].ToString());
      Debug.Log(Grid[15].ToString() + " " + Grid[16].ToString() + " " + Grid[17].ToString() + " " + Grid[18].ToString() + " " + Grid[19].ToString());
      Debug.Log(Grid[20].ToString() + " " + Grid[21].ToString() + " " + Grid[22].ToString() + " " + Grid[23].ToString() + " " + Grid[24].ToString());*/
      for (int i = 0; i < 25; i++) {
         if (Grid[i] > 9) {
            Grid[i] = 0;
         }
      }
      if (!Grid.Contains(0)) {   //We stop once the entire grid is filled
         bool Flag = false;
         for (int i = 0; i < 25; i++) {
            if (!CheckIfGroupsAreCorrectSizes(i)) {
               Flag = true;
            }
         }
         if (!Flag) {
            return;
         }
      }

      //Debug.Log("---------------------");

      //Finds all zeros, as they are blank spots
      List<int> Zeros = new List<int> { };

      for (int i = 0; i < 25; i++) {
         if (Grid[i] == 0) {
            Zeros.Add(i);
         }
      }
      //Chooses a random one
      Zeros = Zeros.Shuffle();

      //Sometimes it fucks up merging or somethind idk, just reroll it lol.
      if (Zeros.Count() == 0) {
         Debug.Log("WHOOOOOOOOOOOOOOOOOOOOOOOOOOO");
         for (int i = 0; i < 25; i++) {
            Grid[i] = 0;
         }
         goto MegaFuckUp;
      }
      int Rand = Zeros[Rnd.Range(0, Zeros.Count())];
      //Debug.Log(Rand);
      List<int> Dir = new List<int> { };

      /* For the zero
       * if that zero has no non-zero neighbors,
       * it becomes a one and it starts again.
       * 
       * if it does have a non-zero neighbor below 9,
       * it merges with it.
       */
      if (Right(Rand) && Grid[Rand + 1] < 9) {
         Dir.Add(1);
      }
      if (Left(Rand) && Grid[Rand - 1] < 9) {
         Dir.Add(-1);
      }
      if (Up(Rand) && Grid[Rand - 5] < 9) {
         Dir.Add(-5);
      }
      if (Down(Rand) && Grid[Rand + 5] < 9) {
         Dir.Add(5);
      }

      Dir = Dir.Shuffle();

      if (Dir.Count() == 0) {
         Grid[Rand] = 1;
      }
      else if (Grid[Rand + Dir[0]] == 1) {
         Grid[Rand] = 2;
         Grid[Rand + Dir[0]] = 2;
      }
      else {
         //Debug.Log(Rand);
         //Debug.Log(Rand + Dir[0]);

         /*
          * This gets all matching squares in a group,
          * newI is the square that we are adding
          * the new square to
          */
         int newI = Rand + Dir[0];
         
         List<int> Group1 = new List<int> { newI, Rand };
         List<int> Visited1 = new List<int> { newI };

         //Debug.Log("BeegGrid");

         while (Visited1.Count() != 0) {
            for (int i = 0; i < Visited1.Count(); i++) {
               //Debug.Log(Visited1[i] + " ");
            }
            List<int> AddToVisit = new List<int> { };

            /* Checks each adjacent square to see if its the same number.
             * If it is the same number, we mark it in group and then
             * check extensions off that
             */
            if (Left(Visited1.Last(), Grid[newI]) && !Group1.Contains(Visited1.Last() - 1)) {
               AddToVisit.Add(Visited1.Last() - 1);
            }
            if (Right(Visited1.Last(), Grid[newI]) && !Group1.Contains(Visited1.Last() + 1)) {
               AddToVisit.Add(Visited1.Last() + 1);
            }
            if (Up(Visited1.Last(), Grid[newI]) && !Group1.Contains(Visited1.Last() - 5)) {
               AddToVisit.Add(Visited1.Last() - 5);
            }
            if (Down(Visited1.Last(), Grid[newI]) && !Group1.Contains(Visited1.Last() + 5)) {
               AddToVisit.Add(Visited1.Last() + 5);
            }
            //Removes the square we check.
            //Debug.Log("Removing " + Visited1.Last());
            Visited1.RemoveAt(Visited1.Count() - 1);

            //Adds all matching squares to group, and to check neighbors
            foreach (int item in AddToVisit) {
               //Debug.Log("Adding" + item);
               Visited1.Add(item);
               //Debug.Log("Adding " + Visited1.Last());
               Group1.Add(item);
            }

            AddToVisit.Clear();
         }


         //Makes every spot in the grid equivalent to the amount in the group.
         for (int i = 0; i < Group1.Count(); i++) {
            Grid[Group1[i]] = Group1.Count();
         }
      }

      //List<List<int>> Groups = new List<List<int>> { };

      /* Now we try to merge touching
       * groups of the same number
       * by using the method before where
       * we check neighbors
       */

      List<int> Group = new List<int> { Rand };
      List<int> Visited = new List<int> { Rand };

      //Debug.Log("BFS");

      while (Visited.Count() != 0) {
         for (int i = 0; i < Visited.Count(); i++) {
            //Debug.Log(Visited[i] + " ");
         }
         List<int> AddToVisit = new List<int> { };
         if (Left(Visited.Last(), Grid[Rand]) && !Group.Contains(Visited.Last() - 1)) {
            AddToVisit.Add(Visited.Last() - 1);
         }
         if (Right(Visited.Last(), Grid[Rand]) && !Group.Contains(Visited.Last() + 1)) {
            AddToVisit.Add(Visited.Last() + 1);
         }
         if (Up(Visited.Last(), Grid[Rand]) && !Group.Contains(Visited.Last() - 5)) {
            AddToVisit.Add(Visited.Last() - 5);
         }
         if (Down(Visited.Last(), Grid[Rand]) && !Group.Contains(Visited.Last() + 5)) {
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

      if (Group.Count() != Grid[Rand]) {
         foreach (int item in Group) {
            Grid[item] = Group.Count();
         }
      }
      GenerateFillimino();
   }

   public bool CheckIfGroupsAreCorrectSizes (int Index) {

      List<int> Group = new List<int> { Index };
      List<int> Visited = new List<int> { Index };

      //Debug.Log("BFS");

      while (Visited.Count() != 0) {
         for (int i = 0; i < Visited.Count(); i++) {
            //Debug.Log(Visited[i] + " ");
         }
         List<int> AddToVisit = new List<int> { };
         if (Left(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() - 1)) {
            AddToVisit.Add(Visited.Last() - 1);
         }
         if (Right(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() + 1)) {
            AddToVisit.Add(Visited.Last() + 1);
         }
         if (Up(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() - 5)) {
            AddToVisit.Add(Visited.Last() - 5);
         }
         if (Down(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() + 5)) {
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

   public bool CheckIfGroupsAreCorrectSizes (int Index, int[] Grid) {

      List<int> Group = new List<int> { Index };
      List<int> Visited = new List<int> { Index };

      //Debug.Log("BFS");

      while (Visited.Count() != 0) {
         for (int i = 0; i < Visited.Count(); i++) {
            //Debug.Log(Visited[i] + " ");
         }
         List<int> AddToVisit = new List<int> { };
         if (Left(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() - 1)) {
            AddToVisit.Add(Visited.Last() - 1);
         }
         if (Right(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() + 1)) {
            AddToVisit.Add(Visited.Last() + 1);
         }
         if (Up(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() - 5)) {
            AddToVisit.Add(Visited.Last() - 5);
         }
         if (Down(Visited.Last(), Grid[Index]) && !Group.Contains(Visited.Last() + 5)) {
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


   bool Left (int Index) {
      if (Index % 5 != 0 && Grid[Index - 1] != 0) {
         return true;
      }
      return false;
   }

   bool Right (int Index) {
      if (Index % 5 != 4 && Grid[Index + 1] != 0) {
         return true;
      }
      return false;
   }

   bool Up (int Index) {
      if (Index / 5 != 0 && Grid[Index - 5] != 0) {
         return true;
      }
      return false;
   }

   bool Down (int Index) {
      if (Index / 5 != 4 && Grid[Index + 5] != 0) {
         return true;
      }
      return false;
   }

   #region Duplicate Checking

   bool Left (int Index, int Check) {
      if (Index % 5 != 0 && Grid[Index - 1] == Check) {
         return true;
      }
      return false;
   }

   bool Right (int Index, int Check) {
      if (Index % 5 != 4 && Grid[Index + 1] == Check) {
         return true;
      }
      return false;
   }

   bool Up (int Index, int Check) {
      if (Index / 5 != 0 && Grid[Index - 5] == Check) {
         return true;
      }
      return false;
   }

   bool Down (int Index, int Check) {
      if (Index / 5 != 4 && Grid[Index + 5] == Check) {
         return true;
      }
      return false;
   }

   #endregion

   public int[] genPuz () {
      GenerateFillimino();
      return Grid;
   }
}
