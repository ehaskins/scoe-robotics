/*
* Gyro.cpp
*
* Created: 10/6/2012 12:46:49 AM
*  Author: EHaskins
*/
#include <Arduino.h>
#include "Gyro.h"

Gyro::Gyro(int pin, double degreesPerSecPerInput){
	this->pin = pin;
	this->center = center;
	this->degreesPerSecPerInput = degreesPerSecPerInput;
}
double Gyro::update(){
	long now = micros();
	long elasped = now - lastMicros;
	lastMicros = now;
	
	elapsedSeconds = (double)elasped / 1000000;
	
	double val = analogRead(pin);
	val -= center;
	
	rate = val * degreesPerSecPerInput;
	deltaAngle = rate * elapsedSeconds;
}

void Gyro::calibrate(){
	calTotal += analogRead(pin);
	calCount++;
}
void Gyro::endCalibrate(){
	center = calTotal / calCount;
	
	calTotal = 0;
	calCount = 0;
}