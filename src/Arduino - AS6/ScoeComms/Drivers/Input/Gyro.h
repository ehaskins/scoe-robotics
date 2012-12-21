/*
 * Gyro.h
 *
 * Created: 10/6/2012 12:46:57 AM
 *  Author: EHaskins
 */ 


#ifndef GYRO_H_
#define GYRO_H_


class Gyro
{
public:
	Gyro(int pin, double degreesPerSecPerInput);
	double update();
	void calibrate();
	void endCalibrate();
	
	double rate;
	double deltaAngle;
	double elapsedSeconds;
	int pin;
	int center;
	double degreesPerSecPerInput;
	
protected:
	unsigned long calTotal;
	unsigned int calCount;
	unsigned long lastMicros;
private:
};


#endif /* GYRO_H_ */