/*
 * ADXL345Axis.cpp
 *
 * Created: 12/21/2012 8:45:04 PM
 *  Author: EHaskins
 */ 

#include "ADXL345Axis.h"

void ADXL345Axis::update(int raw){
	this->rawValue = raw;
	this->acceleration = (float)rawValue * parent->getResolution() * (invert ? -1 : 1);
}