/*
* Skateboard.cpp
*
* Created: 12/17/2012 3:13:01 PM
*  Author: EHaskins
*/

#include <Arduino.h>
#include <Drivers\Output\Osmc.h>
#include <Drivers\Output\RCMotor.h>
#include <Drivers\Input\ADXL345.h>
#include <Drivers\Input\ADXL345Axis.h>
#include <Drivers\Input\ITG3200.h>
#include <Drivers\Input\ITG3200Axis.h>
#include <Control\SimpleAngleThing.h>

#include "Skateboard.h"

Osmc right(6, 5, 7);
Osmc left(9, 11, 8);
ITG3200 *gyro;
ADXL345 *accel;
SimpleAngleThing *angleCalc;

unsigned long lastPrint;

void setup(){
	Serial.begin(115200);
	
	gyro = new ITG3200();
	
	Serial.println("Waiting to calibrate...");
	delay(1000);
	Serial.println("Calibrating...");
	gyro->calibrate();
	Serial.println("Calibration complete.");
	
	accel = new ADXL345();
	
	angleCalc = new SimpleAngleThing(gyro->getY(), accel->getX(), accel->getZ(), 0.98, false);
	
	right.setIsEnabled(true);
	left.setIsEnabled(true);
	
	delay(200);
	Serial.println("ready!");
	lastPrint = 0;
}


float drive = 0;
float max = 0.15;
float min = -0.15;
float dir = 0.01;
void loop(){
	if (accel->update()){
		gyro->update();
		angleCalc->update(gyro->getY()->getDeltaAngle(), accel->getX()->getAcceleration() * -1, accel->getZ()->getAcceleration());
		printSensors();
	}	
}

void printAngle(){
	Serial.println(angleCalc->angle);
}
void printSensors(){
	//if (accel->update()){
		Serial.print(accel->getX()->getAcceleration());
		Serial.print(",");
		Serial.print(accel->getY()->getAcceleration());
		Serial.print(",");
		Serial.print(accel->getZ()->getAcceleration());
	//}
	//if (gyro->update()){
		unsigned long now = micros();
		Serial.print(",");
		Serial.print(gyro->getX()->getRate());
		Serial.print(",");
		Serial.print(gyro->getY()->getRate());
		Serial.print(",");
		Serial.print(gyro->getZ()->getRate());
		Serial.print(",");
		Serial.println(now-lastPrint);
		lastPrint = now;
	//}
}

void testDrive(){
	drive += dir;
	
	if (drive < min || drive > max)
	dir *= -1;
	
	right.setOutput(drive);
	left.setOutput(drive);
	Serial.println(drive);
	delay(100);
}