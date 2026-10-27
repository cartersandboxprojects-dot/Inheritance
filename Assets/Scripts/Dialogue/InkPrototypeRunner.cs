using Inheritance.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inheritance.Dialogue
{
    // Prototype session owner. All story mutations remain guarded backend consequences.
    public sealed class InkPrototypeRunner : MonoBehaviour
    {
        [SerializeField] private Ink.UnityIntegration.InkFile compiledStory;
        [SerializeField] private Transform player;
        [SerializeField] private Transform elena;
        [SerializeField] private Transform charlie;
        [SerializeField] private Transform victoria;
        [SerializeField] private float interactionRange = 2.5f;
        public GameState State { get; private set; }
        public ElenaInkConversation Conversation { get; private set; }
        public string Transcript { get; private set; }
        private string speaker;

        private void Start() => ResetPrototype();

        public void ResetPrototype()
        {
            State = FuneralNightContent.CreateState();
            CloseConversation();
        }

        private Transform Nearest()
        {
            if (player == null) return null;
            Transform nearest = null;
            float distance = interactionRange;
            foreach (var npc in new[] { elena, charlie, victoria })
            {
                if (npc == null) continue;
                float candidate = Vector3.Distance(player.position, npc.position);
                if (candidate <= distance) { nearest = npc; distance = candidate; }
            }
            return nearest;
        }

        private void Update()
        {
            if (Keyboard.current == null || !Application.isFocused) return;
            if (Keyboard.current.escapeKey.wasPressedThisFrame) CloseConversation();
            if (Conversation == null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                var npc = Nearest();
                if (npc != null) TalkTo(npc.name);
            }
        }

        public bool TalkTo(string npcName)
        {
            var npc = Nearest();
            if (npc == null || npc.name != npcName || Conversation != null) return false;
            if (npc == victoria)
            {
                // Authored information event: Brecken tells Victoria which meeting he attended.
                // Only the selected true event is granted; truth alone never implies knowledge.
                FuneralNightConsequences.InformVictoria.TryApply(State);
                FuneralNightConsequences.InformVictoriaAboutCharlie.TryApply(State);
            }
            speaker = npcName;
            string knot = npc == elena ? "elena" : npc == charlie ? "charlie" : "victoria";
            Conversation = new ElenaInkConversation(compiledStory.storyJson, State, knot);
            Transcript = Conversation.ContinueToChoice();
            return true;
        }

        public void SelectChoice(int index) => Transcript += "\n" + Conversation.Choose(index);

        public void CloseConversation()
        {
            Conversation = null;
            Transcript = "";
            speaker = null;
        }

        private void OnGUI()
        {
            if (State == null) return;
            GUILayout.BeginArea(new Rect(12, 12, 460, 290), GUI.skin.box);
            GUILayout.Label("Funeral night - WASD/arrows to move, E to talk, Esc to close");
            if (Conversation == null)
            {
                var npc = Nearest();
                if (npc != null && GUILayout.Button("Talk to " + npc.name + " [E]")) TalkTo(npc.name);
                else if (npc == null) GUILayout.Label("Approach Elena, Charlie, or Victoria.");
            }
            else
            {
                GUILayout.Label(speaker == FuneralNightContent.Victoria &&
                    State.StoryPhase == FuneralNightContent.AfterFuneralNight
                    ? "Brecken tells Victoria who he spent the evening with." : speaker);
                GUILayout.Label(Transcript);
                for (int i = 0; i < Conversation.Choices.Count; i++)
                    if (GUILayout.Button(Conversation.Choices[i].text)) { SelectChoice(i); break; }
                if (GUILayout.Button("Close")) CloseConversation();
            }
            GUILayout.Label("Family: " + State.GetFact(FuneralNightContent.BreckenComfortedElena) +
                " | Business: " + State.GetFact(FuneralNightContent.BreckenMetCharlie));
            GUILayout.Label(State.StoryPhase + " | " + State.CurrentTimeBlock);
            if (GUILayout.Button("Restart prototype (try the other choice)")) ResetPrototype();
            GUILayout.EndArea();
        }
    }
}
