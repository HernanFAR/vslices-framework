# Experiment: Initial Testing Battery against access-management-product

Status: exploratory, non-canonical

Date: 2026-09-02

## Purpose

This note proposes an initial experimental battery for testing the current VSlices testing hypotheses against a real product:

```text
atom-dev-serviu/access-management-product
```

The goal is not to maximize conventional coverage.

The goal is to obtain evidence about the testing approach itself:

- whether Regressive Testing can preserve knowledge already represented by VSIR and documentation;
- whether Progressive Testing can discover boundaries, contradictions, missing assumptions, or model gaps;
- whether the same abstract Verification and Challenge intentions can be applied meaningfully at different semantic scales;
- whether claim-relative semantic fidelity is a useful rule for selecting the execution surface;
- whether generated candidates can be reviewed, reduced, rejected, or promoted without making generated tests authoritative by default;
- whether functional and E2E evidence can challenge not only implementation, but also the documentation and VSIR from which implementation was derived.

The current `access-management-product` is a useful experimental subject because it already contains:

- Domain Type VSIR;
- reusable invariant VSIR;
- Feature VSIR;
- real SQL-backed Infrastructure;
- hosted integration-test surfaces using `WebApplicationFactory`;
- a browser-driven Ticket Support E2E using Playwright and the distributed AppHost;
- explicit documentation of what the final Ticket Support E2E does and does not claim to prove.

This allows the testing model to be exercised vertically rather than discussed only in the abstract.

---

# 1. Experimental principle

The first battery should not attempt to generate every possible test.

It should select a small number of targets whose semantic geometry differs enough to expose weaknesses in the model.

A useful progression is:

```text
Domain value space
    -> reusable invariant over world state
    -> Feature transition with real persistence
    -> Infrastructure realization
    -> complete actor journey
    -> adversarial actor/world exploration
```

Each experiment should answer two questions.

### Product question

```text
What does this target claim about the system or world?
```

### Testing-model question

```text
Does our Verification / Challenge model produce useful evidence
without merely restating the implementation?
```

The second question is as important as the first during this phase.

---

# 2. Existing product surfaces that make the experiment possible

## Folders service

The current Folders test project already contains a realistic service-level habitat:

```text
FoldersWebApplicationFactory
    +
real Folders Infrastructure
    +
real SQL-backed persistence
```

The existing integration suite exercises `AddFile`, `RemoveFile`, `DownloadFile`, and `CopyFile`, and inspects the SQL materialization directly after execution.

This is already close to the claim-relative surface envisioned by the testing approach.

It should be reused rather than replaced by an artificial testing architecture.

## Ticket Support product

The current Ticket Support final E2E already provides a broad regressive baseline.

It launches the product, provisions four minimal actors, authenticates through real browser sessions, exercises administration and workflow stages, persists drafts and transitions, verifies tray behavior, and finally checks SQL state for the same Ticket identity.

The represented actors are currently:

```text
Requester
Analyst
Agent
Administrator
```

The existing acceptance documentation explicitly records several useful limits:

- some executable Features are not currently exposed through the UI;
- `Perfil` is currently visual input whose persistence semantics remain unresolved;
- workflow-history projection still contains a deterministic fallback;
- `Agent -> Monitoring` is current product evidence rather than a promoted generic workflow law.

These limits are especially valuable for Progressive Testing because they are already known boundaries between represented knowledge, implementation, and unresolved semantics.

---

# 3. Initial hypothesis H1: Regressive testing can be derived from represented semantic knowledge

## Target

Start with a small Domain Type such as:

```text
StreetExtension
```

and a richer Domain Type such as:

```text
EmailAddress
```

## Product claims

Examples already represented include:

```text
StreetExtension
    non-whitespace
    length boundaries
    split/refinement structure
    stable representation

EmailAddress
    trim normalization
    non-whitespace
    maximum length
    valid-email intrinsic
    case-insensitive equality
```

## Verification experiment

Without reading the concrete C# implementation first, derive candidate Verification obligations from the VSIR.

For example:

```text
known accepted boundary
known rejected boundary
normalization relation
expected failure ownership
representation stability
semantic equality
```

Then lower those candidates to executable tests against the current implementation.

## Hypothesis under test

> Production VSIR contains enough semantic information to derive useful regression evidence without reproducing its rules manually in test code.

## Evidence to record

- which candidate tests were derivable directly;
- which required interpretation not present in the VSIR;
- which required implementation inspection;
- whether generated assertions corresponded to semantic claims rather than implementation structure;
- whether any existing behavior contradicted the represented claim.

## Failure of the testing hypothesis

The experiment weakens H1 if useful Verification requires repeatedly reading implementation details that the VSIR was expected to make unnecessary.

---

# 4. Initial hypothesis H2: Progressive Domain testing can discover semantic questions, not only invalid inputs

## Target

`StreetExtension` is a strong first Challenge target because its current semantics combine raw-input constraints, structural splitting, and canonical representation.

## Challenge families

Generate several semantically distinct candidates rather than a large random corpus.

Examples:

```text
multiple regular spaces
leading/trailing spaces
tab separator
non-breaking space
zero-width characters
mixed separators
raw input whose padding changes length classification
different raw inputs that canonicalize to the same representation
```

## Hypothesis under test

> Progressive Testing can use represented semantics to generate questions whose useful outcome may be a model ambiguity rather than a simple pass/fail result.

## Expected classifications

A candidate may reveal:

```text
implementation defect
semantic ambiguity
intrinsic-definition gap
accepted canonicalization
irrelevant candidate
```

## Important criterion

The experiment is successful even if no bug is found, provided it produces meaningful questions that distinguish:

```text
what the VSIR explicitly says
what the implementation happens to do
what has not yet been semantically decided
```

This is a direct test of the proposition that Progressive Testing is knowledge-seeking rather than merely broader regression.

---

# 5. Initial hypothesis H3: Invariant testing must generate world states, not only input values

## Target

Use a database-backed invariant such as:

```text
ExistingAccount
```

and later:

```text
EmailNotInUse
```

## Product model

A database-backed invariant is not merely:

```text
Input -> Result
```

It is closer to:

```text
Input x WorldState -> Result
```

because the same input may be admissible or inadmissible depending on persisted observations.

## Verification candidates

For `ExistingAccount`:

```text
existing id -> stored semantic Account
missing id -> expected AccountNotFound
successful observation -> no database mutation
```

For `EmailNotInUse`:

```text
absent email -> accepted
present email -> expected rejection
```

## Progressive candidates

```text
same semantic identity under different technical representation
case variation
schema-valid but Domain-invalid stored data
record changed between related observations
concurrent insertion around availability check
```

## Hypothesis under test

> A Challenge campaign can derive meaningful `Input x WorldState` exploration from the invariant's declared capabilities and observations.

## Secondary question

Can this be done without yet creating a complete Infrastructure VSIR?

If the campaign needs schema or adapter inspection, that requirement should be recorded as evidence about the future role of Infrastructure representation.

---

# 6. Initial hypothesis H4: Infrastructure should be tested as semantic fidelity between spaces

## Target

Use Folders persistence because the Domain/Infrastructure relationship is unusually explicit:

```text
Folder
    reconstructed from persisted AttachedFile rows
```

and the current tests already read SQL rows directly.

## Regressive realization experiments

For valid Domain values:

```text
Domain
    -> repository write
    -> SQL
    -> repository read
    -> Domain
```

Observe preservation of:

```text
identity
path
file order
name
content type
content bytes
audit data
```

## Progressive technical experiments

Mutate SQL directly to construct technical worlds that normal Application flows would not create.

Candidate classes:

```text
invalid AttachedFile identifier
invalid FileName representation
unexpected ContentType
corrupted FolderPath component
position collision
position gap
reordered rows
invalid audit identity
valid SQL row that cannot reconstruct a Domain Type
```

The objective is not to force every candidate through Domain constructors first.

The point is to explore:

```text
T = technical representable space
V = Domain valid space
```

and ask which `T` values decode into `V`, which fail safely, and whether any silently create semantic loss.

## Hypothesis under test

> Infrastructure testing becomes more useful when expressed as fidelity and safe-decoding laws rather than CRUD interaction checks.

## Important evidence

Record whether the current VSIR alone can describe the relevant obligation or whether the technical schema/mapping contributes indispensable semantics.

That result should inform, but not prematurely force, a future Infrastructure/adapter representation.

---

# 7. Initial hypothesis H5: Feature testing should preserve transitions through realistic Infrastructure

## Target

Use the existing Folders Features:

```text
AddFile
RemoveFile
CopyFile
DownloadFile
```

`AddFile` is the preferred first target because it performs:

```text
read world state
-> generate identity
-> establish AttachedFile
-> construct new Folder state
-> persist
-> return persisted Folder
```

## Regressive transition experiments

Starting from a valid persisted world:

```text
AddFile
    preserves previous files
    appends one semantically new file
    preserves order
    preserves supplied content
    returns reconstructible persisted state
```

For `CopyFile`:

```text
source remains unchanged
copy receives different identity
semantic content is preserved
destination persists the copy
```

## Progressive transition experiments

Vary world and history rather than only Request values:

```text
same Feature repeated
source and destination equal
multiple files with ordering pressure
ID-generation collision
concurrent AddFile
RemoveFile racing with DownloadFile
CopyFile racing with RemoveFile
interruption before persistence
```

Not every case is expected to have a predefined correct answer.

Some may expose missing concurrency or idempotency semantics.

## Hypothesis under test

> Feature Challenge is naturally stateful and transition-oriented, and the faithful evidence surface normally includes the real adapters required by the Feature.

This experiment should help determine whether a fake/model repository is useful as an independent oracle rather than as a substitute for the real Infrastructure execution.

---

# 8. Initial hypothesis H6: Existing E2E can serve as executable memory of documentation

## Target

Use:

```text
TicketSupportFinalE2ETests
```

as the first broad Regressive baseline.

The existing test already captures a strong scenario:

```text
Administrator
    -> maintain Project / Incident Type catalogs

Requester
    -> register Ticket

Analyst
    -> persist Support Evaluation draft
    -> round-trip draft
    -> advance workflow

Agent
    -> persist Operational Approval draft
    -> round-trip draft
    -> advance to Monitoring

Agent fresh session
    -> observe prior stage data
    -> update Monitoring

SQL
    -> prove final persisted continuity of the same Ticket
```

## Hypothesis under test

> A functional/E2E scenario can operate as executable memory for claims that exist only at product scale and cannot be faithfully compressed into local tests.

## Experiment

Map every meaningful assertion of the current E2E to one of:

```text
explicit documentation claim
VSIR-derived claim
current implementation expectation
migration-specific acceptance condition
```

Any assertion that cannot be traced to a meaningful claim should be questioned.

Any important documented claim with no observable evidence should be identified.

## Desired result

A first approximation of **claim coverage**, rather than code coverage.

---

# 9. Initial hypothesis H7: Progressive E2E can act as functional red teaming

## Target

Use the same Ticket Support product, but do not simply replay the known tour with different values.

Treat the accepted workflow as a set of claims and deliberately attempt to falsify them through realistic actor behavior.

## Initial Challenge strategies

### Actor substitution

Attempt valid workflow actions from the wrong product actor or from a session whose permissions changed.

Questions:

```text
Can Requester reach Analyst behavior through direct navigation?
Can Analyst reach Administration through a guessed route?
Can Administrator accidentally enter operational workflow through another surface?
```

### Stale-session / stale-tab behavior

```text
Actor A opens Ticket
Actor B advances Ticket
Actor A attempts the old action
```

Observe whether stale UI state can produce an invalid transition or misleading result.

### Retry and duplicate action

```text
double click
refresh after submit
browser retry
resubmit same transition
```

Observe identity continuity, duplicate effects, and user-visible state.

### Interruption and resumption

```text
save draft
sign out
new session
resume
```

or:

```text
fill unsaved state
refresh
navigate away
return
```

Distinguish accepted draft semantics from accidental browser-state behavior.

### Concurrent actors

```text
two Analysts open same Ticket
two valid actors attempt conflicting transitions
Agent opens approval while another actor changes relevant state
```

These challenges may expose missing concurrency semantics rather than conventional defects.

### Navigation adversary

Use:

```text
direct URLs
browser back/forward
refresh
old bookmarks
route manipulation
```

The target claim is not merely route security. It is whether navigation can expose a semantic action that the current product model says should not be available.

## Hypothesis under test

> Progressive E2E can function as adversarial engineering against functional semantics, analogous to red teaming but not limited to security vulnerabilities.

## Promotion rule

A discovered path should not automatically become a permanent E2E case.

After interpretation it may become:

```text
new documentation/VSIR knowledge
lower-level invariant Verification
Feature regression
security regression
one minimized E2E regression
a recurring Challenge strategy
or discarded noise
```

---

# 10. Initial hypothesis H8: Progressive testing can falsify documentation or modeling assumptions

The Ticket Support acceptance documentation already exposes unresolved or provisional areas.

These are especially valuable targets because they make the distinction between implementation correctness and model correctness observable.

## Candidate A: `Perfil`

Current product documentation states that `Perfil` is visible in registration but its functional contract is unresolved and the composer does not currently propagate it into `RegisterTicket`.

A useful Challenge is not:

```text
assert that Profile is persisted
```

because that would invent knowledge.

The useful question is:

```text
Does the real business workflow require Profile to survive registration?
```

This requires returning to product/domain evidence, not merely code.

The executable system can expose the gap, but human/domain interpretation must decide it.

## Candidate B: workflow history

The current product documents a deterministic fallback rather than authoritative workflow history.

Challenge can ask:

```text
Which product claims would become false if users interpreted this projection as real historical evidence?
```

This may produce a documentation/UI clarification, a new Domain concept, or no change.

## Candidate C: Agent -> Monitoring

The current mapping is explicitly provisional product evidence.

Challenge can ask whether real roles, handoffs, and organizational responsibility support that mapping.

Again, the expected output is not necessarily a test failure.

It may be revised understanding.

## Hypothesis under test

> Progressive Testing can produce useful evidence against the semantic source itself, not only against implementation.

This is one of the most important hypotheses in the entire approach.

---

# 11. Initial hypothesis H9: Challenge generation should be diverse while lowering remains reproducible

## Experiment

For one target from each scale:

```text
StreetExtension
ExistingAccount
AddFile
Ticket Support E2E workflow
```

ask an exploratory agent to generate several candidate Challenges from the same semantic source in separate runs.

The expected output is not test code first.

It should be a reproducible candidate description containing at least:

```text
target claim
exploration strategy
world/setup mutation
stimulus/action
relevant observations
reason this candidate differs from the others
```

Then lower accepted candidate descriptions through a constrained test-generation step.

## Hypothesis under test

> Useful diversity belongs primarily in hypothesis/scenario generation, while concrete lowering and retained regression should remain reproducible.

## Evidence to measure

- semantic novelty between candidates;
- redundant candidates;
- candidates that simply mirror implementation;
- candidates that require undocumented assumptions;
- compile/lowering repair burden;
- human review burden;
- number of candidates that produce genuinely distinct evidence.

This experiment should help define what "divergent testing AI" actually means operationally.

---

# 12. Initial hypothesis H10: Human review is the real economic bottleneck

AI-assisted testing makes candidate production cheap.

The experiment should therefore explicitly measure whether the proposed process merely moves cost from writing tests to reviewing noise.

For every Challenge campaign, record:

```text
candidates generated
candidates executed
candidates producing distinct observations
candidates rejected as redundant
candidates rejected as semantically invalid
findings requiring human interpretation
findings promoted
```

A useful target is not:

```text
100 generated tests
```

but something closer to:

```text
broad exploration
    -> clustering / shrinking / reduction
    -> a small number of meaningful findings
```

## Hypothesis under test

> Progressive Testing is economically viable only if generation is broad but human review remains narrow.

This should influence future candidate minimization, clustering, and evidence-report design.

---

# 13. Suggested first execution order

The initial experiments should increase semantic and environmental complexity gradually.

## Stage 1 - pure semantic space

```text
StreetExtension
EmailAddress
```

Purpose:

- test VSIR-derived Verification;
- test boundary/metamorphic Challenge;
- identify semantic ambiguity;
- learn what a candidate description needs.

## Stage 2 - semantic knowledge depending on world state

```text
ExistingAccount
EmailNotInUse
```

Purpose:

- generate world states;
- use real database observation;
- distinguish invariant semantics from repository behavior.

## Stage 3 - real transitions

```text
Folders / AddFile
Folders / CopyFile
```

Purpose:

- test state-transition obligations;
- compare returned and persisted state;
- introduce stateful/concurrent Challenges;
- test Infrastructure fidelity.

## Stage 4 - technical corruption

```text
Folders SQL representation
```

Purpose:

- mutate real persisted representation;
- explore schema-valid/domain-invalid states;
- determine what Infrastructure knowledge is missing from current VSIR.

## Stage 5 - whole product regression

```text
TicketSupportFinalE2ETests
```

Purpose:

- map existing assertions to documentation/semantic claims;
- establish claim coverage at product scale;
- preserve the known migration scenario as executable memory.

## Stage 6 - whole product progressive exploration

```text
Ticket Support functional red-team campaign
```

Purpose:

- actor substitution;
- stale sessions;
- retries;
- direct navigation;
- concurrency;
- workflow interruption;
- model/specification challenges.

The sequence should stop whenever an earlier stage exposes a missing concept that materially blocks interpretation of the later ones.

The purpose is not to rush toward E2E generation. It is to let each scale teach us what the testing model lacks.

---

# 14. Minimum initial tooling

The first experiment does not require a new testing framework.

Reuse existing product infrastructure where possible.

## Existing components

```text
xUnit
WebApplicationFactory
real SQL-backed service Infrastructure
Playwright
Aspire DistributedApplicationTestingBuilder
current VSIR files
current product documentation
```

## New experimental components

Initially these can be lightweight scripts or AI workflows rather than Framework APIs.

### Semantic target reader

Reads the target VSIR and its directly referenced semantic dependencies.

### Obligation worksheet

A temporary structured representation of:

```text
claim
source
possible Verification evidence
possible Challenge dimensions
required evidence surface
```

This can begin as generated Markdown or JSON. It does not need to become test IR yet.

### Candidate generator

Produces a small set of semantically differentiated candidates.

For early experiments, deliberately cap the batch around:

```text
1 to 5 candidates
```

so the human can inspect whether diversity is actually useful.

### Candidate lowerer

Converts an accepted candidate into the current test surface:

```text
xUnit
SQL setup/mutation
WebApplicationFactory
Playwright
```

This may initially still use AI, but should operate under tighter constraints than the exploratory generator.

### Evidence report

For each candidate record at least:

```text
target
source claim
candidate description
execution surface
observed result
classification proposal
human decision
promotion outcome
```

Provenance details such as fingerprints, model versions, and seeds can be added later without blocking the first experiment.

---

# 15. What should not be built yet

The experiment should avoid prematurely implementing:

- a final Verification VSIR grammar;
- a final Challenge VSIR grammar;
- a generalized Infrastructure VSIR;
- a universal environment planner;
- a permanent AI-agent topology;
- an automatic promotion mechanism;
- a coverage score for Progressive Testing;
- a large generated regression suite.

The purpose of this battery is to discover what those things would actually need to represent.

---

# 16. Success criteria for the first battery

The battery should be considered useful if it can answer some of the following with concrete evidence.

### Derivability

Can useful evidence obligations be derived from current VSIR without restating semantics?

### Divergence

Can Challenge produce meaningful candidates that a deterministic Verification generator would not naturally produce?

### Fidelity

Does choosing evidence surface by semantic relation produce better findings than mechanically preferring unit or whole-system tests?

### Knowledge discovery

Can at least one candidate expose a genuine ambiguity, missing assumption, or design question even if no implementation bug exists?

### Infrastructure gap

Can technical corruption experiments identify which semantics belong to adapters/schema but are absent from current VSIR?

### Product-scale value

Can Ticket Support E2E evidence be mapped back to documentation claims strongly enough to function as executable memory of the migration?

### Progressive E2E value

Can functional red-team exploration discover a meaningful workflow, authorization, stale-state, concurrency, or modeling question beyond the known happy path?

### Promotion

Can one progressive discovery be followed through:

```text
Challenge
    -> evidence
    -> human interpretation
    -> accepted knowledge
    -> appropriate regression protection
```

### Review economics

Can broad generation be reduced to a small number of human-reviewable findings?

---

# 17. Strongest initial falsification targets

If only a few experiments can be executed, prioritize targets capable of falsifying the testing approach rather than merely demonstrating it.

## Falsifier A

If Domain VSIR cannot produce useful test obligations without reading the generated C# implementation, then the current representation is less operationally examinable than expected.

## Falsifier B

If Progressive candidates are mostly random edge cases or implementation mirrors, then Challenge lacks a sufficiently distinct generative objective.

## Falsifier C

If real Infrastructure adds no relevant evidence beyond pure/fake execution for current Features, then the claim-relative fidelity hypothesis may be overstating the need for realistic adapters.

## Falsifier D

If the Ticket Support final E2E cannot be meaningfully traced back to documentation/semantic claims, then E2E may be preserving implementation behavior rather than engineering knowledge.

## Falsifier E

If functional red-team candidates produce only already-known invalid actions and no useful semantic questions, then the hacking/adversarial analogy may be narrower than currently hypothesized.

## Falsifier F

If human review cost grows roughly linearly with candidate generation and cannot be reduced through clustering, shrinking, or structured evidence, then AI-generated progressive testing may be economically self-defeating.

These negative outcomes would be valuable findings.

The purpose of the experiment is not to prove that the VSlices testing approach is correct.

It is to make the approach itself falsifiable.

---

# 18. Compact experimental roadmap

```text
1. StreetExtension
   derive claims -> verify -> challenge boundaries

2. EmailAddress
   add normalization + equality + representation questions

3. ExistingAccount / EmailNotInUse
   add generated world state + real database

4. AddFile / CopyFile
   add state transitions + persisted outcomes

5. Folders SQL corruption
   add technical-space challenge

6. TicketSupportFinalE2E
   map documentation -> executable regression evidence

7. Ticket Support progressive E2E
   adversarial actors + stale state + retries + concurrency

8. Promote one finding
   Challenge -> interpretation -> semantic decision -> Verification
```

If this sequence works, VSlices will have exercised the same abstract testing model from one Domain value through an entire multi-actor product journey.

That would provide substantially stronger evidence for the approach than designing a complete testing DSL in advance.
