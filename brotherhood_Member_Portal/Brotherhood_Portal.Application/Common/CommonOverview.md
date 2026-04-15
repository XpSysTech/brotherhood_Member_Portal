# Application Layer – Common

This folder contains **shared application-level primitives** that are used across multiple services, use cases, and entry points (REST, GraphQL, background jobs).

Commons exist to **standardize behavior**, **reduce duplication**, and **keep business logic clean and transport-agnostic**.

---

## 1. What Are “Commons”?

**Commons** are:

> Small, reusable, application-scoped building blocks that support multiple use cases without belonging to any single domain concept.

They are **not business entities**, **not persistence concerns**, and **not UI/transport logic**.

Commons typically include:
- Operation result wrappers
- Outcome/result abstractions
- Cross-cutting application utilities
- Shared conventions for service responses

---

## 2. What Commons Are Used For

Commons solve recurring problems such as:

- How should application services communicate **success vs failure**?
- How do we return **business outcomes** without leaking HTTP concerns?
- How do we avoid throwing exceptions for **expected business states**?
- How do we standardize service responses across controllers and APIs?

They allow services to focus on **what happened**, not **how it is returned**.


### Design Rules for Commons:

Allowed:
- Result wrappers
- Outcome abstractions
- Cross-cutting application primitives

Not Allowed:
- HTTP concerns
- EF Core references
- Controllers or repositories
- Domain business rules

---

## 3. What Commons Are NOT

Commons are **not**:

- ❌ DTOs (they don’t represent API payloads)
- ❌ Domain Entities (they don’t represent persisted state)
- ❌ Infrastructure utilities (no DB, HTTP, logging, or framework dependencies)
- ❌ UI concerns (no status codes, no formatting)

They live **squarely in the Application layer**.

---

## 4. How Commons Are Used (High-Level Flow)

```text
Controller / GraphQL Resolver
        ↓
Application Service
        ↓
Common (OperationResult, etc.)
        ↓
Controller maps result → HTTP / GraphQL response
```

This keeps:

- Application logic transport-agnostic
- Controllers thin and readable
- Business outcomes explicit and testable


---

## Current Commons in this Project

**OperationResult<TOutcome>**

– A generic wrapper for service outcomes that indicates success/failure and carries business results without throwing exceptions.    

**It Communicates:**
- Whether the operation succeeded or failed
- The business outcome (if successful)
- Error details (if failed) - Human-readable messages, error codes, etc.

**Why It Exists**
- To avoid using exceptions for expected business states (e.g., “User not found”)
- Prevent service from returning raw boolean flags or nulls that lack context
- Enables controllers to decided how to respond based on the outcome. (HTTP, GraphQL, Background jobs)
- Keeps services independent of REST/GraphQL concerns while still providing rich information about the operation’s result.

**Who uses OperationResult<TOutcome>?**

1. Used By:
    - Application Services: To return outcomes of business operations without throwing exceptions for expected states.
    - Query / Command Services

**Example In Use**

Successful Deposit Approval:
```
return OperationResult<DepositApprovalOutcome>.Ok(
    DepositApprovalOutcome.Approved,
    "Deposit approved successfully."
);

```

Failed Deposit Approval (e.g., insufficient funds):
```
return OperationResult<DepositApprovalOutcome>.Fail(
    DepositApprovalOutcome.AlreadyApproved,
    "Deposit has already been approved."
);
```

In a Controller:
```
return result.Outcome switch
{
    DepositApprovalOutcome.Approved =>
        Ok(new { message = result.Message }),

    DepositApprovalOutcome.AlreadyApproved =>
        Conflict(new { message = result.Message }),

    _ =>
        BadRequest(new { message = result.Message })
};

```