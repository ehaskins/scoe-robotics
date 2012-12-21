/*
* Accelerometer.h
*
* Created: 10/6/2012 1:12:24 AM
*  Author: EHaskins
*/


#ifndef ACCELEROMETER_H_
#define ACCELEROMETER_H_

class Accelerometer
{
	public:
		
	void init(int center, bool invert){
		this->center = center;
		this->invert = invert;
	}
	
	virtual void update() = 0;
	bool invert;
	int center;
	float acceleration;
};

#endif /* ACCELEROMETER_H_ */