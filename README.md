# ProcedurallyAssistedTerrainGenerator
A procedutal assitant terrain generator for unity

Build Surface Functions with a graphical node based interface. 
Use all unity Mathf functions, create constants, use the local sample point of the chunk
or the sample point in world coordinates. 
No programming needed.
Use those to generate your terrain chunks with the marching cubes algorithm.

Change the shape and amount of chunks you generate.
All in editor, and multithreaded.

TODO:
Write TCP Command Relay server for unity blender communication.
Write blender plugin that connects to Command Relay Server.
Write Unity script that manages starting and stoping of server in editor, as well as a script that communicates with server.
Send chunk meshes between unity and blender for easy mesh manipulation with a tool dedicated to that.
