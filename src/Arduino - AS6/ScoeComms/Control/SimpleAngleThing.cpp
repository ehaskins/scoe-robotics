/*
* SimpleAngleThing.cpp
*
* Created: 10/6/2012 1:21:49 AM
*  Author: EHaskins
*/

#include "SimpleAngleThing.h"
#include <Arduino.h>
#include <math.h>

SimpleAngleThing::SimpleAngleThing(Gyro *gyro, Accelerometer *xAccel, Accelerometer *yAccel, double gyroWeight, bool updateSensors){
	this->gyro = gyro;
	this->xAccel = xAccel;
	this->yAccel = yAccel;
	this->gyroWeight = gyroWeight;
	this->accelWeight = 1 - gyroWeight;
	this->updateSensors = updateSensors;
	firstAngleUpdate = true;
}


void SimpleAngleThing::update(){
	
	if (updateSensors){
		gyro->update();
		xAccel->update();
		yAccel->update();
	}
	
	update(gyro->getDeltaAngle(), xAccel->getAcceleration(), yAccel->getAcceleration());

}
void SimpleAngleThing::update(float gyroDelta, float accelX, float accelY){
		float accelRads = atan2(accelX, accelY);
		float accelAngle = accelRads * 180 / PI;

		if (firstAngleUpdate) {
			angle = accelAngle;
			firstAngleUpdate = false;
		}

		angle = (angle + gyroDelta) * gyroWeight + accelAngle * accelWeight;
		
		/*
		Serial.print("Angle : ");
		Serial.print(angle);
		Serial.print(" AccelAngle : ");
		Serial.print(accelAngle);
		Serial.print(" GyroChange : ");
		Serial.println(gyroDelta);
		
		*/
}