/*
* PID.cpp
*
* Created: 10/5/2012 11:59:57 PM
*  Author: EHaskins
*/
#include "PID.h"

PID::PID(double P, double I, double D){
	this->P = P;
	this->I = I;
	this->D = D;
}
double PID::update(double current, double desired, double velocity){
	double error = desired - current;
	
	double output = 0.0;
	
	output = error * P;
	iTotal += error * I;
	output += iTotal;
	output += velocity * D;
	
	return output;
}