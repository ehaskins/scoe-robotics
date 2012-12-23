/*
* ITG3200Axis.cpp
*
* Created: 12/21/2012 9:12:02 PM
*  Author: EHaskins
*/
#include <Arduino.h>
#include "ITG3200Axis.h"

void ITG3200Axis::update(int val, float elapsedSeconds){
	rawValue = val;

	
	rate = ((val - (int)getCenterValue()) * parent->getResolution()) * (invert ? -1 : 1);
	deltaAngle = rate * elapsedSeconds;
}