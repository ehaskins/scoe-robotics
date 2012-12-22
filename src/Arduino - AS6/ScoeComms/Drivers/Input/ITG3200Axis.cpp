/*
 * ITG3200Axis.cpp
 *
 * Created: 12/21/2012 9:12:02 PM
 *  Author: EHaskins
 */ 
#include <Arduino.h>
#include "ITG3200Axis.h"

void ITG3200Axis::update(int val){
		long now = micros();
		long elasped = now - lastMicros;
		lastMicros = now;
		
		elapsedSeconds = (double)elasped / 1000000;
		
		rate = val * parent->getResolution();// - getCenterValue();
		deltaAngle = rate * elapsedSeconds;
}