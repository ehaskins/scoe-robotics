/*
* Motor.cpp
*
* Created: 12/21/2012 5:44:21 PM
*  Author: EHaskins
*/

#include "Motor.h"
float Motor::getOutput(){
	return output;
}

void Motor::setOutput(float val){
	output = val;
	updateOutput();
}
void Motor::setIsEnabled(bool val) {
	isEnabled = val;
	updateEnabled();
}
bool Motor::getIsEnabled() {
	return isEnabled;
}