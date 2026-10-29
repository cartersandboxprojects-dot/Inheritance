using System;
using System.IO;
using Inheritance.State;
using UnityEngine;

namespace Inheritance.Dialogue
{
    // Lightweight interaction panel only. No physical scene, travel or Afternoon content.
    public sealed class DayOneMorningRunner : MonoBehaviour
    {
        public GameState State { get; private set; }
        public string Transcript { get; private set; }
        public string PersistenceStatus { get; private set; } = "";
        public string SavePath => Path.Combine(Application.persistentDataPath, "day1-morning-v3.json");
        private Vector2 scroll;
        private void Awake()
        {
            if (gameObject.scene.path != "Assets/Scenes/Prototype/Prototype_Sandbox.unity") { enabled = false; return; }
            ResetMorning();
        }
        public void ResetMorning()
        {
            State = DayOneMorningContent.CreateState(); PersistenceStatus = "";
            Transcript = "Brecken wakes in the Master Suite. Abstract context only; no room construction.";
        }
        private bool Apply(Consequence action, string text)
        {
            if (!action.TryApply(State)) return false;
            Transcript = text; return true;
        }
        public bool Coffee() => Apply(DayOneMorningContent.DeliverCoffee, "A household staff member brings coffee. (Free; provisional line.)");
        public bool Agenda() => Apply(DayOneMorningContent.ReadAgenda, DayOneMorningContent.AgendaText);
        public bool FirstChoice(bool stay) => Apply(stay ? DayOneMorningContent.Stay : DayOneMorningContent.Leave,
            stay ? "Coffee and newspaper: funeral coverage and a stock price drop. Brecken gains context for later Richard Hale / board preparation; no payoff implemented."
            : "[Provisional] Brief family conversation with Name[1]. A small rapport / side-story seed; optional, with no major story loss if missed.");
        public bool Breakfast(bool support) => Apply(support ? DayOneMorningContent.BreakfastSupport : DayOneMorningContent.BreakfastBusiness,
            "Required Family Breakfast with " + DayOneMorningContent.BreakfastParticipants + ". [Provisional topic] " +
            (support ? "Brecken offers Elena support; a small rapport change and shared knowledge are recorded."
            : "Brecken asks Charlie about business expectations. Charlie is his older cousin and mentor; CEO Richard Hale remains distinct. A small rapport change and shared knowledge are recorded."));
        public bool FinalChoice(bool social) => Apply(social ? DayOneMorningContent.SocialDuty : DayOneMorningContent.PersonalLife,
            (social ? "[Provisional call] Funeral follow-up with Adrian Kessler. No second contact invented."
            : "[Provisional call] Former girlfriend / pre-inheritance-life contact. Identity unresolved; this is not Alex Wynn.") +
            " Morning exhausted; automatic transition to Afternoon 3/3. Tranche complete. Office/board content comes next.");
        public bool SaveMorning()
        {
            try { StatePersistence.Save(SavePath, State, DayOneMorningContent.Validate, DayOneMorningContent.Catalog()); PersistenceStatus = "Morning saved."; return true; }
            catch (Exception e) { PersistenceStatus = "Save failed: " + e.Message; return false; }
        }
        public bool LoadMorning()
        {
            try
            {
                var loaded = StatePersistence.Load(SavePath, DayOneMorningContent.Validate, DayOneMorningContent.Catalog());
                State = loaded; Transcript = "Morning loaded. Continue from the recorded stage below."; PersistenceStatus = "Morning loaded."; return true;
            }
            catch (Exception e) { PersistenceStatus = "Load failed: " + e.Message; return false; }
        }
        private void OnGUI()
        {
            if (State == null) return;
            GUI.depth = -1000;
            var panel = new Rect(12, 12, Mathf.Min(620, Screen.width - 24), Screen.height - 24);
            var previousColor = GUI.color;
            GUI.color = new Color(0.12f, 0.12f, 0.14f, 1f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture); GUI.color = previousColor;
            GUILayout.BeginArea(panel, GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("DAY 1 / MORNING LOGIC - Sandbox scaffolding");
            GUILayout.Label(DayOneMorningContent.Notice);
            GUILayout.Label("Day " + State.Day + " | " + State.CurrentTimeBlock + " | Capacity " + State.TimeUnitsRemaining + "/3");
            GUILayout.Label("Stage: " + State.StoryPhase);
            if (State.StoryPhase == DayOneMorningContent.Wake)
            {
                GUILayout.Label("Master Suite / waking context (abstract)");
                Button("Household staff brings coffee (free)", DayOneMorningContent.DeliverCoffee, Coffee);
                Button("Marie reads agenda (free)", DayOneMorningContent.ReadAgenda, Agenda);
            }
            if (State.StoryPhase == DayOneMorningContent.First)
            {
                Button("Stay: coffee + newspaper (1 unit)", DayOneMorningContent.Stay, () => FirstChoice(true));
                Button("Leave room: family conversation (1 unit)", DayOneMorningContent.Leave, () => FirstChoice(false));
            }
            if (State.StoryPhase == DayOneMorningContent.BreakfastPhase)
            {
                GUILayout.Label("Required Family Breakfast: " + DayOneMorningContent.BreakfastParticipants);
                GUILayout.Label("Choose one provisional topic within the breakfast scene; total cost 1 unit.");
                Button("Breakfast: offer Elena support (1 unit)", DayOneMorningContent.BreakfastSupport, () => Breakfast(true));
                Button("Breakfast: ask Charlie about business (1 unit)", DayOneMorningContent.BreakfastBusiness, () => Breakfast(false));
            }
            if (State.StoryPhase == DayOneMorningContent.Final)
            {
                Button("Social Duty: funeral follow-up / Adrian (1 unit)", DayOneMorningContent.SocialDuty, () => FinalChoice(true));
                Button("Personal Life: former girlfriend [identity provisional] (1 unit)", DayOneMorningContent.PersonalLife, () => FinalChoice(false));
            }
            if (DayOneMorningContent.ReadyForOffice.IsMet(State)) GUILayout.Label("MORNING COMPLETE / Afternoon 3/3. Required office sequence pending next tranche.");
            GUILayout.Label("Newspaper context: " + State.Knows(DayOneMorningContent.Brecken, DayOneMorningContent.Preparation) +
                " | Optional family seed: " + State.GetFact(DayOneMorningContent.Seed) + " | Breakfast complete: " + State.GetFact(DayOneMorningContent.Breakfast));
            GUILayout.Label(Transcript);
            if (GUILayout.Button("Save Morning")) SaveMorning();
            if (GUILayout.Button("Load Morning")) LoadMorning();
            if (GUILayout.Button("Reset Morning / try another route")) ResetMorning();
            GUILayout.Label(PersistenceStatus);
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }
        private void Button(string label, Consequence action, Func<bool> handler)
        { GUI.enabled = action.Condition.IsMet(State); if (GUILayout.Button(label)) handler(); GUI.enabled = true; }
    }
}
