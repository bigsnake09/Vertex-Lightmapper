# About
Vertex Lightmapper is the public release of the Unity based light mapping tool developed for and used in BallisticNG. By storing lighting information in mesh vertex colors and using a shader that reads them to tint the mesh, you can create lighting that mimics 3D games from the PS1 era.

Because of how VLM works it is not compatible with tools that store information in the RGB vertex color channels for interpolating data (texture blending, etc). VLM attempts to leave the alpha channel intact so anything using that channel should be fine. If you're planning to use this alongside such a tool then I recommend that you create backups first!

Two example scenes are included to provide a basic showcase of terrain lighting baked with VLM. Two shaders are also included, these are taken directly from the Unity Tools package shipped with BallisticNG.

# Usage
## Scene Preperation
* If you do not plan to use Unity's lightmapper then head over to the lighting panel (Window -> Rendering -> Lighting) and turn off Auto Generate at the bottom
* Set any meshes you want to be baked using VLM to lightmap static. The option for this is at the top right of the inspector, to the right of the name.
* VLM does not instantiate meshes, so make sure anything you're baking is a unique mesh if you want to avoid conflicts in the same scene. If you're using the same mesh but with different lighting conditions in a separate scene then this doesn't matter!

## Lights
* VLM uses Unity's built in light components. Point, spot and directional types are supported.
* Setting the shadow type of a light to either hard or soft will trigger VLM to bake shadows for that light, the strength property is also used!
* All 3 ambient sources from the lighting panel are supported. Note that you will need to allow Unity to automatically generate lighting (or generate it manually) for the skybox source mode to work.

## Light Sponges
```
Menu              : GameObject -> Light -> Light Sponge
Add Component     : Rendering -> VLM Light Sponge
```
Light sponges are used to absorb ambient and directional lighting.

**Shape** - The shape controls whether the light sponge is a sphere or box.

**Ignore Opposite Normals** - Whether the light sponge should ignore normals that are not facing it

**Intensity** - How much light to absorb.

**Radius/Bounds** - The extent of the sponge.

## Bake Options
```
Add Component     : Rendering -> VLM Bake Options
```

Bake options is a script component that allows you to determine whether the mesh is ignored by the lightmapper and provides independent control over casting/receiving shadows so you don't need to touch the mesh renderer settings.

## Baked Data
Baked data is stored in a script called VLM Baked Data, which is attached to any mesh gameobject that has had colors baked. Colors are re-applied to a mesh every time Unity's Start method is called both in-game and in-editor.

This script is standalone and can be safely removed from objects if you want to stop baked colors from being applied.

## The Bake Process
All of the methods in the code base have been suitably abstracted so you can easily build your own bake process and expand on what's already there if you need to. Out of the box this is the default bake process:

```
- Discover lights
- Discover lightmap static objects not set to be ignored
- Attach non saveable (Unity hideflags) mesh collider to all discovered meshes that don't already have one attached
- Iterate through every discovered mesh and for each vertex:
    * Apply ambient color
    * Add directional light colors
    * Multiply color so far by color sponges
    * Add point/spoit light colors
- Attach baked data component to store colors
- Clean up created mesh colliders
```
