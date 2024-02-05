# APF_MobileRobots_FiducialMarkers

This repository provides a comprehensive implementation for Mobile Robots using Fiducial Markers and Artificial Potential Fields (APF) in Matlab. The integration involves reacTIVision and TUIO for acquiring position and orientation data. Here are concise instructions for setting up and executing the project:

## Instructions:
### 1. Get reacTIVision

* Download and install reacTIVision from https://reactivision.sourceforge.net/.
* Follow the installation instructions provided on the website.

### 2. Get TUIO

* Download TUIO from https://www.tuio.org/.
* Follow the installation instructions provided on the website.

### 3. Install Matlab

* Ensure that Matlab and Simulink are installed on your system.

### 4. Install Simulink Desktop Real-Time

* Install Simulink Desktop Real-Time library for real-time simulation capabilities.

### 5. Connect your webcam

* Ensure that your webcam is connected and functioning properly.

### 6. Get APF from .stl model

* Use the provided Matlab script "graficar_nube.m" to generate the APF representation from a 3D surface model.
* The repository includes an example .stl file ("planoInclinado.stl").
* Change the file name in line 7 of the Matlab script to use your own 3D model.

### 7. Execution

* Run "graficar_nube.m" to load the APF into the Matlab Workspace as "resultante".
* Launch the reacTIVision application and ensure that markers with ID 0, 1, 4 and 5 are being detected.
* Run "pruebasAPF.slx" simulation in Simulink.
* Start the TUIO_version2 program from Visual Studio Community.

These steps should help you set up and run the APF implementation for Mobile Robots using Fiducial Markers.
