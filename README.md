### What is this?
The final project for our B.Sc. in Software Engineering, an Artificial Reality mobile app that shows you directions in order to navigate inside buildings by setting up points around a physical space.

![image](https://github.com/user-attachments/assets/fcd4d337-b298-4645-ace6-6eab545e8173)

### Technologies used
[![Technologies used](https://skillicons.dev/icons?i=unity,visualstudio,cs)](https://skillicons.dev)

Created in Unity Engine with ARFoundation, Google ARCore Extensions, and Cesium.

### Usage/Build instructions
Open the project in Unity editor version `2022.3.0f1`. 

Required packages that might need manual installation (you can find the .tgz files in the `ContentPackages` folder under the root directory):
- Google ARCore Unity Extension `v1.40.0`
- Cesium for Unity `v1.11.0`

Link your unity project with Unity Cloud services in order to use Unity Cloud Save for user registration/authorization, and Unity Cloud Code to be able to save and delete points (you can find the scripts you will need to add to Cloud Code in the `Assets\Scripts\CloudCodeScripts` folder)

You can turn a user into an admin by updating the user's player data - change the Role in Unity Cloud Save from `"client"` to `"admin"`

Create a Google Developers API key and enable the Map Tiles API and ARCore API to set up a geospatial creator anchor, as well as a Cesium token. refer to this guide: https://developers.google.com/ar/geospatialcreator/unity/quickstart
Add the key to the field in `Project Settings > XR Plug-in Management > ARCore Extensions`
![image](https://github.com/user-attachments/assets/75730b49-2be9-49ad-a95f-d2bb0253ef1d)
