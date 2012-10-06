/*
 * BalBot.h
 *
 * Created: 10/4/2012 12:16:09 AM
 *  Author: EHaskins
 */ 


#ifndef BALBOT_H_
#define BALBOT_H_

void calibrate(int calibrationDelay, int calibrationLoops);
void balance(double desiredAngle, double spin);
void setDrive(int left, int right);
void writeLed(bool state);
#endif /* BALBOT_H_ */