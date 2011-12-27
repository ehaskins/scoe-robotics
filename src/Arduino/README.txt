Needed packages:
Arduino IDE - 0022 - http://arduino.cc/en/Main/Software
Eclipse for C/C++ developers - Helios - http://www.eclipse.org/downloads/
WinAVR - 20081205 - http://sourceforge.net/projects/winavr/files/
Eclipse AVR Plugin - Latest Version - http://avr-eclipse.sourceforge.net/wiki/index.php/Plugin_Download

Process:
Extract Arduino IDE to C:\arduino\
Extract Eclipe to c:\eclipse\
Install WinAVR
Merge C:\arduino-0022\hardware\tools\avr into c:\WinAVR-...
Copy "C:\WinAVR-20081205\etc\avrdude.conf" to "C:\WinAVR-20081205\bin\avrdude.conf"

Create workspace:
Open Eclipse with workspace set to [LOCAL_REPO]\src\Arduino\ where [LOCAL_REPO] is the location of the local source tree.
Close the welcome tab.
Right click in the project explorer, select Import.
Click browse, and browse to [LOCAL_REPO]\src\Arduino\
At least the 'Arduino' and 'RobotTemplate' projects should be found, select any in the list and finish.

Programmer Configuration:
Open "Robot template" properties
Navigate to AVR>AVRDude
Create a new programmer configuration with the following settings:
Programmer: Atmel STK500 V2.x firmware
Port(in the format of): \\.\COMx
Baud: 115200
Name as you feel appropriate.

Save configuration.
Select your configuration from the list in AVRDude settings.
Save and exit properties.
Upload.
