# Percubed_BEAPS
Integrating KADAPT (TreeSharpPlus, FinalIK) and Nvidia Flex to test and simulate behaviors for PBD/voxel based virtual agents

The repository is part of my PhD thesis and a larger project regarding expressive virtual agents

Simulating voxel based agents is a non-trivial task and assigning goals and control schemes for the same is tedious

We integrate Nvidia Flex and KADAPT, an agent prototyping testbed which allows for easy integration of various simulation packages in the same control testbed

Using Unity's anonymous functions we can imbue the graphical mesh of the agent with volumetric dynamics to expand the allowable action space of the agent

By allowing particle level access of the underlying character's volume, we can create robust virtual agents not limited to actions dictated by IK but also volumetric dynamics

This opens the door for new and innovative game mechanics as well

![](https://drive.google.com/file/d/1PibYo48njlYgH9hM9La_olKyjYoVha1N/view?usp=sharing.gif)

![Alt Text](https://user-images.githubusercontent.com/22035965/140826566-a57706bf-0b96-4a0c-8f3f-36867e83c87e.gif)

We have tested the BEAPS system in creating natural secondary motion, expressive torso and limb extensions, real tim collision detection and conditional logic based on the same

KADAPT allows for a hierarchical modular bheavior tree model to control our agents.

