using System;
using System.IO;
using Inheritance.State;
using UnityEngine;


namespace Inheritance.Dialogue
{
    // Sandbox-only panel: the buttons and live verification call these same handlers.
    public sealed class SandboxDayRunner : MonoBehaviour
    {
        public GameState State { get; private set; }
        public string Transcript { get; private set; } = SandboxDayContent.Notice;
        public string PersistenceStatus { get; private set; } = "";
        public string SavePath => Path.Combine(Application.persistentDataPath, "provisional-sandbox-day-v3.json");
        private Vector2 scroll;
        private void Awake()
        {
            if (gameObject.scene.path != "Assets/Scenes/Prototype/Prototype_Sandbox.unity") { enabled = false; return; }
            ResetDay();
        }
        public void ResetDay() { State = SandboxDayContent.CreateState(); Transcript = SandboxDayContent.Notice; PersistenceStatus = ""; }
        private bool Apply(Consequence action, string text)
        {
            if (!action.TryApply(State)) return false;
            SandboxDayContent.Evaluate(State); Transcript = "[PROVISIONAL TEST] " + text; return true;
        }
        public bool TalkElena() => Apply(SandboxDayContent.Greet, "Elena: Could you help sort some sample boxes this morning? (Free conversation.)");
        public bool TalkCharlie() => Apply(SandboxDayContent.Brief, "Charlie: There is also a sample figures call this morning. Pick one commitment. (Free conversation.)");
        public bool Choose(bool family)
        {
            if (!(family ? SandboxDayContent.Family : SandboxDayContent.Business).TryComplete(State)) return false;
            SandboxDayContent.Evaluate(State);
            Transcript = family ? "[PROVISIONAL TEST] Helped Elena sort sample boxes. Cost 2; Charlie's call is missed. One morning unit remains."
                : "[PROVISIONAL TEST] Checked sample figures with Charlie. Cost 3; afternoon begins. Elena's task is deferred.";
            return true;
        }
        public bool PassTime()
        {
            return State.CurrentTimeBlock == TimeBlock.Morning
                ? Apply(SandboxDayContent.FinishMorning, "Morning finished; the queued phone message can arrive now.")
                : Apply(SandboxDayContent.FinishAfternoon, "Afternoon finished. Read your phone, then report to Victoria.");
        }
        public bool ReadPhone()
        {
            var message = State.GetFact(SandboxDayContent.FamilyFact) ? SandboxDayContent.FamilyMessage : SandboxDayContent.BusinessMessage;
            if (!message.TryRead(State)) return false;
            Transcript = message.Text; return true;
        }
        public bool TalkVictoria()
        {
            bool family = State.GetFact(SandboxDayContent.FamilyFact);
            return Apply(family ? SandboxDayContent.FamilyReaction : SandboxDayContent.BusinessReaction,
                family ? "Victoria: You helped Elena, read her update, and missed Charlie's window. I now know about the sample boxes."
                : "Victoria: You helped Charlie and read his update. Elena's sample task will wait. I now know about the sample figures.");
        }
        public bool FinishDay() => Apply(SandboxDayContent.EndDay, "Day complete at Night. This branch is a systems exercise, not accepted story. Restart to try the other path.");
        public bool SaveDay()
        {
            try { StatePersistence.Save(SavePath, State, catalog: SandboxDayContent.Catalog()); PersistenceStatus = "Saved provisional day."; return true; }
            catch (Exception error) { PersistenceStatus = "Save failed: " + error.Message; return false; }
        }
        public bool LoadDay()
        {
            try
            {
                var loaded = StatePersistence.Load(SavePath, catalog: SandboxDayContent.Catalog());
                SandboxDayContent.Evaluate(loaded); State = loaded;
                Transcript = "[PROVISIONAL TEST] Loaded. Continue using the state and phone controls."; PersistenceStatus = "Loaded provisional day."; return true;
            }
            catch (Exception error) { PersistenceStatus = "Load failed: " + error.Message; return false; }
        }
        private void OnGUI()
        {
            if (State == null) return;
            GUI.depth = -1000;
            var panel = new Rect(12, 12, Mathf.Min(570, Screen.width - 24), Screen.height - 24);
            var previousColor = GUI.color;
            GUI.color = new Color(0.12f, 0.12f, 0.14f, 1f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture); GUI.color = previousColor;
            GUILayout.BeginArea(panel, GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("PROVISIONAL SANDBOX DAY / NON-CANONICAL");
            GUILayout.Label("AI-generated test beats only. Personal Carter may rewrite all content.");
            GUILayout.Label("Day " + State.Day + " | " + State.CurrentTimeBlock + " | Capacity " + State.TimeUnitsRemaining + "/3");
            GUILayout.Label("1. Free conversations -> 2. One commitment -> 3. Phone -> 4. Evening report -> 5. Finish");
            GUILayout.Label("Elena task: " + State.GetOpportunityState(SandboxDayContent.FamilyId) + " | Charlie call: " + State.GetOpportunityState(SandboxDayContent.BusinessId));
            GUILayout.Label("Phone Elena: " + State.GetMessageStatus(SandboxDayContent.FamilyMessageId) + " | Charlie: " + State.GetMessageStatus(SandboxDayContent.BusinessMessageId));
            GUILayout.Label("Elena knows boxes: " + State.Knows(FuneralNightContent.Elena, SandboxDayContent.FamilyFact) + " | Charlie knows figures: " + State.Knows(FuneralNightContent.Charlie, SandboxDayContent.BusinessFact));
            GUILayout.Label("Victoria knows boxes/figures: " + State.Knows(FuneralNightContent.Victoria, SandboxDayContent.FamilyFact) + "/" + State.Knows(FuneralNightContent.Victoria, SandboxDayContent.BusinessFact));
            Button("Talk Elena (free)", !State.GetFact(SandboxDayContent.Greeting), TalkElena);
            Button("Talk Charlie (free)", !State.GetFact(SandboxDayContent.Briefing), TalkCharlie);
            Button("Help Elena: sample boxes (2 units)", SandboxDayContent.Family.CanStart(State), () => Choose(true));
            Button("Help Charlie: sample figures call (3 units)", SandboxDayContent.Business.CanStart(State), () => Choose(false));
            Button("Finish morning (remaining capacity)", State.CurrentTimeBlock == TimeBlock.Morning && State.GetFact(SandboxDayContent.FamilyFact), PassTime);
            Button("Read delivered phone message (free)", State.GetMessageStatus(SandboxDayContent.FamilyMessageId) == MessageStatus.Delivered || State.GetMessageStatus(SandboxDayContent.BusinessMessageId) == MessageStatus.Delivered, ReadPhone);
            Button("Finish afternoon (remaining capacity)", State.CurrentTimeBlock == TimeBlock.Afternoon, PassTime);
            Button("Report to Victoria (free)", State.CurrentTimeBlock == TimeBlock.Evening && !State.GetFact(SandboxDayContent.ReactionFact), TalkVictoria);
            GUILayout.Label("Victoria responds only after the chosen character knows the outcome and the phone message is read.");
            Button("Finish day (remaining evening)", State.GetFact(SandboxDayContent.ReactionFact) && !State.GetFact(SandboxDayContent.EndFact), FinishDay);
            GUILayout.Label(Transcript);
            if (GUILayout.Button("Save provisional day")) SaveDay();
            if (GUILayout.Button("Load provisional day")) LoadDay();
            GUILayout.Label(PersistenceStatus);
            if (GUILayout.Button("Restart fresh / try other branch")) ResetDay();
            GUILayout.EndScrollView(); GUILayout.EndArea();
        }
        private static void Button(string label, bool available, Func<bool> handler)
        { GUI.enabled = available; if (GUILayout.Button(label)) handler(); GUI.enabled = true; }
    }
}


