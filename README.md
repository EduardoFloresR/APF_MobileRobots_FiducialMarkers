# APF_MobileRobots_FiducialMarkers

This repository provides a comprehensive implementation for Mobile Robots using Fiducial Markers and Artificial Potential Fields (APF) in Matlab. The integration involves reacTIVision and TUIO for acquiring position and orientation data. Here are concise instructions for setting up and executing the project:

## Instructions:
### 1. Get reacTIVision

* Download and install reacTIVision from [reacTIVision.net](https://reactivision.sourceforge.net/).
* Follow the installation instructions provided on the website.

### 2. Get TUIO

* Download TUIO from [tuio.org](https://www.tuio.org/).
* Follow the installation instructions provided on the website.
* This application is desinged to run in Visual Studio Community, download this IDE is required, more information in [VS Community 2022](https://visualstudio.microsoft.com/es/vs/community/).

### 3. Install Matlab

* Ensure that Matlab and Simulink are installed on your system.
* If not installed, you can get them in [MathWorks](https://la.mathworks.com/products/matlab.html).
* Remember that UNAM community has access to MathWorks account, you can get it at [Software UNAM](https://www.software.unam.mx/producto/matlab/).

### 4. Install Simulink Desktop Real-Time

* Install Simulink Desktop Real-Time library for real-time simulation capabilities.
* This library can only run in Windows Pro versions, please check if this is your case.

### 5. Connect your webcam

* Ensure that your webcam is connected and functioning properly.
* Since the camera provides information on the position and orientation of robots and objects in space, it is recommended to place it in an elevated place with a good field of view.

### 6. Get APF from .stl model

* Use the provided Matlab script "graficar_nube.m" to generate the APF representation from a 3D surface model.
* The repository includes an example .stl file ("planoInclinado.stl").
* Change the file name in line 7 of the Matlab script to use your own 3D model.

### 7. Robot preparation

* As Simulink implementation of APF is designed to a (2,0) robot, it sends a packet formed with 2 uint8, each of one represents the speed in one of the motors.
* The robot's board must receive this package and decode it, in order to transmit the information to the motors.
* Note that information is sended as an uint8, and its values range is 0 to 255. In this context 128 means null speed (or stop), meanwhile a lower number represent reverse and a higher number is used for forward direction.

### 8. UDP Configuration

* Enable "Mobile Hotspot" (Zona con cobertura inalámbrica). Make sure the robot's controller board is connected and copy its IP direction.
* Note that this direction is automatically assigned each time Mobile Hotspot 
* Open "pruebasAPF.slx" simulation in Simulink, open "Packet Input UDP Protocol 58625" block the rigth side of the screen by double-clicking on it.
* Access to "Board Setup" and write down the IP direction that was assigned to robot's board. Do not modify UDP port.
* It is recomended to test Device Abailavility by clicking "Test". Finish this configuration clicking "OK" button.

### 9. Execution

* Run "graficar_nube.m" to load the APF into the Matlab Workspace as "resultante".
* Launch the reacTIVision application and make sure that markers with ID 0, 1, 4 and 5 are being detected.
* This TUIO code uses ID 0 and 1 as reference for scale and orientation in reference system. ID 4 is reserved for robot and ID 5 represent APF origin.
* Run "pruebasAPF.slx" simulation in Simulink.
* Start the TUIO_version2 program from Visual Studio Community.
* Robot will start moving only when switch selector block on the right side is setted to "1".
* Similarly, the robot will follow the APF behavior only when switch selector block on the left side is setted to "1". Other case will try to reach marker 5 position.

These steps should help you set up and run the APF implementation for Mobile Robots using Fiducial Markers.
