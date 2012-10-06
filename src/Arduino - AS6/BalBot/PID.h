/*
* PID.h
*
* Created: 10/6/2012 12:00:06 AM
*  Author: EHaskins
*/


#ifndef PID_H_
#define PID_H_


class PID
{
	public:
		double P;
		double I;
		double D;
		
		double iTotal;
		
		PID(double P, double I, double D);
		double update(double current, double desired);
	protected:
	private:
		double lastError;
};


#endif /* PID_H_ */