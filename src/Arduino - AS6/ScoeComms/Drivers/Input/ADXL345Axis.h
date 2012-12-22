/*
 * ADXL345Axis.h
 *
 * Created: 12/21/2012 7:19:47 PM
 *  Author: EHaskins
 */ 


#ifndef ADXL345AXIS_H_
#define ADXL345AXIS_H_

#include "ADXL345.h"
#include "Accelerometer.h"

class ADXL345Axis : public Accelerometer {
	public:
ADXL345Axis(){};
ADXL345Axis (ADXL345 * parent){
	this->parent = parent;
}

void update(){
	parent->update();
}

ADXL345* getParent() const {
	return parent;
}

float getAcceleration();
int getRawValue(){
	return rawValue;
}

void update(int raw);
private:
int rawValue;
ADXL345 *parent;
};


#endif /* ADXL345AXIS_H_ */