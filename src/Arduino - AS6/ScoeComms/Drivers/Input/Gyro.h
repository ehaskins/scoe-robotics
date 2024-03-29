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
	Gyro(){};
	virtual void update();
	
	float getRate(){
		return rate;
	}
	float getDeltaAngle(){
		return deltaAngle;
	}
	float getElapsedSeconds(){
		return elapsedSeconds;
	}
	
	float getCenterValue(){
		return centerValue;
	}
	void setCenterValue(float val){
		centerValue = val;
	}
	int getRawValue(){
		return rawValue;
	}
	bool getInvert(){
		return invert;
	}
	void setInvert(bool val){
		invert = val;
	}
	protected:
	bool invert;
	int rawValue;
	float centerValue;
	float rate;
	float deltaAngle;
	float elapsedSeconds;
	private:
};


#endif /* GYRO_H_ */