/*
* SimpleAngleThing.h
*
* Created: 10/6/2012 1:22:01 AM
*  Author: EHaskins
*/


#ifndef SIMPLEANGLETHING_H_
#define SIMPLEANGLETHING_H_
#include "..\Drivers\Input\Accelerometer.h"
#include "..\Drivers\Input\Gyro.h"

#define ANGLE_PRINT_INTERVAL 500
class SimpleAngleThing
{
	public:
	SimpleAngleThing(Gyro *gyro, Accelerometer *xAccel, Accelerometer *yAccel, double gyroWeight, bool updateSensors){
		this->gyro = gyro;
		this->xAccel = xAccel;
		this->yAccel = yAccel;
		this->gyroWeight = gyroWeight;
		this->accelWeight = 1 - gyroWeight;
		this->updateSensors = updateSensors;
		firstAngleUpdate = true;
	}
	
	void update(){
		if (updateSensors){
			gyro->update();
			xAccel->update();
			yAccel->update();
		}
		
		double accelRads = atan2(xAccel->getAcceleration(), yAccel->getAcceleration());
		double accelAngle = (long) (accelRads * 180 / PI);

		double gyroDelta = gyro->getDeltaAngle();

		if (firstAngleUpdate) {
			angle = accelAngle;
			firstAngleUpdate = false;
		}

		angle = (angle + gyroDelta) * gyroWeight + accelAngle * accelWeight;
		
		/*int now = millis();
		if (now - lastAnglePrint > ANGLE_PRINT_INTERVAL){
			lastAnglePrint = now;
			
			Serial.print("Angle : ");
			Serial.println(angle);
			//Serial.print(" AccelAngle : ");
			//Serial.print(accelAngle);
			//Serial.print(" GyroChange : ");
			//Serial.println(gyroDelta);
		}
		*/
	}
	double angle;
	Gyro *gyro;
	Accelerometer *xAccel;
	Accelerometer *yAccel;
	double gyroWeight;
	double accelWeight;
	bool updateSensors;
	protected:
	private:
	bool firstAngleUpdate;
	unsigned long lastAnglePrint;
};


#endif /* SIMPLEANGLETHING_H_ */