/*
 * Skateboard.cpp
 *
 * Created: 12/17/2012 3:13:01 PM
 *  Author: EHaskins
 */ 

#include <Arduino.h>
#include <Drivers\Output\Osmc.h>
#include <Drivers\Output\RCMotor.h>
Osmc right(6, 5, 7);
Osmc left(9, 11, 8);

void setup(){
	Serial.begin(115200);
	right.setIsEnabled(true);
	left.setIsEnabled(true);
}


float drive = 0;
float max = 0.15;
float min = -0.15;
float dir = 0.01;
void loop(){
	drive += dir;
	
	if (drive < min || drive > max)
		dir *= -1;
		
	right.setOutput(drive);
	left.setOutput(drive);
	Serial.println(drive);
	delay(100);
}