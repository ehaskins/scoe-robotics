/*
 * Skateboard.cpp
 *
 * Created: 12/17/2012 3:13:01 PM
 *  Author: EHaskins
 */ 

#include <Arduino.h>
#include <Drivers\Output\Osmc.h>
Osmc right;
Osmc left;

void setup(){
	Serial.begin(115200);
	left.init(6, 5, 7);
	right.init(11, 9, 8);
	right.setEnabled(true);
	left.setEnabled(true);
}


float drive = 0;
float max = 0.20;
float accel = 0.01;

int count = 0;
void loop(){
	int accelLoops = max / accel;
	int loops = accelLoops * 6;
	
	
	if (count % loops < accelLoops){
		drive += accel;
	}
	else if (count % loops < accelLoops * 3){} //hold speed
	else if (count % loops < accelLoops * 4){
		drive -= accel;
	}		
	else {} //hold speed
	
	count++;
	
	right.drive(drive);
	left.drive(drive);
	Serial.println(drive);

	delay(100);
}