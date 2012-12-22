/*
* Motor.h
*
* Created: 12/21/2012 5:35:25 PM
*  Author: EHaskins
*/


#ifndef MOTOR_H_
#define MOTOR_H_

class Motor {
	public:
	float getOutput();
	void setOutput(float val);

	void setIsEnabled(bool val);
	bool getIsEnabled();


	protected:
	virtual void updateOutput() = 0;
	virtual void updateEnabled() = 0;
	float output;
	bool isEnabled;
};


#endif /* MOTOR_H_ */