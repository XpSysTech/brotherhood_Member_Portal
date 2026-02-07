# Aggregates – Application Layer

## What is an Aggregate?

In this application, an **Aggregate** represents the *result of a calculation or grouping*
performed over one or more domain entities.

**Aggregates:**
- Are **read-only**
- Do **not** represent persisted entities
- Do **not** contain behavior
- Represent **facts**, not intentions or commands

**They answer questions like:**
- “What are the totals?”
- “What happened over time?”
- “What is the computed state?”

### Plain-English meaning of “aggregate”

Aggregate = to collect things together and summarize them

In everyday language:
- Aggregate marks = total marks
- Aggregate score = combined score
- Aggregate data = grouped + calculated data

**So at the most basic level:**
	An aggregate is a result produced by grouping and combining data.

---

## When should Aggregates be used?

**Use an Aggregate when:**
- Data must be **derived**, not stored
- Multiple rows/entities are combined into a **summary**
- The result is used for:
  - reporting
  - dashboards
  - ownership calculations
  - financial summaries
- The result should be reusable across:
  - REST APIs
  - GraphQL APIs
  - background jobs
  - exports

**Do NOT use aggregates for:**
- Write operations
- Commands
- State mutation
- Validation logic

### Derived Data

**What is Derived Data?**
- Derived data = data that is calculated from other data, not stored as the original truth
		- Simple terms: If you can recalculate it from existing records, it is derived data.

**Why don't you store derived data?**
- Storing derived data risks it becoming out-of-date or inconsistent
	- Example:
		1. A deposit is corrected
		2. But the stored TotalSavings is not updated
		3. Now your system lies
			-> This is what we call inconsistent data

**Derived ≠ Cached ≠ Stored**
- Important Distinction:
	- Derived data is calculated on-the-fly from source records
	- Cached data is temporarily stored for performance, but can be recalculated
	- Stored data is permanently saved in the database
- Derived data can be cached later, but should not be the primary source of truth.

---

## Why Aggregates exist in the Application layer

**Aggregates belong in the Application layer because:**
- They represent **business calculations**
- They are independent of persistence technology
- They protect query logic from leaking into controllers
- They allow multiple DTO projections from a single source of truth

---

## Current Aggregates in this project

### MonthlyFinanceAggregate

**Purpose**
Represents the computed financial totals for a given month.

**Fields**
- Year
- Month
- TotalSavings
- TotalOpsContribution
- ContributingMemberCount

**Derived From**
- Approved `Finance` records

**Used By**
- FinanceQueryService
- FundFinanceSummaryDTO mapping
- Member ownership calculations
- Monthly history reporting

**Typical flow:**
	Finance (Entity)
	   ↓
	MonthlyFinanceAggregate
	   ↓
	MonthlyFinanceSummaryDTO
	   ↓
	API Response

---

## Aggregates vs DTOs

| Aggregates | DTOs |
|----------|------|
| Represent facts | Represent API contracts |
| Used internally | Used externally |
| Derived from entities | Derived from aggregates or services |
| Stable | Change with UI/API needs |

---

## Aggregates vs Domain Entities

| Aggregate | Entity |
|--------|-------|
| Not persisted | Persisted |
| Read-only | Mutable |
| Derived | Source of truth |
| No identity | Has identity |

---

## Design Rules

- Aggregates must never write to the database
- Aggregates must not depend on controllers
- Repositories may return aggregates
- Services map aggregates → DTOs

This separation keeps the system:
- auditable
- testable
- adaptable
