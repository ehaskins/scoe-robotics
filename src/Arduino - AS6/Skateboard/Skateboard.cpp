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
	right.init(9, 11, 8);
	right.setEnabled(true);
	left.setEnabled(true);
}


float drive = 0;
float max = 0.15;
float min = -0.15;
float dir = 0.01;
void loop(){
	drive += dir;
	
	if (drive < min || drive > max)
		dir *= -1;
		
	right.drive(drive);
	left.drive(drive);
	Serial.println(drive);
	delay(100);
}