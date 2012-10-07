/*
* PID.h
*
* Created: 10/6/2012 12:00:06 AM
*  Author: EHaskins
*/


#ifndef PID_H_
#define PID_H_

#include <Arduino.h>

class PID
{
	public:
	double P;
	double I;
	double D;
	
	double iTotal;
	
	PID(double P, double I, double D);
	double update(double current, double desired, double velocity);
	double update(double current, double desired){
		double error = desired - current;
		double elapsed = (double)(micros() - lastMicros) / 1000000;
		double velocity = (error - lastError) / elapsed;
		lastError = error;
		return update(current, desired, velocity);
	}
	double lastError;
	unsigned long lastMicros;
	protected:
	private:
};


#endif /* PID_H_ */