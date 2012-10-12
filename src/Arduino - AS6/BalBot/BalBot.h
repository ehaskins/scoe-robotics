/*
 * BalBot.h
 *
 * Created: 10/4/2012 12:16:09 AM
 *  Author: EHaskins
 */ 


#ifndef BALBOT_H_
#define BALBOT_H_

void calibrate(int calibrationDelay, int calibrationLoops);
void balance(float desiredAngle, float spin);
void setDrive(float left, float right);
void writeLed(bool state);
#endif /* BALBOT_H_ */