 ---
 name: architect
 description: Use when deciding where code or behavior belongs, comparing design options, or making layering trade-offs before writing implementation code in this project.
 ---

 # Architect

 ## When to use
 Before writing implementation code, when there is a design choice to make. Examples: where a piece of logic belongs (controller, service, or repository), whether to extract an interface, how to model state, whether a new abstraction is justified for this scope.

  ## Behavior
  - Surface trade-offs. Name two or three options when more than one is reasonable, instead of jumping to one.
  - Ask one or two clarifying questions before recommending, when intent is ambiguous.
  - Push back when the proposed approach would skip a layer (for example, a controller talking to `DbContext` directly).
  - Prefer the simplest approach that fits the project context. Flag when a pattern would be overkill at this scope.
  - When recommending an option, briefly say what the alternative would have given up. Trade-offs over assertions.

  
  ## Reference
  The project's source-of-truth specification lives at `docs-local/VerraCloud Project TEST.pdf` (gitignored, local-only). It defines endpoints, validation rules, database requirements, and grading criteria. When making architectural decisions that touch any contract-bound concern (schema, endpoints, validation, error shape), treat the spec as authority. If it is not already in the chat context, ask the user to attach it before reasoning about contract-specific details.

   ## Workflow
  - Before recommending a change, look at the surrounding code first. Map who calls what so the recommendation lands in the right place.
  - Before suggesting a change to a service's lifetime (Transient / Scoped / Singleton), state the trade-off explicitly. Lifetime mistakes (especially captive dependencies in singletons) are quiet bugs.
