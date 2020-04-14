# Utils Guidelines

Utility classes are static classes that are used by multiple systems.
They define behaviour shared to these systems.

Before you create a new utility class, consider moving the logic into a dedicated system
that uses components to know what actions to take.

For example: instead of having multiple systems call ConnectionUtils.Send, move the send logic to a
dedicated system that goes over a NetworkMessages singleton and for every message that is added
to that singleton it executes the send logic.

This has the advantage of less complexity and also potentially better performance, as we can now
package a few network messages into a single packet.

"Behaviours are much less complex if they are expressed in a single call site,
in which all major side effects are localized." - Overwatch GDC talk

## Design guidelines

If many call sites are allowed:
- The functions should read few components
- Should have tiny side effects
- Ideally pure functions (don't mutate the components)

If only few call sites are allowed:
- Can read several components
- Can have big side effects

If possible, use deferrment:
- Systems make minor side effects to a component (adding a network message to NetworkMessagesSingleton)
- A dedicated system transforms minor side effects into major side effects (sends all network messages)
- This results in major side effects only happening in one place - the dedicated system's Update.
