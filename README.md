# UnityCameraMover
This is a basic script to automate moving the camera. You can add different target positions via the editor UI. When enabling debug mode, you can easily move to the different positions via key presses. It interpolates between two positions and uses a animation curve of your choice for interpolation. 

# Setup
- Add the CameraMover script to your camera 
- Create an empty GameObject. This will be uses as a parent for the different target positions
- Reference this GameObject in your CameraMover script (Cam Positions Parent)
- Now you can move your camera and add different positions via "Add Target Position"


![Editor Preview](https://github.com/Sebastian-Schuchmann/UnityCameraMover/blob/master/Preview%20Image.png?raw=true)
