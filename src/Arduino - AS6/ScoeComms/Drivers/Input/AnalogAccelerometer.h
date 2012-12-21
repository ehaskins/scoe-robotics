/*
 * AnalogAccelerometer.h
 *
 * Created: 12/17/2012 1:37:59 PM
 *  Author: EHaskins
 */ 


#ifndef ANALOGACCELEROMETER_H_
#define ANALOGACCELEROMETER_H_

#include "Accelerometer.h"

class AnalogAccelerometer : public Accelerometer {
	public:
		AnalogAccelerometer(int pin, int center, boolean invert){
			this->pin = pin;
			init(center, invert);	
		}
		
		void update(){
			acceleration = (analogRead(pin) - center) * (invert ? -1 : 1);
		}
		
		int pin;
	};


#endif /* ANALOGACCELEROMETER_H_ */