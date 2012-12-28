/*
 * Skateboard.h
 *
 * Created: 12/17/2012 3:12:48 PM
 *  Author: EHaskins
 */ 


#ifndef SKATEBOARD_H_
#define SKATEBOARD_H_


void loop();
void mainLoop();
void setup();
void printSensors();
void testDrive();
void printAngle();
void balance(float angle, float spin);
bool checkSafties();
void setDrive(float left, float right);
void printTurn();
void printSafties();
float getTurn();
void printDigitalImuCsv();

float deadBand(float val, float deadband, float range);
#endif /* SKATEBOARD_H_ */