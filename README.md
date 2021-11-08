# Percubed_BEAPS
Integrating KADAPT (TreeSharpPlus, FinalIK) and Nvidia Flex to test and simulate behaviors for PBD/voxel based virtual agents

The repository is part of my PhD thesis and a larger project regarding expressive virtual agents

Simulating voxel based agents is a non-trivial task and assigning goals and control schemes for the same is tedious

We integrate Nvidia Flex and KADAPT, an agent prototyping testbed which allows for easy integration of various simulation packages in the same control testbed

Using Unity's anonymous functions we can imbue the graphical mesh of the agent with volumetric dynamics to expand the allowable action space of the agent

By allowing particle level access of the underlying character's volume, we can create robust virtual agents not limited to actions dictated by IK but also volumetric dynamics

This opens the door for new and innovative game mechanics as well

![](https://github.com/skore11/Percubed_BEAPS/blob/main/GIF%209-22-2021%206-26-07%20PM.gif)

![](https://github.com/skore11/Percubed_BEAPS/blob/main/GIF%209-21-2021%203-06-36%20PM.gif)

![Alt Text](https://github.com/skore11/Percubed_BEAPS/blob/main/GIF%209-21-2021%203-49-58%20PM.gif)

![](https://github.com/skore11/Percubed_BEAPS/blob/main/GIF%209-21-2021%203-54-36%20PM.gif)

We have tested the BEAPS system in creating natural secondary motion, expressive torso and limb extensions, real tim collision detection, user deformed free form deformations and conditional logic based on the same

KADAPT allows for a hierarchical modular bheavior tree model to control our agents.

