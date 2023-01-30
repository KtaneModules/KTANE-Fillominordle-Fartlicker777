using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Fillominordle : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public KMSelectable Mod;

   public KMSelectable[] GridButtons;
   public KMSelectable[] Arrows;
   public KMSelectable[] NumberButtons;
   public KMSelectable SubmissiveAndBreedable;
   public KMColorblindMode CB;
   bool cbMode;

   public GameObject Screen;

   public TextMesh[] FrontText;
   public TextMesh[] BackText;
   public TextMesh Stage;

   public Material[] Colors;

   bool Focused;

   int[] Grid = new int[25];
   int[] UserInput = new int[25];
   int[][] CheckGroups = new int[][] {
      new int[] { 0},
      new int[] { 1, 5},
      new int[] { 2, 6, 10},
      new int[] { 3, 7, 11, 15},
      new int[] { 4, 8, 12, 16, 20},
      new int[] {    9, 13, 17, 21},
      new int[] {       14, 18, 22},
      new int[] {           19, 23},
      new int[] {               24},
   };

   int Selected;

   int[][] NumStates = new int[][] {
      new int[25],
      new int[25],
      new int[25],
      new int[25],
      new int[25],
      new int[25],
      new int[25]
   };
   int[] GridButBroken = new int[25];
   int[][] ColStates = new int[][] {
      new int[25],
      new int[25],
      new int[25],
      new int[25],
      new int[25],
      new int[25],
      new int[25]
   };
   bool[] CanModifyState = new bool[7];
   int StageN = 1;

   int StageTemp;

   bool Animating;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   KeyCode[] NumKeys = {
      KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9,
      KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
   };

   void Awake () {
      ModuleId = ModuleIdCounter++;

      foreach (KMSelectable Button in GridButtons) {
         Button.OnInteract += delegate () { GridPress(Button); return false; };
      }

      foreach (KMSelectable Button in NumberButtons) {
         Button.OnInteract += delegate () { NumPress(Button); return false; };
      }

      foreach (KMSelectable Arrow in Arrows) {
         Arrow.OnInteract += delegate () { ArrowPress(Arrow); return false; };
      }

      SubmissiveAndBreedable.OnInteract += delegate () { SubmitPress(); return false; };

      Mod.OnFocus += delegate () { Focused = true; };
      Mod.OnDefocus += delegate () { Focused = false; };

      if (Application.isEditor) {
         Focused = true;
      }

   }

   void GridPress (KMSelectable B) {   //Just changes where the selection mark is 
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, B.transform);
      if (!CanModifyState[StageN - 1] || Animating) {
         return;
      }
      for (int i = 0; i < 25; i++) {
         if (B == GridButtons[i]) {
            GridButtons[Selected].GetComponent<MeshRenderer>().material = Colors[1];
            Selected = i;
            GridButtons[Selected].GetComponent<MeshRenderer>().material = Colors[0];
         }
      }
   }

   void NumPress (KMSelectable B) { //Changes the numbers in the grid
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, B.transform);
      if (!CanModifyState[StageN - 1] || Animating) {
         return;
      }
      for (int i = 0; i < 9; i++) { //StageN is 1 indexed
         if (B == NumberButtons[i]) {
            UserInput[Selected] = i + 1;
            NumStates[StageN - 1][Selected] = UserInput[Selected];
         }
      }
      FrontText[Selected].text = UserInput[Selected].ToString();
      BackText[Selected].text = UserInput[Selected].ToString();
      GridButtons[Selected].GetComponent<MeshRenderer>().material = Colors[1];
      Selected = (Selected + 1) % 25;
      GridButtons[Selected].GetComponent<MeshRenderer>().material = Colors[0];
   }

   void ArrowPress (KMSelectable A) {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, A.transform);
      if (Animating) {
         return;
      }
      if (A == Arrows[0]) {
         if (StageN == 1 && CanModifyState[6]) {
            return;
         }
         StageN = StageN - 1 == 0 ? 7 : StageN - 1;
      }
      else {
         if (StageN != 7 && CanModifyState[StageN - 1]) {
            return;
         }
         StageN = StageN + 1 == 8 ? 1 : StageN + 1;
      }
      UpdateStage();
   }

   void SubmitPress () {
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SubmissiveAndBreedable.transform);
      if (Animating || !CanModifyState[StageN - 1] || (StageN != 1 && CanModifyState[StageN - 2])) {
         return;
      }
      for (int i = 0; i < 25; i++) {
         if (!FillominordleChecker.CheckIfGroupsAreCorrectSizes(i, UserInput) || NumStates[StageN - 1][i] == 0) { //Checks for empty square and if it's a correct Fillomino
            return;
         }
      }
      StageTemp = StageN;
      Animating = true;
      //Debug.Log(Grid[0].ToString() + Grid[1].ToString() + Grid[2].ToString() + Grid[3].ToString() + Grid[4].ToString() + Grid[5].ToString() + Grid[6].ToString() + Grid[7].ToString() + Grid[8].ToString() + Grid[9].ToString() + Grid[10].ToString() + Grid[11].ToString() + Grid[12].ToString() + Grid[13].ToString() + Grid[14].ToString() + Grid[15].ToString() + Grid[16].ToString() + Grid[17].ToString() + Grid[18].ToString() + Grid[19].ToString() + Grid[20].ToString() + Grid[21].ToString() + Grid[22].ToString() + Grid[23].ToString() + Grid[24].ToString());
      //Debug.Log(UserInput[0].ToString() + UserInput[1].ToString() + UserInput[2].ToString() + UserInput[3].ToString() + UserInput[4].ToString() + UserInput[5].ToString() + UserInput[6].ToString() + UserInput[7].ToString() + UserInput[8].ToString() + UserInput[9].ToString() + UserInput[10].ToString() + UserInput[11].ToString() + UserInput[12].ToString() + UserInput[13].ToString() + UserInput[14].ToString() + UserInput[15].ToString() + UserInput[16].ToString() + UserInput[17].ToString() + UserInput[18].ToString() + UserInput[19].ToString() + UserInput[20].ToString() + UserInput[21].ToString() + UserInput[22].ToString() + UserInput[23].ToString() + UserInput[24].ToString());

      CalculateColors();

      StartCoroutine(Check());
   }

   void UpdateStage () {   //Visually update mod
      if (Animating) {
         return;
      }
      Stage.text = StageN.ToString() + "\nof\n7";
      for (int i = 0; i < 25; i++) {
         //This part just replaces the text from preexisting stages                                   | This is so it doesn't mark the symbols while placing them   | The Actual Colorblind mode bullshit
         FrontText[i].text = NumStates[StageN - 1][i] == 0 ? "" : NumStates[StageN - 1][i].ToString() + (!CanModifyState[StageN - 1]                                ? (cbMode ? new string[] { "@", "x", "*", "!" }[ColStates[StageN - 1][i] + 1] : "") : "");
         BackText[i].text = NumStates[StageN - 1][i] == 0 ? "" : NumStates[StageN - 1][i].ToString()  + (!CanModifyState[StageN - 1]                                ? (cbMode ? new string[] { "@", "x", "*", "!" }[ColStates[StageN - 1][i] + 1] : "") : "");
         GridButtons[i].GetComponent<MeshRenderer>().material = Colors[ColStates[StageN - 1][i] + 1];
      }
      if (CanModifyState[StageN - 1]) {
         GridButtons[Selected].GetComponent<MeshRenderer>().material = Colors[0];
      }
   }

   IEnumerator Check () {
      for (int i = 0; i < CheckGroups.Length; i++) {
         for (int j = 0; j < CheckGroups[i].Length; j++) {
            //Debug.Log(StageTemp);
            StartCoroutine(RotateAround(GridButtons[CheckGroups[i][j]], ColStates[StageTemp - 1][CheckGroups[i][j]]));  //Animation and check
         }
         yield return new WaitForSeconds(.3f);
      }
      CanModifyState[StageN - 1] = false;
      yield return new WaitForSeconds(2f);
      bool WillSolve = true;
      for (int i = 0; i < 25; i++) {   //Checks if any square is not green. If this is the case, we add a stage
         if (ColStates[StageN - 1][i] != 2) {
            Animating = false;
            ArrowPress(Arrows[1]);
            UpdateStage();
            WillSolve = false;
            break;
         }
      }
      if (WillSolve) {
         GetComponent<KMBombModule>().HandlePass();
         Screen.GetComponent<MeshRenderer>().material = Colors[3];
         Audio.PlaySoundAtTransform("Correct", transform);
      }
      else if (StageN == 1) {
         //Debug.Log("BOZO 8");
         Animating = false;
         Reset();
      }
   }

   void Update () {  //Keyboard
      if (Animating) {
         return;
      }
      if (Focused) {
         if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Arrows[0].OnInteract();
            UpdateStage();
         }
         if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Arrows[1].OnInteract();
            UpdateStage();
         }
         if (CanModifyState[StageN - 1]) {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
               SubmitPress();
               return;
            }
            for (int i = 0; i < 18; i++) {
               if (Input.GetKeyDown(NumKeys[i])) {
                  NumberButtons[i % 9].OnInteract();
               }
            }
            if (Input.GetKeyDown(KeyCode.W)) {
               Selected = Selected - 5 < 0 ? Selected + 20 : Selected - 5;
            }
            if (Input.GetKeyDown(KeyCode.A)) {
               if (Selected % 5 == 0) {
                  Selected += 4;
               }
               else {
                  Selected -= 1;
               }
            }
            if (Input.GetKeyDown(KeyCode.S)) {
               Selected = (Selected + 5) % 25;
            }
            if (Input.GetKeyDown(KeyCode.D)) {
               if (Selected % 5 == 4) {
                  Selected -= 4;
               }
               else {
                  Selected += 1;
               }
            }
            UpdateStage();
         }
      }
   }

   IEnumerator RotateAround (KMSelectable B, int j) {
      for (int i = 0; i < 90; i++) {
         B.transform.Rotate(2.0f, 0.0f, 0.0f, Space.Self);
         yield return null;
         //Debug.Log(B.GetComponent<Transform>().transform.rotation.x);
         if (B.GetComponent<Transform>().transform.rotation.x >= .5f || B.GetComponent<Transform>().transform.rotation.x <= -.5f) { //Quaternions moment
            B.GetComponent<MeshRenderer>().material = Colors[1 + j];
         }
      }
      if (cbMode) {
         FrontText[Array.IndexOf(GridButtons, B)].text = FrontText[Array.IndexOf(GridButtons, B)].text + new string[] { "@", "x", "*", "!" }[1 + j];
         BackText[Array.IndexOf(GridButtons, B)].text = BackText[Array.IndexOf(GridButtons, B)].text + new string[] { "@", "x", "*", "!" }[1 + j];
      }
      B.transform.Rotate(180f, 0.0f, 0.0f, Space.Self);  //Returns back to original state so you can select the button. Basically this whole function does a 360 flip, but shows only 180
   }

   void CalculateColors () { //0 is black, 1 is yellow, 2 is green
      for (int e = 0; e < 25; e++) { //First, set up "Broken Grid"
         GridButBroken[e] = Grid[e];
      }

      for (int c = 0; c < 25; c++) { //Next, find all greens
         if (Grid[c] == UserInput[c]) {
            //Debug.Log(StageN);
            ColStates[StageN - 1][c] = 2;
            GridButBroken[c] = 0;
         }
      }

      for (int o = 0; o < 25; o++) { //Finally, find all yellows
         if (ColStates[StageN - 1][o] == 2) { //Obviously a green is not a yellow
            continue;
         }
         else {
            if (GridButBroken.Contains(UserInput[o])) { //A yellow is only allowed if there is a cell for it to 'match up' with in the Broken Grid
               ColStates[StageN - 1][o] = 1;
               Remove(UserInput[o]);
            }
         }
      }

      /*for (int i = 0; i < 25; i++) {
         Debug.Log(ColStates[StageN - 1][i]);
      }*/
   }

   void Remove (int v) { //Remove function
      for (int k = 0; k < 25; k++) {
         if (GridButBroken[k] == v) {
            GridButBroken[k] = 0;
            break;
         }
      }
   }

   void Start () {
      for (int i = 0; i < 7; i++) {
         CanModifyState[i] = true;
      }
      FillominoGenerator L = new FillominoGenerator();
      Grid = L.genPuz();
      Debug.LogFormat("[Fillominordle #{0}] {1}", ModuleId, L.LogAttempts());
      Debug.LogFormat("[Fillominordle #{0}] The grid is\n[Fillominordle #{0}] {1}{2}{3}{4}{5}\n[Fillominordle #{0}] {6}{7}{8}{9}{10}\n[Fillominordle #{0}] {11}{12}{13}{14}{15}\n[Fillominordle #{0}] {16}{17}{18}{19}{20}\n[Fillominordle #{0}] {21}{22}{23}{24}{25}", ModuleId, Grid[0], Grid[1], Grid[2], Grid[3], Grid[4], Grid[5], Grid[6], Grid[7], Grid[8], Grid[9], Grid[10], Grid[11], Grid[12], Grid[13], Grid[14], Grid[15], Grid[16], Grid[17], Grid[18], Grid[19], Grid[20], Grid[21], Grid[22], Grid[23], Grid[24]);
      Stage.text = "1\nof\n7";
      for (int i = 0; i < 25; i++) {
         FrontText[i].text = "";
      }
      cbMode = CB.ColorblindModeActive;
   }

   void Reset () {   //Self explanatory
      StageN = 1;
      Selected = 0;
      Animating = false;
      for (int i = 0; i < 7; i++) {
         for (int j = 0; j < 25; j++) {
            ColStates[i][j] = 0;
            NumStates[i][j] = 0;
         }
      }
      Start();
      UpdateStage();
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} 12873192... to input the 25 digit number all at once. Use cycle to see all results so far.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper().Split(' ').Join("");
      yield return null;
      if (Command == "CYCLE") {
         for (int i = 0; i < 7; i++) {
            Arrows[0].OnInteract();
            yield return new WaitForSeconds(.1f);
         }
         for (int i = 0; i < StageN; i++) {
            Arrows[1].OnInteract();
            yield return new WaitForSeconds(1f);
         }
      }
      else if (Command.Length != 25 || !Command.Any(x => "123456789".Contains(x))) {
         yield return "sendtochaterror I don't understand";
      }
      else {
         for (int i = 0; i < 25; i++) {
            NumberButtons[int.Parse(Command[i].ToString()) - 1].OnInteract();
            yield return new WaitForSeconds(.1f);
         }
         SubmitPress();
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      if (Animating) {
         yield return true;
      }
      int Start = Selected;
      if (UserInput[Selected] != Grid[Selected]) {
         NumberButtons[Grid[Selected] - 1].OnInteract();
         yield return new WaitForSeconds(.1f);
      }
      while (Start != Selected) {
         NumberButtons[Grid[Selected] - 1].OnInteract();
         yield return new WaitForSeconds(.1f);
      }
      SubmissiveAndBreedable.OnInteract();
      yield return true;
   }
}
