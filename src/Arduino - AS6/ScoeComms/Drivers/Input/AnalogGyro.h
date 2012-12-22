/*
 * AnalogGyro.h
 *
 * Created: 12/21/2012 11:06:33 PM
 *  Author: EHaskins
 */ 


#ifndef ANALOGGYRO_H_
#define ANALOGGYRO_H_

#include "Gyro.h"

class AnalogGyro : public Gyro{
	public:
AnalogGyro(int pin, double degreesPerSecPerInput);

	void calibrate();
	void endCalibrate();
	
	void update();

	int getCenterValue(){
		return center;
	}
	void setCenterValue(int val){
		center = val;
	}
	void setResolution(float val){
		resolution = val;
	}
	float getResolution(){
		return resolution;
	}
	private:
	int pin;
	int center;
	float resolution;

	unsigned long calTotal;
	unsigned int calCount;
	unsigned long lastMicros;

};




#endif /* ANALOGGYRO_H_ */