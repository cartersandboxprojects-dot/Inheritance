# Stable authored identifiers

Implemented October 29, 2026; Unity 6000.3.27f1. Schema remains v3.

## Inventory and ownership

| Area | Current authored vocabulary / use | Validation boundary |
| --- | --- | --- |
| Characters | `Elena`, `Charlie`, `Victoria` in FuneralNightContent; scheduling proof reuses Elena | Character catalog; relationships, knowledge, message sender |
| Facts/tags | `BRECKEN_COMFORTED_ELENA`, `BRECKEN_MET_CHARLIE_FUNERAL_NIGHT`; `proof.family.visited`, `proof.elena.reacted` | Fact catalog; predicate/effect references and saved facts/knowledge |
| Opportunities | `Charlie_FuneralNightBusiness`; `proof.family.visit_elena`, `proof.business.charlie_call` | Opportunity catalog; competition targets, predicates/effects, saved progress and message relation |
| Messages | `proof.message.elena_thanks` | Message catalog plus actual MessageDefinition; sender/relation metadata checked on load |
| Relationships | `(character, attribute)` keys; current attribute `Resentment` | Both Character and Attribute catalogs |
| Knowledge | `(character, fact)` pairs, independent of fact truth | Both catalogs; no automatic inference from truth |
| Story phases | `FuneralNight`, `AfterFuneralNight`, `SchedulingProof` | Phase catalog; predicates/effects and saved phase |
| Interactions | FuneralNightConsequences choices/reports/reactions and scheduling proof reaction | Catalog Binding entries inspect actual condition/effect reference metadata |
| Scene/Ink linkage | Runner uses serialized transforms, `npc.name`/identity, knots `elena`, `charlie`, `victoria`; six Ink externals `elena_comforted`, `comfort_elena`, `charlie_met`, `meet_charlie`, `victoria_family`, `victoria_business` | Existing Ink regression tests; these are adapter contracts, not saved state IDs |
| Save records | Phase; Fact.Id; Relationship.Character/Attribute; Knowledge.Character/Fact; Opportunity.Id; Message.Id/Sender/RelatedOpportunity | Detached catalog migration/validation before Deserialize/Load returns |
| Generic tests | Local short IDs and intentionally bad values in isolated state/consequence/persistence fixtures | Generic state/serializer remains registry-free; authored catalogs are tested strictly |

There is no separate persisted scene-ID, interaction-ID, arbitrary tag registry, or
character asset registry today. Binding IDs identify validation owners, not saved
progress. Brecken is named in fact identifiers, not currently a referenced character
record. Don't invent content records solely to make an inventory look complete.

Development/Repo is authoritative for implemented IDs and game-state content.
Personal Carter can propose story, character and scene concepts; proposals become
implemented IDs only through reviewed authored content/catalog changes here.

## Small stable-ID layer

Keep the string-based GameState API, DTOs, Ink source, and content constants.
ContentReference pairs a ContentKind with its human-readable spelling; catalogs
define uniqueness within a kind, not across unrelated namespaces. ContentEntry
factories for Opportunity, Message and Binding read references from the actual
immutable definitions. Condition.All carries metadata through nested predicates;
effects, competition targets, message delivery and onRead are included. Use these
factories when authoring definitions; don't duplicate their reference graph by hand.
Use a plain ContentEntry for vocabulary records such as facts or characters.

AuthoredContentCatalogs contains two independent content scopes, FuneralNight and
SchedulingProof. Each CreateState validates its definitions in Runtime mode before
creating state. New scopes should do the same. Built-in catalogs have strict tests.
Future data-authored content can supply the same entries at its import boundary;
there is no requirement to move prose, characters or story design into C#.
The repeated scheduling Elena literal now reuses FuneralNightContent.Elena. Ink
function/knot names stay at the adapter/content boundary; no new narrative C# store.

## Canonical spelling

New IDs: ASCII lowercase; begin with a letter; subsequent segments use letters or
digits, separated by a single dot or underscore; maximum 128 characters.
Exact regex: `\A[a-z][a-z0-9]*(?:[._][a-z0-9]+)*\z`.
Examples: `elena`, `chapter1.funeral.comfort_elena`, `proof.message.elena_thanks`.
Use dots for scope and underscores for multiword concepts. A category prefix is
optional when the catalog kind already makes it clear. No whitespace, hyphens,
slashes, repeated/trailing separators, Unicode, case folding, trimming or GUIDs.
Comparison is exact and case-sensitive. IDs describe durable concepts, not display
names, text, scene hierarchy paths or a character's current characterization.

Existing mixed/uppercase spellings listed above are explicitly grandfathered by
legacySpelling declarations; none was renamed in this tranche. Don't expand those
exceptions to new vocabulary. A spelling change is a saved-ID migration, not cleanup.

## Validation modes and contradictions

Validate(mode) returns explicit diagnostics; RequireValid(mode) throws
InvalidDataException with actionable diagnostics if any are errors.

| Mode | Missing canonical target on entry with nonblank provisional reason | Other structural errors |
| --- | --- | --- |
| Development | Warning naming owner, target, reason; work can continue | Error |
| Strict (tests/CI, default) | Error | Error |
| Runtime | Error before content bootstrap/save/load succeeds | Error |

Every provisional marker also emits a warning with its reason. A fully resolved
experimental entry can pass strict validation; provisional is not a blanket ban on
experimentation. Missing references on unmarked entries always fail. Malformed IDs
or references, duplicates, invalid migration rules, invalid predicate/effect enums,
and contradictory equality predicates cannot be waived with a provisional marker.
Sandbox authoring tools should call Validate(Development), display all diagnostics,
and explicitly isolate unresolved experiments from production state/persistence.
This tranche adds the engine-free API; it changes no Sandbox scene or editor UI.

Checks cover all declared IDs and actual predicate/effect references, competition,
message senders/relations and saved records. Constructors already reject null
effects/conditions, invalid costs/windows, self/duplicate competition targets and
unsupported competition results. Catalog validation additionally detects conflicting
equalities in nested All conditions (fact/knowledge truth, phase, time, opportunity
or message status) and invalid enum operands. It does not infer narrative intent,
mandate reciprocal competition, impose relationship ranges, prohibit deliberate
multiple time advances, or require every queued message eventually to deliver.
FuneralNightSaveValidation still owns the existing branch/history invariants.

## Saved-ID rename and removal policy

SavedIdRule is a small, category-scoped, one-hop rule with a mandatory reason.
A retired source must be unique and absent from live entries; its replacement must
be a live entry of the same kind. Chains, cycles, dangling targets and active-source
rules fail catalog validation. Aliases apply only to saved IDs, never to authored
references: fix current content to use the live spelling.

Representative pre-release compatibility examples (no current content was renamed):

- Character `Elena_FuneralPrototype` -> `Elena` in relationships, knowledge and sender fields.
- Opportunity `Charlie_FuneralBusiness` -> `Charlie_FuneralNightBusiness` in progress and message relations.

Removed content either receives an explicit same-kind replacement using the same
mechanism, or fails load with a category/ID diagnostic. No silent skipping, default
substitution, progress reset or merging. If alias plus canonical records collapse
onto the same key, reject the save, including conflicting truth/progress/knowledge.
Repair means a deliberate reviewed rule or explicit repair outside this tranche.

Only historical facts may be archived without a replacement. The explicit example
`archive.funeral.old_greeting` retains its true/false record and existing per-character
knowledge. It is not a live declaration and cannot satisfy authored references.
No knowledge is invented and no truth is inferred. Characters, opportunities,
messages, relationship attributes and phases cannot use archive mode: replace or
reject. Archived entries remain in the existing Facts/Knowledge arrays; no extra
schema fields or universal migration framework are introduced.

## Save/load and runtime safety

StatePersistence Serialize/Deserialize/Save/Load accept an optional ContentCatalog.
The actual InkPrototypeRunner always supplies the FuneralNight catalog and existing
authored invariant callback. The callback itself also rejects unknown IDs, protecting
existing callers that already supplied it. Migration requires the catalog argument.
Generic GameState.Restore and registry-free persistence remain deliberately useful
for low-level tests and detached action clones; they are not production ID validation.
New production save/load owners must supply their scoped catalog and story validator.

Load parses and validates v1/v2/v3 structure, clones a detached snapshot, resolves all
saved ID fields, checks message sender/relation against its definition, rejects
collisions, then runs the caller's story validation. Only a successful return can
replace session state. Failure retains live state/dialogue and reports through the
existing UI error handling. Save rejects unknown or still-retired live IDs before
writing a temporary file; catalog migration is load-only. Existing atomic slot writes
are unchanged. Capacity, message statuses, timing and delayed evaluation are unchanged.
V1 full capacity and v1/v2 empty-inbox migrations remain intact; writes remain v3.

## Verification and deferred work

Full unfiltered EditMode suite: 167/167 passed (121 baseline + 46 identifier cases),
zero failures/skips/inconclusive. Includes canonical/malformed IDs, duplicates,
missing targets, nested contradictions, development provisional behavior, strict
and runtime rejection, actual definition reference extraction, rename/collision
handling, unknown IDs in all six saved categories, archival/removal policy, file
save preservation, mismatched message metadata, both funeral choices/Victoria and
scheduled message/reaction continuation. Unity compilation: failed=false, errors=[],
compilationFailed=false. Current Console: 0 errors, 0 warnings. Evidence is ignored
Temp/StableIds-full-tests.json plus final compile/Console records.

No additional live gameplay walkthrough or player build. Deferred: external data
import adapters, editor validation UI, Ink/scene graph scanning, generated wrappers,
global automatic catalog discovery, arbitrary historical story inference, universal
migrations, repair UI and broad GameState access restrictions. New content can extend
catalogs without changing the state engine. No scenes, art, vendor content, packages
or project settings are part of this change.
