import math
import bpy

# Get exporter file, directory can be changed easily
directory = '/Users/xingze/Desktop/gamelab-2018-team2/Project/'
modelDir = directory + 'BRS/BRS/Content/Models/polygonheist/'
file = directory + 'Unity/Scene Exporter/export1.txt'

# Read file line by line
f = open(file,'r')

# Initialize parameters
names = []
amount = []
prefab = []
prefabs = []


# Read .txt specification
for line in f:
    if line.startswith('<begin>') or not line.strip():
        continue
    
    # Ensure all the parameters are associated to correct prefabs
    if line.startswith('<end>'):
        prefabs.append(prefab)
        prefab = []
        continue
    
    content = line.split(': ')[1].split('\n')[0] 
    if line.startswith('prefabName'):
        names.append(content)
        prefab.append(line)
        continue
    
    if line.startswith('amount'):
        amount.append(int(content))
        continue
    
    prefab.append(line)
    
# Import each prefab the amount of times and set the parameters accordingly
for i in range(len(names)):
    pos = []
    rot = []
    sca = []
    prefab = prefabs[i]
    
    for lineNum in range(len(prefab)):
        line = prefab[lineNum]
        content = line.split(': ')[1].split('\n')[0] 
        
        if lineNum == 0:
            if content != names[i]:
                print("prefab wrong!")
                break
            else:
                continue
        vector3 = list(map(float,content.split()))
        x = vector3[0]
        y = vector3[1]
        z = vector3[2]
        
        if line.startswith('pos'):
            pos.append(vector3)
            continue
    
        if line.startswith('rot'):
            # Convert degree to radians
            vector3 = list(map(math.radians,vector3))
        
            # Convert from left-handed coordinate system to right-handed
            vector3[0] -= math.pi/2
            #vector3[1] = math.pi
            rot.append(vector3)
            continue
    
        if line.startswith('sca'):
            # Scale up 100 times for Blender
            #vector3 = [i * 100 for i in vector3]
            sca.append(vector3)
            continue
          
    for j in range(amount[i]):
        # Import the prefab
        bpy.ops.import_scene.fbx(filepath = modelDir + names[i] + '.fbx')
        
        if j == 0:
            objName = names[i]
        else:
            objName = names[i] + "." + "{0:0=3d}".format(j)   
             
        # Set position, rotation and scale for each component
        bpy.data.objects[objName].location = pos[j]
        bpy.data.objects[objName].rotation_euler = rot[j]
        bpy.data.objects[objName].scale = sca[j]    