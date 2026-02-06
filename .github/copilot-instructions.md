# GitHub Copilot Instructions for Food Poisoning Stack Fix (Continued)

## Mod Overview and Purpose

**Food Poisoning Stack Fix (Continued)** is a RimWorld mod that modifies the way food poisoning affects meal stacks. The existing mechanism in the game applies a uniform chance of food poisoning across an entire stack if any meal in the stack is tainted. This mod changes the behavior to more accurately track the number of poisoned meals within a stack and ensures that there's an appropriate risk when meals are picked up. This provides a more realistic experience that correlates food poisoning cases with the skill of your cook and the cleanliness of your kitchen.

### Credits
Special thanks to Dyrewulfe for the original concept and implementation.

## Key Features and Systems

- **Accurate Poison Tracking:** Implements a mechanism that tracks poisoned meals within a stack, allowing for more precise food poisoning risk.
- **Consistent Randomization:** When meals are picked up, they randomly select from the stack, increasing the likelihood of choosing a non-poisoned meal if most are safe.
- **Correlation to Reality:** Food poisoning cases now feel more aligned with real-world kitchen conditions and cook expertise.

## Coding Patterns and Conventions

- **Class Naming:** Classes are named using PascalCase. For example, `CompFoodPoisonable_PostSplitOff`.
- **Method Naming:** Methods follow PascalCase conventions and are descriptive of their function within the class.
- **Static Classes:** Utility and patch classes, like `CompFoodPoisonable_PostSplitOff`, are static when possible to avoid unnecessary instantiation.
- **Versioning:** This project targets .NETFramework,Version=v4.7.2 and .NETFramework,Version=v4.8.

## XML Integration

RimWorld mods often rely on XML for defining game concepts and components. In this mod, ensure that any definitions related to food stacks, components, or events are reflected in the XML where necessary. Modifications should maintain interoperability with other mods and game updates.

## Harmony Patching

- **Harmony Use:** This mod employs Harmony patches to extend or override game functionality without modifying the game's source code directly.
- **Patch Locations:** Key patches are applied around the absorption and splitting of meal stacks to handle poisoned meal tracking accurately.
- **Examples:** Files such as `CompFoodPoisonable_PostSplitOff.cs` and `CompFoodPoisonable_PreAbsorbStack.cs` contain patches that are crucial for this functionality.

## Suggestions for Copilot

- **Utility Methods:** Copilot can assist in generating utility methods that help in managing meal data, such as calculating the probability of picking up a tainted meal.
- **Patch Skeletons:** Use Copilot to create Harmony patch skeletons, ensuring you have the correct method signatures and patch attributes.
- **Code Refactoring:** Let Copilot suggest alterations to improve code readability and performance while adhering to project-specific conventions.
- **Error Handling:** Include error checking and logging statements in critical areas to ensure smooth integration and debugging.
- **Test Cases:** Guide Copilot to suggest potential test cases that align with various gameplay scenarios to validate mod functionality.

By adhering to these guidelines and utilizing Copilot effectively, you can enhance the development process of the Food Poisoning Stack Fix mod, ensuring it is both robust and user-friendly.
