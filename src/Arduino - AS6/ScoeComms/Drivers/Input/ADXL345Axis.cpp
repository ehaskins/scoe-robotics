/*
 * ADXL345Axis.cpp
 *
 * Created: 12/21/2012 8:45:04 PM
 *  Author: EHaskins
 */ 

#include "ADXL345Axis.h"

void ADXL345Axis::update(int raw){
	this->rawValue = raw;
}

float ADXL345Axis::getAcceleration(){
	return (float)rawValue*parent->getResolution();
}