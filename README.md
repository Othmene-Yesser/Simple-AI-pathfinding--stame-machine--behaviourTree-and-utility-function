# IntroAI
A time trial mini-game with a specific focus on creating and optimizing the AI components. The main AI components include:

Finite State Machine (FSM)
Behaviour Tree (BT)
Utility Function
Finite State Machine (FSM)
Definition: FSM is a computational model used to design the AI's behavior by defining a set of states and transitions between those states based on specific conditions or events.
Implementation:
You created distinct states for different AI behaviors (e.g., idle, moving, attacking).
Each state has specific actions and transitions that are triggered by events or conditions (e.g., spotting the player, reaching a waypoint).
Behaviour Tree (BT)
Definition: BT is a hierarchical model used to control the decision-making process of the AI by organizing behaviors into a tree structure.
Implementation:
You structured the AI's behaviors in a tree format where each node represents a task or a decision.
The tree is traversed from the root to the leaves, evaluating conditions and executing actions based on the outcome of those evaluations.
This allows for modular and reusable behavior design, making it easier to manage complex AI logic.
Utility Function
Definition: Utility functions are used to make decisions based on a scoring system where different options are evaluated and the one with the highest score is chosen.
Implementation:
You implemented utility functions to evaluate different actions the AI can take, scoring them based on factors such as distance to the goal, health level, and time remaining.
The AI selects the action with the highest utility score, allowing for more dynamic and context-aware decision-making.
Integration
The FSM, BT, and utility functions were integrated into the AI system to work together seamlessly.
The FSM manages the overall state transitions, while the BT handles more granular decision-making within those states.
Utility functions provide a way to prioritize actions dynamically, ensuring that the AI can adapt to changing circumstances in the game.
Results
The AI demonstrated improved decision-making capabilities, efficiently navigating the game environment and responding to player actions.
The combination of FSM, BT, and utility functions allowed for a robust and flexible AI system that can handle complex behaviors in a time-sensitive game scenario.
This structured approach to AI development provided a comprehensive framework for creating intelligent and responsive game characters, enhancing the overall player experience in the time trial mini-game.





