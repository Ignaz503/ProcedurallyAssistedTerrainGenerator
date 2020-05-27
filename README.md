# ProcedurallyAssistedTerrainGenerator
A procedurally assisted terrain generator for unity.

Build surface functions with a graphical node based function editor.
Use *all*(not all atm) unity Mathf functions, create consstants, use local or world based coordinates of the marching cubes sample point.

No programming needed.
Use those functions created to generate your terrain with the marching cubes algorithm.

Change the shape and amount of chunks you generate.
All in editor, and multithreaded.

TODO:
Write TCP Command Relay server for unity blender communication.
Write blender plugin that connects to Command Relay Server.
Write Unity script that manages starting and stoping of server in editor, as well as a script that communicates with server.
Send chunk meshes between unity and blender for easy mesh manipulation with a tool dedicated to that.
