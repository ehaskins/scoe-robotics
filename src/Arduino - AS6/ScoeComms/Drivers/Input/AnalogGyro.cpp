/*
* Gyro.cpp
*
* Created: 10/6/2012 12:46:49 AM
*  Author: EHaskins
*/
#include <Arduino.h>
#include "AnalogGyro.h"

AnalogGyro::AnalogGyro(int pin, double degreesPerSecPerInput){
	this->pin = pin;
	setResolution(degreesPerSecPerInput);
}
void AnalogGyro::update(){
	long now = micros();
	long elasped = now - lastMicros;
	lastMicros = now;
	
	elapsedSeconds = (double)elasped / 1000000;
	
	double val = analogRead(pin);
	val -= center;
	
	rate = val * getResolution();
	deltaAngle = rate * elapsedSeconds;
}

void AnalogGyro::calibrate(){
	calTotal += analogRead(pin);
	calCount++;
}
void AnalogGyro::endCalibrate(){
	center = calTotal / calCount;
	
	calTotal = 0;
	calCount = 0;
}