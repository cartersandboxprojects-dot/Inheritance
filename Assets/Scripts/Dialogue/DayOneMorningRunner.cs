using System;
using System.IO;
using Inheritance.State;
using UnityEngine;

namespace Inheritance.Dialogue
{
    // Lightweight interaction panel only. Abstract structured Day 1 interactions only.
    public sealed class DayOneMorningRunner : MonoBehaviour
    {
        public GameState State { get; private set; }
        public string Transcript { get; private set; }
        public string PersistenceStatus { get; private set; } = "";
        public string SavePath => Path.Combine(Application.persistentDataPath, "day1-evening-v3.json");
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
            stay ? "Coffee and newspaper: funeral coverage and a stock price drop. Brecken gains context for Richard Hale / board preparation."
            : "[Provisional] Brief family conversation with Name[1]. A small rapport / side-story seed; optional, with no major story loss if missed.");
        public bool Breakfast(bool support) => Apply(support ? DayOneMorningContent.BreakfastSupport : DayOneMorningContent.BreakfastBusiness,
            "Required Family Breakfast with " + DayOneMorningContent.BreakfastParticipants + ". [Provisional topic] " +
            (support ? "Brecken offers Elena support; a small rapport change and shared knowledge are recorded."
            : "Brecken asks Charlie about business expectations. Charlie is his older cousin and mentor; CEO Richard Hale remains distinct. A small rapport change and shared knowledge are recorded."));
        public bool FinalChoice(bool social) => Apply(social ? DayOneMorningContent.SocialDuty : DayOneMorningContent.PersonalLife,
            (social ? "[Provisional call] Funeral follow-up with Adrian Kessler. No second contact invented."
            : "[Provisional call] Former girlfriend / pre-inheritance-life contact. Identity unresolved; this is not Alex Wynn.") +
            " Morning exhausted; automatic transition to Afternoon 3/3. Required office sequence is now available.");
        public bool SaveMorning()
        {
            try { StatePersistence.Save(SavePath, State, DayOneEveningContent.Validate, DayOneEveningContent.Catalog()); PersistenceStatus = "Day 1 saved."; return true; }
            catch (Exception e) { PersistenceStatus = "Save failed: " + e.Message; return false; }
        }
        public bool LoadMorning()
        {
            try
            {
                var loaded = StatePersistence.Load(SavePath, DayOneEveningContent.Validate, DayOneEveningContent.Catalog());
                State = loaded; Transcript = "Day 1 loaded. Continue from the recorded stage below."; PersistenceStatus = "Day 1 loaded."; return true;
            }
            catch (Exception e) { PersistenceStatus = "Load failed: " + e.Message; return false; }
        }
        public bool OfficeChoice(int route)
        {
            if (!DayOneAfternoonContent.ChooseOffice(State, route)) return false;
            Transcript = route == 0 ? "[Provisional] Meet the secretary; light rapport and optional office-life seed. The encounter remains delayable on other routes."
                : route == 1 ? (State.GetFact(DayOneAfternoonContent.HalePrepared)
                    ? "CEO Richard Hale builds on Brecken's newspaper context: improved preparation for board questions. [Provisional dialogue]"
                    : "CEO Richard Hale provides basic succession and continuity context; no newspaper required. [Provisional dialogue]")
                : "[Provisional VP / target] A possible acquisition opens a continuing business opportunity. No acceptance, resolution or deadline on Day 1.";
            return true;
        }
        public bool BoardMeeting(bool measured)
        {
            if (!DayOneAfternoonContent.CompleteBoard(State, measured)) return false;
            Transcript = DayOneAfternoonContent.Context + " " + (State.GetFact(DayOneAfternoonContent.Concern)
                ? "Dismissal creates initial concern."
                : State.GetFact(DayOneAfternoonContent.Informed) ? "Preparation supports an informed, composed first impression."
                : "Listening and restraint create a cautious, constructive first impression even without prior context.")
                + " [Provisional reactions] Afternoon exhausted; automatic Evening 3/3. Evening opportunities now available.";
            return true;
        }
        public bool EnterEvening() => ApplyEvening(() => DayOneEveningContent.Enter(State), "Evening invitation [provisional]. Attendance is optional.");
        public bool ExternalEvening(bool take) => ApplyEvening(() => take ? DayOneEveningContent.TakeExternal(State) : DayOneEveningContent.DeferExternal(State),
            take ? "[Provisional] External encounter complete (2 units). Return to Mansion (abstract)." : "Invitation deferred, unresolved. Remain at Mansion; no attendance penalty authored.");
        public bool MansionEvening(int choice) => ApplyEvening(() => DayOneEveningContent.ChooseFinal(State, choice),
            choice == 0 ? "[Provisional] Elena seeks connection and reassurance after Brecken's return and their parents' deaths. Spend time together."
            : choice == 1 ? "[Provisional] Charlie offers helpful business advice and review as an experienced cousin and mentor. Spend time together."
            : "[Provisional identity] A light extended-family conversation; optional world texture.");
        public bool VictoriaClose()
        {
            string text = DayOneEveningContent.CloseText(State);
            return ApplyEvening(() => DayOneEveningContent.CloseDay(State), text);
        }
        private bool ApplyEvening(Func<bool> action, string text)
        { if (!action()) return false; Transcript = text; return true; }
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
            GUILayout.Label("DAY 1 / MORNING + AFTERNOON + EVENING - Sandbox scaffolding");
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
            if (DayOneMorningContent.ReadyForOffice.IsMet(State))
            {
                GUILayout.Label("Office arrival (abstract). Choose exactly one visit; the Board Meeting is required afterward.");
                if (GUILayout.Button("Go straight to office / meet secretary [provisional] (1 unit)")) OfficeChoice(0);
                if (GUILayout.Button("Talk to CEO Richard Hale (1 unit)")) OfficeChoice(1);
                if (GUILayout.Button("Speak to Acquisitions VP [provisional] (1 unit)")) OfficeChoice(2);
            }
            if (State.StoryPhase == DayOneAfternoonContent.BoardPending)
            {
                GUILayout.Label("Required Board Meeting: " + DayOneAfternoonContent.Participants);
                GUILayout.Label(DayOneAfternoonContent.Context);
                if (GUILayout.Button("Listen, acknowledge limits, ask informed questions; commit to continuity (2 units)")) BoardMeeting(true);
                if (GUILayout.Button("Dismiss concerns and demand trust in your authority (2 units)")) BoardMeeting(false);
            }
            if (State.StoryPhase == DayOneAfternoonContent.EveningPending)
                if (GUILayout.Button("Begin Evening (free)")) EnterEvening();
            if (State.StoryPhase == DayOneEveningContent.External)
            {
                GUILayout.Label(State.GetFact(DayOneMorningContent.Social) ? "Adrian / private-club drinks [provisional, abstract]" : "Former girlfriend / personal encounter [provisional, distinct from Alex Wynn]");
                if (GUILayout.Button("Take external commitment (2 units)")) ExternalEvening(true);
                if (GUILayout.Button("Defer invitation / remain at Mansion (free)")) ExternalEvening(false);
            }
            if (State.StoryPhase == DayOneEveningContent.Mansion)
            {
                GUILayout.Label("Mansion final conversation. One choice, 1 unit; unused Evening capacity winds down explicitly before Victoria.");
                if (GUILayout.Button("Elena: connection and reassurance (1 unit)")) MansionEvening(0);
                if (GUILayout.Button("Charlie: business advice / review (1 unit)")) MansionEvening(1);
                if (GUILayout.Button("Minor relative [provisional]: light conversation (1 unit)")) MansionEvening(2);
            }
            if (State.StoryPhase == DayOneEveningContent.Close)
                if (GUILayout.Button("Victoria: report final conversation / close Day 1 (free)")) VictoriaClose();
            if (State.GetFact(DayOneEveningContent.Complete)) GUILayout.Label("TUTORIAL DAY 1 COMPLETE. No post-Day-1 gameplay implemented.");
            GUILayout.Label("Newspaper context: " + State.Knows(DayOneMorningContent.Brecken, DayOneMorningContent.Preparation) +
                " | Optional family seed: " + State.GetFact(DayOneMorningContent.Seed) + " | Breakfast complete: " + State.GetFact(DayOneMorningContent.Breakfast));
            GUILayout.Label(Transcript);
            if (GUILayout.Button("Save Day 1")) SaveMorning();
            if (GUILayout.Button("Load Day 1")) LoadMorning();
            if (GUILayout.Button("Reset Morning / try another route")) ResetMorning();
            GUILayout.Label(PersistenceStatus);
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }
        private void Button(string label, Consequence action, Func<bool> handler)
        { GUI.enabled = action.Condition.IsMet(State); if (GUILayout.Button(label)) handler(); GUI.enabled = true; }
    }
}
