/*
 * Utils.cpp
 *
 *  Created on: Apr 4, 2011
 *      Author: EHaskins
 */

int deadband(int value, int deadband){
	if (value < 127-deadband && value > -128+deadband){
		if (value > deadband){
			value -= deadband;
		}
		else if (value < -deadband){
			value += deadband;
		}
		else{
			value = 0;
		}
	}
	return value;
}

int limit(int value, int min, int max){
	if (value > max)
		value = max;
	else if (value < min)
		value = min;
	return value;
}

double limit(double value, double min, double max){
	if (value > max)
		value = max;
	else if (value < min)
		value = min;
	return value;
}

double removeDeadband(double value, double deadband, double range){
	double output;
	double scale = (range - deadband) / range;

	if (value < 0)
		output = (value - deadband) * scale;
	else if (value > 0)
		output = (value + deadband) * scale;
	else
		output = 0;

	return output;
}

