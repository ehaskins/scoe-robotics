/*
* RCMotor.h
*
* Created: 12/21/2012 6:04:09 PM
*  Author: EHaskins
*/


#ifndef RCMOTOR_H_
#define RCMOTOR_H_

#include "Motor.h"

class RCMotor : public Motor{
	public:
	RCMotor (int signalPin);

	protected:
	void updateOutput();
	void updateEnabled();

	private:
	int signalPin;

};



#endif /* RCMOTOR_H_ */