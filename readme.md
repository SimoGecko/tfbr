# Game Programming Lab

## Feature - Lighting

### Contents
This branch contains scripts, experiments and assets for lighting:

#### Unity experiment: dungeon
   Experiment with Unity lighting and the simple-dungeon assets. It contains two 
   scripts: `Move.cs` and `ChangeColor.cs`.
1. `Move.cs` takes in parameters like the rotation axis, rotation angle, orbit center 
     and translations. This script can be used to move, rotate or orbit lights. It
    can also be used for specifying movements of other objects.
   
2. `ChangeColor.cs` takes in input colors and create a disco effect by changing between the colors.
     It can also be used to create effect of dim light.

    
#### Lighting experiment: Effects
Experiment with Lighting in mono game. Here are some highlights:
    
###### Simple lighting (can use directly)

- Default lighting: simply use `effect.EnableDefaultLighting();`
    
- Directional lighting: use `effect.DirectionalLight0`,`effect.DirectionalLight1`,
    `effect.DirectionalLight2`. This method supports
    up to 3 directional lights, and can set direction, diffuse color and specular color of the directional lights directly.
    

###### Advanced lighting (requires writing shaders) 

- Point light

- Spot light

- Pre-baked light maps
  Scenes in Unity can be built as light maps (for example the sky box light map baked for the environment as in the `dungeon/Asset/LightMaps`).
  Light on textures can be baked in Unity or Blender.






