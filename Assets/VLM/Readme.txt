Thanks for downloading Vertex Light Mapper! This readme provides a brief overview of the tool.

VLM is a public release of the internal lightmapping built for and used in BallisticNG. It's designed to be used with Unity 2018+, it should however work with older versions.

HOW IT WORKS
===========================================================
Vertex Light Mapper works by calculating and storing lighting information on a per vertex level. When paired with shaders that use the mesh vertex colors you can create lighting that mimics retro 3D games.

Please note that VLM is not compatible with tools that use the RGB vertex color channels for interpolating data (texture blending, etc). VLM leaves alpha channels intact so anything using it should be fine. If you are planning to use this alongside such a tool then I recommend you create backups first!
	

SCENE PREPERATION
===========================================================

1. Open the Unity lighting panel (Window -> Rendering -> Lighting) and turn off Auto Generate at the bottom
2. Set anything you want to bake lighting on to lightmap static
3. Open the VLM bake window (Window -> Rendering -> Vertex Lightmapper) and click the bake button when you're ready

LIGHTS
===========================================================

- Use Unity's built in light components. Point, spot and directional are all supported.
- Setting the shadow type of a light to either hard or soft will trigger VLM to bake shadows for that light. The strength property is also used!
- All 3 ambient sources from the lighting panel are supported. Note that you will need to allow Unity to automatically generate lighting or manually generate lighting for the Skybox source mode to work.

LIGHT SPONGES
===========================================================

Light sponges will absorb ambient and directional light in the baking process. Use GameObject -> Light -> Light Sponge to create one.

BAKE OPTIONS
===========================================================

Bake options allow you to control whether an object is used in bakes and provides controls for casting/recieving shadows independant from the mesh renderer options. You can find it in the add component menu (Rendering -> VLM Bake Options).

THE BAKE PROCESS
===========================================================

Just to note first, everything in the code has been suitably abstracted so you can easily build your own bake process if the included one doesn't suite your needs.
The default bake process is as follows:

- Discover lights in the scene
- Discover lightmap static objects not set to be ignored and attach temporary non savable mesh colliders if one doesn't already exist
- Iterate through every discovered mesh and for each vertex:
	- Apply ambient color
	- Add directional light colors
	- Multiply color so far by color sponges
	- Add point/spot light colors
- Attach baked data component to store the new baked colors
- Clean up any of the temporary mesh colliders created in the discovery process


BAKED DATA
===========================================================

Baked color data is stored in the scene via a script that's attached to your mesh objects. These re-apply the baked colors in Unity's start method which executes both in-game and in-editor.

To stop baked data being loaded you can simply remove this script.

PROVIDED SCENES
===========================================================
Day Example and Night Example are included to demonstrate basic usage of the tools. Day showcases directional lights, shadows and cave sponges. Night showcases point lights and spot lights.

PROVIDED SHADERS
===========================================================
A basic vertex lit and normal based terrain blend shader are included. These come directly from the Unity Tools package that ships with BallisticNG. You can find both of them under the VLM Example category in the shader selector.
