/*
 * AnalogAccelerometer.h
 *
 * Created: 12/17/2012 1:37:59 PM
 *  Author: EHaskins
 */ 


#ifndef ANALOGACCELEROMETER_H_
#define ANALOGACCELEROMETER_H_

#include "Accelerometer.h"

public class AnalogAccelerometer : public Accelerometer{
	public:
		AnalogAccelerometer(int pin, int center, boolean invert){
			this->pin = pin;
		}
		
		void init(
		void update(){
			rawValue = (analogRead(pin) - center) * (invert ? -1 : 1);
		}
	}


#endif /* ANALOGACCELEROMETER_H_ */