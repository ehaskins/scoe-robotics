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
	Accelerometer (){}
		
	void init(int center, bool invert){
		this->center = center;
		this->invert = invert;
	}
	
	abstract void update();
	bool invert;
	int center;
	int rawValue;
	protected:
	private:
};



#endif /* ACCELEROMETER_H_ */