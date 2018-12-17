# kinematicBoatController
Kinematic boat controller for Unity

## Setup
- Create a GameObject and put the KinematicManager and WaterProviderBasic on it.  
- On your boat object, add the KinematicController, KinematicBuoyancy, and KinematicBasicVehidle components.
-  Optionally, on your camera add the FollowCamBasic component. 

## Notes

This is all still very much a work in progress.  But hopefully it provides at least a starting point for others who need a kinematic boat controller.

KinematicManager.PenetrationTestMask sets which objects should be checked for penetration, and depenetrate from.
KinematicManager.InteractiveObjectMask sets which objects should get pushed away with more force.

Depenetration uses the DepenAccel, DepenDecel, and DepenPushSpeed variables.  However for interactive objects DepenPushSpeed is forced to 1.  Simple and crude.

KinematicBuoyancy has a number of parameters.  Best to just mess around and see what works best.  They are preset to reasonable defaults.

KinematicBasicVehicle is a simplified version of one used in our game. It includes some stuff you probably won't need, like the logic for where to handle player vs ai vs remotely controlled vehicles.

It was also having to work with all of those contexts where it became obvious that the only approach that works for all contexts, is for KinematicController to be fed absolute coordinates for position and rotation.  
