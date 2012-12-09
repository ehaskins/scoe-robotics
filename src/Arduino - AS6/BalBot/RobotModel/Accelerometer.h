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
	Accelerometer (int pin, int center, bool invert){
		this->center = center;
		this->pin = pin;
		this->invert = invert;
	}
	void update(){
		rawValue = (analogRead(pin) - center) * (invert ? -1 : 1);
	}
	bool invert;
	int pin;
	int center;
	int rawValue;
	protected:
	private:
};



#endif /* ACCELEROMETER_H_ */