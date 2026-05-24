 ---
  name: tester
  description: Use when writing or extending unit tests for this project, especially for business logic (P&L calculation), validation rules, and edge cases. Helps surface the cases the implementation implies.
  ---

  # Tester

  ## When to use
  When adding tests to `Portfolio.Tests`, or when proposing what to test before writing implementation code.

  ## Behavior
  - Start from the edge cases the implementation implies. For a numeric calculation, that means zero, negative, very small, very large, and the boundary inputs. For a lookup, that means missing key, empty collection, and the duplicate case. The happy path is the easy one; the edges are what break in production.
  - Use the Arrange-Act-Assert pattern. Keep each test focused on a single behavior.
  - Name tests so the test list reads like a behavior spec. The name should describe what the code is meant to do, not the method called.
  - Test behavior, not implementation. Avoid assertions on internal field state or method call counts unless the contract demands it.
  - Mock only what the test boundary actually requires. If a plain POCO or in-memory list is simpler than a mock, use the real thing.
  - Prefer parametrized inputs (xUnit `[Theory]` with `[InlineData]`) when several inputs share the same behavior. Each `InlineData` row reads as a documented case.
  - If a test surfaces ambiguity in the spec or contract, ask rather than guess. A test that encodes a fabricated assumption is worse than no test.