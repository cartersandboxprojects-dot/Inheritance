using System;
using System.Collections.Generic;
using Ink.Runtime;
using Inheritance.State;

namespace Inheritance.Dialogue
{
    // The caller owns the session state. Ink owns only conversation flow.
    public sealed class ElenaInkConversation
    {
        private readonly Story story;
        public IReadOnlyList<Choice> Choices => story.currentChoices;
        public bool IsComplete => !story.canContinue && story.currentChoices.Count == 0;

        public ElenaInkConversation(string compiledJson, GameState state, string knot = null)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            story = new Story(compiledJson);
            story.BindExternalFunction("elena_comforted",
                () => state.GetFact(FuneralNightContent.BreckenComfortedElena), true);
            story.BindExternalFunction("charlie_met",
                () => state.GetFact(FuneralNightContent.BreckenMetCharlie), true);
            story.BindExternalFunction("meet_charlie", () =>
                FuneralNightConsequences.ChooseCharlie.TryApply(state), false);
            story.BindExternalFunction("victoria_family",
                () => FuneralNightConsequences.VictoriaBranch.IsMet(state), true);
            story.BindExternalFunction("victoria_business",
                () => FuneralNightConsequences.VictoriaBusinessBranch.IsMet(state), true);
            // Side effects must never execute during Ink lookahead.
            story.BindExternalFunction("comfort_elena", () =>
                FuneralNightConsequences.ChooseElena.TryApply(state), false);
            if (knot != null) story.ChoosePathString(knot);
        }

        public string ContinueToChoice()
        {
            var lines = new List<string>();
            while (story.canContinue) lines.Add(story.Continue().Trim());
            return string.Join("\n", lines);
        }

        public string Choose(int index)
        {
            story.ChooseChoiceIndex(index);
            return ContinueToChoice();
        }
    }
}
