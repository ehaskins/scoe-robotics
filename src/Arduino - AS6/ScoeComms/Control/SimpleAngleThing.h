/*
* SimpleAngleThing.h
*
* Created: 10/6/2012 1:22:01 AM
*  Author: EHaskins
*/


#ifndef SIMPLEANGLETHING_H_
#define SIMPLEANGLETHING_H_
#include "..\Drivers\Input\Accelerometer.h"
#include "..\Drivers\Input\Gyro.h"

#define ANGLE_PRINT_INTERVAL 500
class SimpleAngleThing
{
	public:
	SimpleAngleThing(Gyro *gyro, Accelerometer *xAccel, Accelerometer *yAccel, double gyroWeight, bool updateSensors);
	
	void update();
	void update(float gyroDeltaAngle, float accelX, float accelY);
	double angle;
	Gyro *gyro;
	Accelerometer *xAccel;
	Accelerometer *yAccel;
	double gyroWeight;
	double accelWeight;
	bool updateSensors;
	protected:
	private:
	bool firstAngleUpdate;
	unsigned long lastAnglePrint;
};


#endif /* SIMPLEANGLETHING_H_ */